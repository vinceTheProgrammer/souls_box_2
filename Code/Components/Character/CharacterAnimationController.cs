using Sandbox;
using Sandbox.Citizen;
using System;

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

		public bool IsDoingAnimation { get; set; }
		public bool CanInterrupt {  get; set; }
		public bool IsPastMidwayPoint {  get; set; }

		protected override void OnFixedUpdate()
		{
			if ( AnimationHelper != null )
			{
				if ( Agent.IsRolling && CanInterrupt )
				{
					AnimationHelper.Target.Set( "sb_interrupt", true );
					IsPastMidwayPoint = false;
					CanInterrupt = false;
					IsDoingAnimation = false;
					MovementController.SetLastMove = false;
					Agent.IsRolling = false;
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

				//Log.Info( GameObject.Parent.Name + ": \n" + "IsDoingAnimation: " + IsDoingAnimation + "\nAgent.IsRolling: " + Agent.IsRolling + "\nisPlayer: " + isPlayer + "\nlockedOn: " + lockedOn + "\ncurrentLockOnAble: " + currentLockOnAble + "\nAgent.IsJumping: " + Agent.IsJumping + "\nAgent.IsBackstepping: " + Agent.IsBackstepping + "\nAgent.IsLightAttacking: " + Agent.IsLightAttacking );

				if ( !IsDoingAnimation && Agent.IsRolling && isPlayer && lockedOn && currentLockOnAble != null )
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
				else if ( !IsDoingAnimation && Agent.IsRolling == true )
				{
					Log.Info( "rolling!" );
					SetAnimgraphParam( "roll_forward", true, rollYaw );
				}
				else if ( !IsDoingAnimation && Agent.IsJumping == true )
				{
					SetAnimgraphParam( "sb_jump", true, rollYaw );
				}
				else if ( !IsDoingAnimation && Agent.IsBackstepping == true )
				{
					SetAnimgraphParam( "sb_backstep", true );
				}
				else if ( !IsDoingAnimation && Agent.IsLightAttacking == true )
				{
					Log.Info( "test" );
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
			/*
			SkinnedModelRenderer skinnedModelRenderer = GameObject.Components.Get<SkinnedModelRenderer>();
			if (skinnedModelRenderer != null )
			{
				Log.Info( GameObject.Parent.Name + " ooo" );
				ClothingContainer clothingContainer = ClothingContainer.CreateFromLocalUser();
				clothingContainer.Apply(skinnedModelRenderer);
			}
			*/

			AnimationHelper.Target.OnGenericEvent = ( SceneModel.GenericEvent genericEvent ) =>
			{
				switch ( genericEvent.String )
				{
					case "roll_start":
					case "roll2_start":
						Agent.IsRolling = true;
						CanInterrupt = false;
						break;
					case "jump_start":
						Agent.IsJumping = true;
						break;
					case "backstep_start":
						Agent.IsBackstepping = true;
						break;
					case "roll_end":
					case "roll2_end":
					case "light_attack_sword_end":
						Log.Info( "Setting false" );
						Agent.IsRolling = false;
						Agent.IsLightAttacking = false;
						IsPastMidwayPoint = false;
						CanInterrupt = false;
						IsDoingAnimation = false;
						MovementController.SetLastMove = false;
						Agent.IsContinuing = false;
						break;
					case "light_attack_reset":
						Agent.IsRolling = false;
						Agent.IsLightAttacking = true;
						IsPastMidwayPoint = false;
						CanInterrupt = false;
						IsDoingAnimation = true;
						MovementController.SetLastMove = false;
						Agent.IsContinuing = false;
						break;
					case "jump_end":
						Agent.IsJumping = false;
						IsPastMidwayPoint = false;
						IsDoingAnimation = false;
						break;
					case "backstep_end":
						Agent.IsBackstepping = false;
						IsPastMidwayPoint = false;
						IsDoingAnimation = false;
						break;
					case "midway":
						IsPastMidwayPoint = true;
						break;
					case "can_interrupt":
						CanInterrupt = true;
						break;
				}
			};
		}

		private void SetAnimgraphParam(string param, bool value, float agentYaw, bool isDoingAnimation = true)
		{
			Agent.Transform.Rotation = Rotation.FromYaw( agentYaw );
			AnimationHelper.Target.Set( param, value );
			Log.Info( "setting to " + isDoingAnimation);
			IsDoingAnimation = isDoingAnimation;
		}

		private void SetAnimgraphParam( string param, bool value, bool isDoingAnimation = true)
		{
			AnimationHelper.Target.Set( param, value );
			Log.Info( "setting to " + isDoingAnimation );
			IsDoingAnimation = isDoingAnimation;
		}

		private void PrintDebug()
		{
			Log.Info( "MovementController.IsRolling: " + Agent.IsRolling );
			Log.Info( "Agent.IsRolling: " + Agent.IsRolling );
			Log.Info( "Agent.IsLightAttacking: " + Agent.IsLightAttacking );
			Log.Info( "IsPastMidwayPoint: " + IsPastMidwayPoint );
			Log.Info( "CanInterrupt: " + CanInterrupt );
			Log.Info( "IsDoingAnimation: " + IsDoingAnimation );
			Log.Info( "MovementController.SetLastMove: " + MovementController.SetLastMove );
			Log.Info( "MovementController.IsAttacking: " + Agent.IsLightAttacking );
		}
	}
}
