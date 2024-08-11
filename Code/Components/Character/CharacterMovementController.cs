using Sandbox.Citizen;
using System;
using Sandbox;

namespace SoulsBox
{
	/// <summary>
	/// Interface between s&box character controller and souls box agent
	/// </summary>
	[Title( "Souls Box Character Movement Controller" )]
	[Category( "Souls Box Character" )]
	[Icon( "directions_run" )]
	public sealed class CharacterMovementController : Component
	{
		public bool SetLastMove;
		public Vector3 LastMove;

		/// <summary>
		/// Character walk speed
		/// </summary>
		[Property]
		public float WalkSpeed { get; set; }

		/// <summary>
		/// Character run speed
		/// </summary>
		[Property]
		public float RunSpeed { get; set; }

		[Property]
		public float TerminalVelocity = 200f;

		[Property]
		public CharacterController CharacterController { get; set; }

		[Property]
		public CharacterAnimationController CharacterAnimationController { get; set; }

		[Property]
		public CharacterAgent Agent { get; set; }

		protected override void OnFixedUpdate()
		{
			if (Network.IsProxy)
			{
				return;
			}
			else
			{
				HandleLocalMovement();
			}
		}

		private void HandleLocalMovement()
		{
			if ( !CharacterController.IsOnGround )
			{
				CharacterAnimationController.AnimationHelper.IsGrounded = CharacterController.IsOnGround;
				CharacterController.Accelerate( Vector3.Down * TerminalVelocity );
				CharacterController.Acceleration = 4.9f;
				CharacterController.Move();
				return;
			}

			if ( Agent.IsRolling )
			{
				HandleRolling();
			}
			else if ( Agent.IsJumping )
			{
				HandleJumping();
			}
			else if ( Agent.IsBackstepping )
			{
				HandleBackstepping();
			}
			else if ( Agent.IsLightAttacking )
			{
				HandleAttacking();
			}
			else
			{
				HandleDefaultMovement();
			}
		}

		private void HandleRolling()
		{
			Vector3 targetDirection = LastMove;

			if (Agent is AgentPlayer player )
			{
				Log.Info( player.CurrentLockOnAble );
				Log.Info( player.CurrentLockOnAble );
				if ( player.LockedOn && player.CurrentLockOnAble != null )
				{
					if ( !SetLastMove )
					{
						Log.Info( "Hello chap" );
						targetDirection = player.MoveVectorRelativeToCamera * (player.Transform.Position - player.CurrentLockOnAblePosition).EulerAngles;
						LastMove = targetDirection;
						SetLastMove = true;
					}
				}
				else
				{
					targetDirection = player.Transform.Rotation.Forward;
				}
			}
			else
			{
				targetDirection = Agent.Transform.Rotation.Forward;
			}

			if (Agent is AgentPlayer player_)
			{
				HandleMovement( targetDirection, player_.LockedOn ? 0.1f : 0.05f, 300.0f );
			}
			else
			{
				HandleMovement( targetDirection, 0.05f, 300.0f );
			}
		}

		private void HandleJumping()
		{
			//Log.Info( CharacterController.Transform.Rotation.Forward );
			HandleMovement( Agent.Transform.Rotation.Forward, 0.05f, RunSpeed );
		}

		private void HandleBackstepping()
		{
			HandleMovement( Agent.Transform.Rotation.Backward, 0.05f, 100.0f );
		}

		private void HandleAttacking()
		{
			CharacterController.MoveTo( Transform.Position + CharacterAnimationController.AnimationHelper.Target.RootMotion.Position.Length * Transform.Rotation.Forward.Normal * 7.5f, true );
		}

		private void HandleDefaultMovement()
		{
			float targetSpeed = Agent.IsSprinting && !Agent.IsGuarding ? RunSpeed : WalkSpeed;
			Vector3 targetVelocity;
			if (Agent is AgentPlayer player)
			{
				targetVelocity = player.MoveVectorRelativeToCamera.Normal * targetSpeed;
			} else
			{
				targetVelocity = Agent.MoveVector.Normal * targetSpeed;
			}

			CharacterController.Accelerate( targetVelocity );
			CharacterController.Acceleration = 10.0f;
			CharacterController.ApplyFriction( 5.0f );
			CharacterController.Move();

			if (Agent is AgentPlayer player_)
			{
				if ( !player_.LockedOn || player_.IsSprinting)
				{
					//Log.Info( GameObject.Parent.Name + " hi" );
					Transform.Rotation = Rotation.Lerp( Transform.Rotation, player_.LastMoveDirectionRotation, 0.1f );
				}
				else
				{
					//Log.Info( GameObject.Parent.Name + " bye" );
					Vector3 targetToPlayerDisplacement = (player_.CurrentLockOnAblePosition - Transform.Position);
					Rotation faceDirection = Rotation.FromYaw( targetToPlayerDisplacement.Normal.EulerAngles.yaw );
					Transform.Rotation = Rotation.Lerp( Transform.Rotation, faceDirection, 0.5f );
				}
			}
		}

		private void HandleMovement( Vector3 targetDirection, float lerpFactor, float baseSpeed )
		{
			Vector3 currentVelocity = CharacterController.Velocity;
			float targetSpeed = Agent.IsSprinting && !Agent.IsGuarding ? RunSpeed : WalkSpeed;
			Vector3 targetVelocity = targetDirection.Normal * baseSpeed;
			Vector3 targetLerpVelocity = Agent.MoveVector.Normal * targetSpeed;

			float currentDotTargetVelocity = targetLerpVelocity.Dot( currentVelocity );
			float currentDotCurrentVelocity = currentVelocity.Dot( currentVelocity );
			targetLerpVelocity = MathF.Max( 0, (currentDotTargetVelocity / currentDotCurrentVelocity) ) * currentVelocity;

			if ( CharacterAnimationController.IsTagActive( "SB_Past_Midway" ))
			{
				targetVelocity = currentVelocity.LerpTo( targetLerpVelocity, lerpFactor, true );
				if ( float.IsNaN( targetVelocity.x ) || float.IsNaN( targetVelocity.y ) || float.IsNaN( targetVelocity.z ) )
				{
					targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
				}
			}

			CharacterController.Velocity = targetVelocity;
			CharacterController.Move();
		}
	}
}
