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
		// TEMPORARY
		private bool isNotDoingAnimation = true;
		private bool isRolling;
		private bool isJumping;
		private bool isBackstepping;
		private bool midwayPoint;
		private bool canInterrupt;
		private bool setLastMove;
		private Vector3 lastMove;

		private float terminalVelocity = 200f;

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
		public CharacterController CharacterController { get; set; }

		[Property]
		public CharacterAgent agent { get; set; }

		[Property]
		public CitizenAnimationHelper AnimationHelper { get; set; }


		protected override void OnUpdate()
		{
			if ( isRolling || isJumping || isBackstepping )
			{
				if ( agent.CameraController != null )
				{
					Vector3 debug = CharacterController.Velocity.ProjectOnNormal( agent.CameraController.Camera.Transform.Rotation.Right.Normal );

					//Log.Info( "ye " + agent.CameraController.Camera.Transform.Rotation.Right.Normal );
					//Log.Info( "ne " + CharacterController.Velocity );

					float sign = Math.Sign( agent.CameraController.Camera.Transform.Rotation.Right.Normal.Dot( CharacterController.Velocity ) );
					float debug2 = debug.Length * sign;

					//Log.Info( debug2 );

					//Log.Info( agent.CameraController.ForwardAngles.yaw + " (yaw) + " + debug.y + " (debug) = " + (agent.CameraController.ForwardAngles.yaw + debug.y) );


					//Gizmo.Draw.Arrow( agent.CameraController.CameraPivot.Transform.Position, agent.CameraController.CameraPivot.Transform.Position + debug );
					Angles _targetAngles = agent.CameraController.ForwardAngles.WithYaw( agent.CameraController.ForwardAngles.yaw - debug2.Clamp( -1.0f, 1.0f ) );
					agent.CameraController.ForwardAngles = agent.CameraController.ForwardAngles.LerpTo( _targetAngles, 0.1f );
				}
			} else
			{
				if ( agent.CameraController != null )
				{
					Angles _targetAngles = agent.CameraController.ForwardAngles.WithYaw( agent.CameraController.ForwardAngles.yaw + Input.AnalogMove.y );
					agent.CameraController.ForwardAngles = agent.CameraController.ForwardAngles.LerpTo( _targetAngles, 0.1f );
				}
			}
		}

		protected override void OnFixedUpdate()
		{

			//Log.Info( Input.AnalogMove );


			if ( AnimationHelper != null )
			{
				//Log.Info( "hm " + canInterrupt + " " + isRolling );
				if ( Input.Pressed("sb_roll") && canInterrupt && isRolling )
				{
					//Log.Info( "interrupted!!!" );
					//agent.Transform.Rotation = Rotation.FromYaw( (agent.GetMoveVector()).EulerAngles.yaw );
					//AnimationHelper.Target.Set( "sb_interrupt", true );
					//agent.Transform.Rotation = Rotation.FromYaw( (agent.GetMoveVector()).EulerAngles.yaw );
					//AnimationHelper.Target.Set( "roll_forward", true );
					//agent.isRolling = true;
					//isRolling = true;
					//canInterrupt = false;
				}
			}

			//Log.Info( "neeee " + CharacterController.Velocity );

			if (!CharacterController.IsOnGround)
			{
				AnimationHelper.IsGrounded = CharacterController.IsOnGround;
				CharacterController.Accelerate( Vector3.Down * terminalVelocity );
				CharacterController.Acceleration = 4.9f;
				CharacterController.Move();
			}

			if ( isRolling && agent.lockedOn )
			{
				//Log.Info( Input.AnalogMove );
				//Log.Info(RoundToCardinal( Input.AnalogMove ));

				Vector3 _targetDirection = lastMove;

				if (setLastMove == false)
				{
					_targetDirection = Input.AnalogMove * (agent.currentLockOnAble.Transform.Position - agent.Transform.Position).EulerAngles;
					lastMove = _targetDirection;
					setLastMove = true;
				}
				

				Vector3 _currentVelocity = CharacterController.Velocity;
				Vector3 _targetVelocity = _targetDirection.Normal * 200.0f;
				float _targetSpeed = agent.IsRunActive() && !agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetLerpVelocity = agent.GetMoveVector().Normal * _targetSpeed;

				// project targetVelocity onto currentVelocity
				float _currentDotTargetVelocity = _targetLerpVelocity.Dot( _currentVelocity );
				float _currentDotCurrentVelocity = _currentVelocity.Dot( _currentVelocity );
				_targetLerpVelocity = MathF.Max( 0, (_currentDotTargetVelocity / _currentDotCurrentVelocity) ) * _currentVelocity;

				if ( midwayPoint == true )
				{
					//Log.Info( "curVel: " + _currentVelocity );
					//Log.Info( "tarLSpeed: " + _targetLerpSpeed );
					//Log.Info( "tarLVel: " + _targetLerpVelocity );
					_targetVelocity = _currentVelocity.LerpTo( _targetLerpVelocity, 0.1f, true );
					if ( float.IsNaN( _targetVelocity.x ) || float.IsNaN( _targetVelocity.y ) || float.IsNaN( _targetVelocity.z ) )
					{
						_targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
					}
					//Log.Info( "tarVel: " + _targetVelocity );
				}
				CharacterController.Velocity = _targetVelocity;
				CharacterController.Move();
			} else if (isRolling)
			{
				Vector3 _currentVelocity = CharacterController.Velocity;
				Vector3 _targetVelocity = agent.Transform.Rotation.Forward * 200.0f;
				float _targetSpeed = agent.IsRunActive() && !agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetLerpVelocity = agent.GetMoveVector().Normal * _targetSpeed;

				// project targetVelocity onto currentVelocity
				float _currentDotTargetVelocity = _targetLerpVelocity.Dot( _currentVelocity );
				float _currentDotCurrentVelocity = _currentVelocity.Dot( _currentVelocity );
				_targetLerpVelocity = MathF.Max( 0, (_currentDotTargetVelocity / _currentDotCurrentVelocity) ) * _currentVelocity;

				if ( midwayPoint == true )
				{
					//Log.Info( "curVel: " + _currentVelocity );
					//Log.Info( "tarLSpeed: " + _targetLerpSpeed );
					//Log.Info( "tarLVel: " + _targetLerpVelocity );
					_targetVelocity = _currentVelocity.LerpTo( _targetLerpVelocity, 0.05f, true );
					if ( float.IsNaN( _targetVelocity.x ) || float.IsNaN( _targetVelocity.y ) || float.IsNaN( _targetVelocity.z ) )
					{
						_targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
					}
					//Log.Info( "tarVel: " + _targetVelocity );
				}
				CharacterController.Velocity = _targetVelocity;
				CharacterController.Move();
			}
			else if ( isJumping )
			{
				Vector3 _currentVelocity = CharacterController.Velocity;
				Vector3 _targetVelocity = agent.Transform.Rotation.Forward * RunSpeed;
				float _targetSpeed = agent.IsRunActive() && !agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetLerpVelocity = agent.GetMoveVector().Normal * _targetSpeed;

				// project targetVelocity onto currentVelocity
				float _currentDotTargetVelocity = _targetLerpVelocity.Dot( _currentVelocity );
				float _currentDotCurrentVelocity = _currentVelocity.Dot( _currentVelocity );
				_targetLerpVelocity = MathF.Max( 0, (_currentDotTargetVelocity / _currentDotCurrentVelocity) ) * _currentVelocity;

				if ( midwayPoint == true )
				{
					//Log.Info( "curVel: " + _currentVelocity );
					//Log.Info( "tarLSpeed: " + _targetLerpSpeed );
					//Log.Info( "tarLVel: " + _targetLerpVelocity );
					_targetVelocity = _currentVelocity.LerpTo( _targetLerpVelocity, 0.05f, true );
					if ( float.IsNaN( _targetVelocity.x ) || float.IsNaN( _targetVelocity.y ) || float.IsNaN( _targetVelocity.z ) )
					{
						_targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
					}
					//Log.Info( "tarVel: " + _targetVelocity );
				}
				CharacterController.Velocity = _targetVelocity;
				CharacterController.Move();
			}
			else if ( isBackstepping )
			{
				Vector3 _currentVelocity = CharacterController.Velocity;
				Vector3 _targetVelocity = agent.Transform.Rotation.Backward * 100.0f;
				float _targetSpeed = agent.IsRunActive() && !agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetLerpVelocity = agent.Transform.Rotation.Backward * 0.00001f;

				// project targetVelocity onto currentVelocity
				float _currentDotTargetVelocity = _targetLerpVelocity.Dot( _currentVelocity );
				float _currentDotCurrentVelocity = _currentVelocity.Dot( _currentVelocity );
				_targetLerpVelocity = MathF.Max( 0, (_currentDotTargetVelocity / _currentDotCurrentVelocity) ) * _currentVelocity;

				if ( midwayPoint == true )
				{
					//Log.Info( "curVel: " + _currentVelocity );
					//Log.Info( "tarLSpeed: " + _targetLerpSpeed );
					//Log.Info( "tarLVel: " + _targetLerpVelocity );
					_targetVelocity = _currentVelocity.LerpTo( _targetLerpVelocity, 0.05f, true );
					if ( float.IsNaN( _targetVelocity.x ) || float.IsNaN( _targetVelocity.y ) || float.IsNaN( _targetVelocity.z ) )
					{
						_targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
					}
					//Log.Info( "tarVel: " + _targetVelocity );
				}
				CharacterController.Velocity = _targetVelocity;
				CharacterController.Move();
			}
			else
			{
				//Log.Info( "!!!!!" );
				float _targetSpeed = agent.IsRunActive() && !agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetVelocity = agent.GetMoveVector().Normal * _targetSpeed;

				CharacterController.Accelerate( _targetVelocity );
				CharacterController.Acceleration = 10.0f;
				CharacterController.ApplyFriction( 5.0f );
				CharacterController.Move();

				// TODO put this in CharacterAnimationController
				if ( AnimationHelper != null )
				{
					AnimationHelper.IsGrounded = CharacterController.IsOnGround;
					AnimationHelper.WithVelocity( CharacterController.Velocity );
					AnimationHelper.WithLook( _targetVelocity );
					if (isNotDoingAnimation && agent.isRolling && agent.lockedOn)
					{
						Vector3 cardinalDirection = RoundToCardinal( Input.AnalogMove );
						


						//Log.Info( "CARDINAL: " + cardinalDirection );


						if (cardinalDirection.y > 0)
						{
							agent.Transform.Rotation = Rotation.FromYaw( (Input.AnalogMove * (agent.currentLockOnAble.Transform.Position - agent.Transform.Position).EulerAngles).EulerAngles.yaw );
							AnimationHelper.Target.Set( "sb_roll2", true );
						} else if (cardinalDirection.y < 0)
						{
							agent.Transform.Rotation = Rotation.FromYaw( (Input.AnalogMove * (agent.currentLockOnAble.Transform.Position - agent.Transform.Position).EulerAngles).EulerAngles.yaw );
							AnimationHelper.Target.Set( "sb_roll2_mirror", true );
						} else
						{
							agent.Transform.Rotation = Rotation.FromYaw( (agent.GetMoveVector()).EulerAngles.yaw );
							AnimationHelper.Target.Set( "roll_forward", true );
						}
						
						isNotDoingAnimation = false;
					}
					else if ( isNotDoingAnimation && agent.isRolling == true )
					{
						agent.Transform.Rotation = Rotation.FromYaw( (agent.GetMoveVector()).EulerAngles.yaw );
						AnimationHelper.Target.Set( "roll_forward", true );
						isNotDoingAnimation = false;
					}
					else if ( isNotDoingAnimation && agent.isJumping == true )
					{
						//Log.Info( "YIPEEEEEEEEE" );
						agent.Transform.Rotation = Rotation.FromYaw( (agent.GetMoveVector()).EulerAngles.yaw );
						AnimationHelper.Target.Set( "sb_jump", true );
						isNotDoingAnimation = false;
					}
					else if ( isNotDoingAnimation && agent.isBackstepping == true )
					{
						AnimationHelper.Target.Set( "sb_backstep", true );
						isNotDoingAnimation = false;
					}
					//Log.Info( "can interrupt: " + canInterrupt );
				}
			}
		}

		private Vector3 RoundToCardinal( Vector3 vector3Input )
		{
			Vector2 input = new( vector3Input.x, vector3Input.y );

			// Normalize the input vector to get the direction
			Vector2 normalized = input.Normal;

			// Compute the angle in radians
			float angle = (float)Math.Atan2( normalized.y, normalized.x );

			// Convert radians to degrees
			float degrees = angle * (180.0f / (float)Math.PI);

			// Round to the nearest 90 degrees (0, 90, 180, 270)
			int roundedDegrees = (int)Math.Round( degrees / 90.0f ) * 90;

			// Map the rounded degree to a cardinal direction
			switch ( roundedDegrees % 360 )
			{
				case 0:
					return new Vector3( 1, 0, 0 );   // Right
				case 90:
				case -270:
					return new Vector3( 0, 1, 0 );   // Up
				case 180:
				case -180:
					return new Vector3( -1, 0, 0 );  // Left
				case 270:
				case -90:
					return new Vector3( 0, -1, 0 );   // Down
				default:
					throw new Exception( "Unexpected angle" );
			}
		}

		protected override void OnStart()
		{
			AnimationHelper.Target.OnGenericEvent = ( SceneModel.GenericEvent genericEvent ) =>
			{
				//Log.Info( genericEvent.String );
				switch ( genericEvent.String )
				{
					case "roll_start":
					case "roll2_start":
						//Log.Info( "setting to true" );
						isRolling = true;
						canInterrupt = false;
						break;
					case "jump_start":
						isJumping = true;
						break;
					case "backstep_start":
						isBackstepping = true;
						break;
					case "roll_end":
					case "roll2_end":
						//Log.Info( "setting to false" );
						//Log.Info( "You're toooo slow" );
						isRolling = false;
						agent.isRolling = false;
						midwayPoint = false;
						canInterrupt = false;
						isNotDoingAnimation = true;
						setLastMove = false;
						break;
					case "jump_end":
						isJumping = false;
						agent.isJumping = false;
						midwayPoint= false;
						isNotDoingAnimation = true;
						break;
					case "backstep_end":
						isBackstepping= false;
						agent.isBackstepping = false;
						midwayPoint = false;
						isNotDoingAnimation = true;
						break;
					case "backstep_midway":
						midwayPoint = true;
						break;
					case "jump_midway":
						midwayPoint = true;
						break;
					case "roll_midway":
					case "roll2_midway":
						//Log.Info( "mid" );
						midwayPoint = true;
						break;
					case "can_interrupt":
						//Log.Info( "Can Interrupt!; " + isRolling );
						canInterrupt = true; 
						break;
				}
			};
		}
	}
}
