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

		public bool IsDoingAnimation { get; set; }
		public bool CanInterrupt {  get; set; }
		public bool IsPastMidwayPoint {  get; set; }

		protected override void OnFixedUpdate()
		{
			if ( AnimationHelper != null )
			{
				if ( Input.Pressed( "sb_roll" ) && CanInterrupt && MovementController.IsRolling )
				{
					AnimationHelper.Target.Set( "sb_interrupt", true );
					MovementController.IsRolling = false;
					IsPastMidwayPoint = false;
					CanInterrupt = false;
					IsDoingAnimation = false;
					MovementController.SetLastMove = false;
					Agent.IsRolling = false;
				}

				AnimationHelper.IsGrounded = MovementController.CharacterController.IsOnGround;
				AnimationHelper.WithVelocity( MovementController.CharacterController.Velocity );

				float rollYawInRelationToCamera = Agent.GetMoveVector().EulerAngles.yaw;

				if ( !IsDoingAnimation && Agent.IsRolling && Agent.LockedOn )
				{
					float horizontalCardinalDirection = Input.AnalogMove.RoundToCardinal().y;
					float rollYawInRelationToTarget = (Input.AnalogMove * (Agent.CurrentLockOnAble.Transform.Position - Agent.Transform.Position).EulerAngles).EulerAngles.yaw;


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
					SetAnimgraphParam( "roll_forward", true, rollYawInRelationToCamera );
				}
				else if ( IsDoingAnimation && Agent.IsJumping == true )
				{
					SetAnimgraphParam( "sb_jump", true, rollYawInRelationToCamera );
				}
				else if ( IsDoingAnimation && Agent.IsBackstepping == true )
				{
					SetAnimgraphParam( "sb_backstep", true );
				}
				else if ( IsDoingAnimation && Agent.IsLightAttacking == true )
				{
					MovementController.IsAttacking = true;
					SetAnimgraphParam( "sb_light_attack_sword", true );
				}
			}
		}

		protected override void OnStart()
		{
			AnimationHelper.Target.OnGenericEvent = ( SceneModel.GenericEvent genericEvent ) =>
			{
				switch ( genericEvent.String )
				{
					case "roll_start":
					case "roll2_start":
						MovementController.IsRolling = true;
						CanInterrupt = false;
						break;
					case "jump_start":
						MovementController.IsJumping = true;
						break;
					case "backstep_start":
						MovementController.IsBackstepping = true;
						break;
					case "roll_end":
					case "roll2_end":
					case "light_attack_sword_end":
						MovementController.IsRolling = false;
						Agent.IsRolling = false;
						Agent.IsLightAttacking = false;
						IsPastMidwayPoint = false;
						CanInterrupt = false;
						IsDoingAnimation = false;
						MovementController.SetLastMove = false;
						MovementController.IsAttacking = false;
						break;
					case "light_attack_reset":
						MovementController.IsRolling = false;
						Agent.IsRolling = false;
						Agent.IsLightAttacking = true;
						IsPastMidwayPoint = false;
						CanInterrupt = false;
						IsDoingAnimation = true;
						MovementController.SetLastMove = false;
						break;
					case "jump_end":
						MovementController.IsJumping = false;
						Agent.IsJumping = false;
						IsPastMidwayPoint = false;
						IsDoingAnimation = false;
						break;
					case "backstep_end":
						MovementController.IsBackstepping = false;
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
			IsDoingAnimation = isDoingAnimation;
		}

		private void SetAnimgraphParam( string param, bool value, bool isDoingAnimation = true)
		{
			AnimationHelper.Target.Set( param, value );
			IsDoingAnimation = isDoingAnimation;
		}
	}
}
