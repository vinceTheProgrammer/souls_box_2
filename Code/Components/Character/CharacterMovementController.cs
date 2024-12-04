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
		public float TerminalVelocity = 100000f;

		[Property]
		public float CreationModeSpeed = 100f;

		[Property]
		public CharacterController CharacterController { get; set; }

		[Property]
		public CharacterAnimationController CharacterAnimationController { get; set; }

		[Property]
		public CharacterAgent Agent { get; set; }

		private Vector3 CreationModeCurrentVelocity { get; set; } = Vector3.Zero;

		private float FallStartingZ { get; set; }

		private TimeSince TimeSinceFallStarted { get; set; }
		private bool StartedFalling { get; set; }


		protected override void OnUpdate()
		{
			if ( Network.IsProxy ) return;
			if (Agent is AgentPlayer player)
			{
				if ( player.IsUsingBonfire ) return;
				if ( player.CreationMode)
				{
					ApplyCreationModeVelocity( player );
				}
			}
		}

		protected override void OnFixedUpdate()
		{
			if ( Network.IsProxy) return;

			if ( !CharacterController.IsOnGround )
			{
				if (!StartedFalling)
				{
					StartedFalling = true;
					TimeSinceFallStarted = 0;
					FallStartingZ = CharacterController.Transform.Position.z;
				}
				if ( StartedFalling && TimeSinceFallStarted > 10 )
				{
					Agent.CharacterVitals.Die();
					FallStartingZ = 0;
					StartedFalling = false;
				}
			}
			else
			{
				float fallHeight = FallStartingZ - CharacterController.Transform.Position.z;
				if ( StartedFalling && fallHeight > 1000 )
				{
					Agent.CharacterVitals.Die();
					//Log.Info( $"{FallStartingZ} - {CharacterController.Transform.Position.z} = {fallHeight}" );
				}
				FallStartingZ = 0;
				StartedFalling = false;
			}

			if ( Agent is AgentPlayer player )
			{
				if ( player.IsUsingBonfire ) return;
				if ( player.CreationMode )
				{
					HandleCreationModeMovement( player );
					return;
				}
			}

			if ( Agent.CharacterAnimationController.IsTagActive( "SB_Stationary" ) ) return;

			//Log.Info(GameObject.Name + " " + Agent.IsDead );

			if (Agent.IsDead) return;

			HandleLocalMovement();
		}

		private void HandleLocalMovement()
		{
			if ( !CharacterController.IsOnGround )
			{
				CharacterAnimationController.AnimationHelper.IsGrounded = CharacterController.IsOnGround;
				//Log.Info( CharacterController.Velocity.z );
				Vector3 targetLerp = CharacterController.Velocity.LerpTo( 0, 0.01f );
				CharacterController.Velocity = new Vector3( targetLerp.x, targetLerp.y, CharacterController.Velocity.z );
				if (CharacterController.Velocity.z > -TerminalVelocity) CharacterController.Punch( Vector3.Down * 9.8f );
				CharacterController.Move();
				return;
			}

			if ( Agent.IsRolling)
			{
				HandleRolling();
			}
			else if ( Agent.IsJumping)
			{
				HandleJumping();
			}
			else if ( Agent.IsBackstepping )
			{
				HandleBackstepping();
			}
			else if ( CharacterAnimationController.IsTagActive("SB_Full_Attack") )
			{
				HandleAttacking();
				if (CharacterAnimationController.IsTagActive("SB_Can_Rotate")) HandleDefaultRotation(0.3f);
			}
			else
			{
				HandleDefaultMovement();
				HandleDefaultRotation();
			}
		}

		private void HandleRolling()
		{

			LerpControllerHeight( 30f );

			Vector3 targetDirection = LastMove;

			if (Agent is AgentPlayer player )
			{
				if ( player.LockedOn && player.CurrentLockOnAble != null )
				{
					if ( !SetLastMove )
					{
						targetDirection = player.MoveVector * (player.CurrentLockOnAblePosition - player.Transform.Position).EulerAngles;
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
			LerpControllerHeight( 72f );
			HandleMovement( Agent.Transform.Rotation.Forward, 0.05f, RunSpeed );
			CharacterController.MoveTo( Transform.Position + new Vector3(0,0,CharacterAnimationController.AnimationHelper.Target.RootMotion.Position.z * 200f), true );
		}

		private void HandleBackstepping()
		{
			LerpControllerHeight( 72f );
			HandleMovement( Agent.Transform.Rotation.Backward, 0.05f, 100.0f );
		}

		private void HandleAttacking()
		{
			LerpControllerHeight( 72f );
			CharacterController.MoveTo( Transform.Position + CharacterAnimationController.AnimationHelper.Target.RootMotion.Position.Length * Transform.Rotation.Forward.Normal * 3.0f, true );
		}

		private void HandleDefaultRotation(float lerp = 0.1f)
		{
			if ( Agent is AgentPlayer player_ )
			{
				if ( !player_.LockedOn || player_.IsSprinting )
				{
					Transform.Rotation = Rotation.Lerp( Transform.Rotation, player_.LastMoveDirectionRotation, lerp );
				}
				else
				{
					Vector3 targetToPlayerDisplacement = (player_.CurrentLockOnAblePosition - Transform.Position);
					Rotation faceDirection = Rotation.FromYaw( targetToPlayerDisplacement.Normal.EulerAngles.yaw );
					Transform.Rotation = Rotation.Lerp( Transform.Rotation, faceDirection, 0.5f );
				}
			}
		}

		private void HandleDefaultMovement()
		{
			LerpControllerHeight( 72f );

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

		private void LerpControllerHeight(float height)
		{
			CharacterController.Height = CharacterController.Height.LerpTo( height, 0.1f );
		}

		private void HandleCreationModeMovement(AgentPlayer player)
		{
			Vector3 movementX = Input.AnalogMove.x * player.CameraController.Camera.Transform.Rotation.Forward.Normal * Time.Delta * CreationModeSpeed;
			Vector3 movementY = Input.AnalogMove.y * player.CameraController.Camera.Transform.Rotation.Left.Normal * Time.Delta * CreationModeSpeed;
			Vector3 movement = movementX + movementY;
			CreationModeCurrentVelocity = CreationModeCurrentVelocity.LerpTo(movement, 0.1f);
		}

		private void ApplyCreationModeVelocity(AgentPlayer player)
		{
			player.CameraController.Camera.Transform.Position += CreationModeCurrentVelocity;
		}
	}
}
