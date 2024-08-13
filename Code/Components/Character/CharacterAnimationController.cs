using Sandbox;
using Sandbox.Citizen;
using System;
using System.Diagnostics;

namespace SoulsBox
{
	/// <summary>
	/// Interface between s&box character animation and souls box character
	/// </summary>
	[Title( "Souls Box Character Animation Controller" )]
	[Category( "Souls Box Character" )]
	[Icon( "movie" )]
	public sealed class CharacterAnimationController : Component
	{
		[Property]
		public CharacterAgent Agent { get; set; }

		[Property]
		public CharacterMovementController MovementController { get; set; }

		[Property]
		public CitizenAnimationHelper AnimationHelper { get; set; }

		[Property]
		public SkinnedModelRenderer ModelRenderer { get; set; }

		public AnimTagEventManager AnimTagEventManager { get; set; }

		protected override void OnFixedUpdate()
		{
			AnimationHelper.IsGrounded = MovementController.CharacterController.IsOnGround;
			AnimationHelper.WithVelocity( MovementController.CharacterController.Velocity );

			if ( Network.IsProxy ) return;

			if ( AnimationHelper != null )
			{
				if (Agent.IsDead)
				{
					SetAnimgraphParam( "sb_death", true );
					return;
				} else if (Agent is AgentPlayer _player)
				{
					if (_player.IsRespawning )
					{
						SetAnimgraphParam( "sb_respawn", true );
						return;
					}
				}

				if ( Agent.IsRolling && IsTagActive("SB_Can_Interrupt" ))
				{
					SetAnimgraphParam( "sb_interrupt", true );
				}

				float rollYaw = Agent.MoveVector.EulerAngles.yaw;

				bool isPlayer = false;
				bool lockedOn = false;
				LockOnAble currentLockOnAble = null;
				if ( Agent is AgentPlayer player )
				{
					isPlayer = true;
					lockedOn = player.LockedOn;
					currentLockOnAble = player.CurrentLockOnAble;
					rollYaw = player.MoveVectorRelativeToCamera.EulerAngles.yaw;
				}

				if ( !IsTagActive("SB_Doing_Animation") && Agent.IsRolling && isPlayer && lockedOn && currentLockOnAble != null )
				{
					float horizontalCardinalDirection = Agent.MoveVector.RoundToCardinal().y;
					float rollYawInRelationToTarget = (Agent.MoveVector * (currentLockOnAble.Transform.Position - Agent.Transform.Position).EulerAngles).EulerAngles.yaw;

					if ( horizontalCardinalDirection > 0 )
					{
						SetAnimgraphParam( "sb_roll2", true, rollYawInRelationToTarget );
					}
					else if ( horizontalCardinalDirection < 0 )
					{

						SetAnimgraphParam( "sb_roll2_mirror", true, rollYawInRelationToTarget );
					}
					else
					{
						SetAnimgraphParam( "roll_forward", true, rollYawInRelationToTarget );
					}
				}
				else if ( !IsTagActive( "SB_Doing_Animation" ) && Agent.IsRolling == true )
				{
					SetAnimgraphParam( "roll_forward", true, rollYaw );
				}
				else if ( !IsTagActive( "SB_Doing_Animation" ) && Agent.IsJumping == true )
				{
					SetAnimgraphParam( "sb_jump", true, rollYaw );
				}
				else if ( !IsTagActive( "SB_Doing_Animation" ) && Agent.IsBackstepping == true )
				{
					SetAnimgraphParam( "sb_backstep", true );
				} else if ( Agent.IsLightAttacking == true )
				{
					if ( !IsTagActive( "SB_Doing_Animation" ) && !IsTagActive( "SB_Can_Continue" ) )
					{
						SetAnimgraphParam( "sb_light_attack_sword", true );
					}
				}

				if ( Agent.IsContinuing )
				{
					if ( IsTagActive( "SB_Can_Continue" ) && !IsTagActive("SB_Attacking") )
					{
						SetAnimgraphParam( "sb_continue_combo", true );
					}
				}
			}
			
		}

		protected override void OnStart()
		{
			AnimTagEventManager = new AnimTagEventManager();
			AnimTagEventManager.RegisterRenderer( AnimationHelper.Target );
			RegisterAgentResets();

		}

		[Broadcast]
		private void SetAnimgraphParam(string param, bool value, float agentYaw)
		{
			if (!Network.IsProxy) Agent.Transform.Rotation = Rotation.FromYaw( agentYaw );
			AnimationHelper.Target.Set( param, value );
		}

		[Broadcast]
		private void SetAnimgraphParam( string param, bool value)
		{
			AnimationHelper.Target.Set( param, value );
		}

		public bool IsTagActive(string tagName)
		{
			return AnimTagEventManager.IsTagActive( tagName );
		}

		private void RegisterAgentResets()
		{
			AnimTagEventManager.RegisterTagCallback( "SB_Rolling", SceneModel.AnimTagStatus.End, () =>
			{
				Agent.IsRolling = false;
				MovementController.SetLastMove = false;
			});
			AnimTagEventManager.RegisterTagCallback( "SB_Jumping", SceneModel.AnimTagStatus.End, () =>
			{
				Agent.IsJumping = false;
			});
			AnimTagEventManager.RegisterTagCallback( "SB_Backstepping", SceneModel.AnimTagStatus.End, () =>
			{
				Agent.IsBackstepping = false;
			});
			AnimTagEventManager.RegisterTagCallback( "SB_Can_Continue", SceneModel.AnimTagStatus.Start, () =>
			{
				Agent.IsLightAttacking = false;
				Agent.IsContinuing = false;
			});
			AnimTagEventManager.RegisterTagCallback( "SB_Attacking", SceneModel.AnimTagStatus.Start, () =>
			{
				Agent.IsContinuing = false;
				Agent.IsContinuing = false;
			});
			AnimTagEventManager.RegisterTagCallback( "SB_Hitbox_Active", SceneModel.AnimTagStatus.Start, () =>
			{
				Agent.CharacterCombatController.CanDealDamage = true;	
			});
			AnimTagEventManager.RegisterTagCallback( "SB_Dying", SceneModel.AnimTagStatus.End, () =>
			{
				if (Agent is AgentPlayer player)
				{
					player.CanRespawn = true;
					player.Respawn();
				}
			} );

		}

		public void AnimateHitReaction(DamageInfo damageInfo)
		{
			Vector3 force = (damageInfo.Attacker.Transform.Position - Transform.Position).Normal * 5f;
			AnimationHelper.ProceduralHitReaction( damageInfo, force: force );
		}
	}
}
