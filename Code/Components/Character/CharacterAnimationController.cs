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
			if ( AnimationHelper != null )
			{
				if ( Agent.IsRolling && IsTagActive("SB_Can_Interrupt" ))
				{
					AnimationHelper.Target.Set( "sb_interrupt", true );
				}

				AnimationHelper.IsGrounded = MovementController.CharacterController.IsOnGround;
				AnimationHelper.WithVelocity( MovementController.CharacterController.Velocity );

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
					Log.Info( "rolling!" );
					SetAnimgraphParam( "roll_forward", true, rollYaw );
				}
				else if ( !IsTagActive( "SB_Doing_Animation" ) && Agent.IsJumping == true )
				{
					SetAnimgraphParam( "sb_jump", true, rollYaw );
				}
				else if ( !IsTagActive( "SB_Doing_Animation" ) && Agent.IsBackstepping == true )
				{
					SetAnimgraphParam( "sb_backstep", true );
				}
				else if ( !IsTagActive( "SB_Doing_Animation" ) && Agent.IsLightAttacking == true )
				{
					SetAnimgraphParam( "sb_light_attack_sword", true );
				}
			}
				
			if (Agent.IsContinuing)
			{
				AnimationHelper.Target.Set( "sb_continue_combo", true );
			}
		}

		protected override void OnStart()
		{
			AnimTagEventManager = new AnimTagEventManager();
			AnimTagEventManager.RegisterRenderer( AnimationHelper.Target );
			RegisterAgentResets();

		}

		private void SetAnimgraphParam(string param, bool value, float agentYaw)
		{
			Agent.Transform.Rotation = Rotation.FromYaw( agentYaw );
			AnimationHelper.Target.Set( param, value );
		}

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
			});
			AnimTagEventManager.RegisterTagCallback( "SB_Jumping", SceneModel.AnimTagStatus.End, () =>
			{
				Agent.IsJumping = false;
			});
			AnimTagEventManager.RegisterTagCallback( "SB_Backstepping", SceneModel.AnimTagStatus.End, () =>
			{
				Agent.IsBackstepping = false;
			});
		}
	}
}
