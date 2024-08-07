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
		public bool IsRolling;
		public bool IsJumping;
		public bool IsBackstepping;
		public bool IsAttacking;
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
		

		protected override void OnUpdate()
		{
			if ( IsRolling || IsJumping || IsBackstepping )
			{
				if ( Agent.CameraController != null )
				{
					Vector3 debug = CharacterController.Velocity.ProjectOnNormal( Agent.CameraController.Camera.Transform.Rotation.Right.Normal );
					float sign = Math.Sign( Agent.CameraController.Camera.Transform.Rotation.Right.Normal.Dot( CharacterController.Velocity ) );
					float debug2 = debug.Length * sign;
					Angles _targetAngles = Agent.CameraController.ForwardAngles.WithYaw( Agent.CameraController.ForwardAngles.yaw - debug2.Clamp( -1.0f, 1.0f ) );
					Agent.CameraController.ForwardAngles = Agent.CameraController.ForwardAngles.LerpTo( _targetAngles, 0.1f );
				}
			} else
			{
				if ( Agent.CameraController != null )
				{
					Angles _targetAngles = Agent.CameraController.ForwardAngles.WithYaw( Agent.CameraController.ForwardAngles.yaw + Input.AnalogMove.y );
					Agent.CameraController.ForwardAngles = Agent.CameraController.ForwardAngles.LerpTo( _targetAngles, 0.1f );
				}
			}
		}

		protected override void OnFixedUpdate()
		{
			if (!CharacterController.IsOnGround)
			{
				CharacterAnimationController.AnimationHelper.IsGrounded = CharacterController.IsOnGround;
				CharacterController.Accelerate( Vector3.Down * TerminalVelocity );
				CharacterController.Acceleration = 4.9f;
				CharacterController.Move();
			}

			if ( IsRolling && Agent.LockedOn )
			{
				Vector3 _targetDirection = LastMove;

				if (SetLastMove == false)
				{
					_targetDirection = Input.AnalogMove * (Agent.CurrentLockOnAble.Transform.Position - Agent.Transform.Position).EulerAngles;
					LastMove = _targetDirection;
					SetLastMove = true;
				}
				

				Vector3 _currentVelocity = CharacterController.Velocity;
				Vector3 _targetVelocity = _targetDirection.Normal * 200.0f;
				float _targetSpeed = Agent.IsRunActive() && !Agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetLerpVelocity = Agent.GetMoveVector().Normal * _targetSpeed;

				float _currentDotTargetVelocity = _targetLerpVelocity.Dot( _currentVelocity );
				float _currentDotCurrentVelocity = _currentVelocity.Dot( _currentVelocity );
				_targetLerpVelocity = MathF.Max( 0, (_currentDotTargetVelocity / _currentDotCurrentVelocity) ) * _currentVelocity;

				if ( CharacterAnimationController.IsPastMidwayPoint == true )
				{
					_targetVelocity = _currentVelocity.LerpTo( _targetLerpVelocity, 0.1f, true );
					if ( float.IsNaN( _targetVelocity.x ) || float.IsNaN( _targetVelocity.y ) || float.IsNaN( _targetVelocity.z ) )
					{
						_targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
					}
				}
				CharacterController.Velocity = _targetVelocity;
				CharacterController.Move();
			} else if (IsRolling)
			{
				Vector3 _currentVelocity = CharacterController.Velocity;
				Vector3 _targetVelocity = Agent.Transform.Rotation.Forward * 200.0f;
				float _targetSpeed = Agent.IsRunActive() && !Agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetLerpVelocity = Agent.GetMoveVector().Normal * _targetSpeed;

				float _currentDotTargetVelocity = _targetLerpVelocity.Dot( _currentVelocity );
				float _currentDotCurrentVelocity = _currentVelocity.Dot( _currentVelocity );
				_targetLerpVelocity = MathF.Max( 0, (_currentDotTargetVelocity / _currentDotCurrentVelocity) ) * _currentVelocity;

				if ( CharacterAnimationController.IsPastMidwayPoint == true )
				{

					_targetVelocity = _currentVelocity.LerpTo( _targetLerpVelocity, 0.05f, true );
					if ( float.IsNaN( _targetVelocity.x ) || float.IsNaN( _targetVelocity.y ) || float.IsNaN( _targetVelocity.z ) )
					{
						_targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
					}
				}
				CharacterController.Velocity = _targetVelocity;
				CharacterController.Move();
			}
			else if ( IsJumping )
			{
				Vector3 _currentVelocity = CharacterController.Velocity;
				Vector3 _targetVelocity = Agent.Transform.Rotation.Forward * RunSpeed;
				float _targetSpeed = Agent.IsRunActive() && !Agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetLerpVelocity = Agent.GetMoveVector().Normal * _targetSpeed;

				float _currentDotTargetVelocity = _targetLerpVelocity.Dot( _currentVelocity );
				float _currentDotCurrentVelocity = _currentVelocity.Dot( _currentVelocity );
				_targetLerpVelocity = MathF.Max( 0, (_currentDotTargetVelocity / _currentDotCurrentVelocity) ) * _currentVelocity;

				if ( CharacterAnimationController.IsPastMidwayPoint == true )
				{
					_targetVelocity = _currentVelocity.LerpTo( _targetLerpVelocity, 0.05f, true );
					if ( float.IsNaN( _targetVelocity.x ) || float.IsNaN( _targetVelocity.y ) || float.IsNaN( _targetVelocity.z ) )
					{
						_targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
					}
				}
				CharacterController.Velocity = _targetVelocity;
				CharacterController.Move();
			}
			else if ( IsBackstepping )
			{
				Vector3 _currentVelocity = CharacterController.Velocity;
				Vector3 _targetVelocity = Agent.Transform.Rotation.Backward * 100.0f;
				float _targetSpeed = Agent.IsRunActive() && !Agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetLerpVelocity = Agent.Transform.Rotation.Backward * 0.00001f;

				float _currentDotTargetVelocity = _targetLerpVelocity.Dot( _currentVelocity );
				float _currentDotCurrentVelocity = _currentVelocity.Dot( _currentVelocity );
				_targetLerpVelocity = MathF.Max( 0, (_currentDotTargetVelocity / _currentDotCurrentVelocity) ) * _currentVelocity;

				if ( CharacterAnimationController.IsPastMidwayPoint == true )
				{
					_targetVelocity = _currentVelocity.LerpTo( _targetLerpVelocity, 0.05f, true );
					if ( float.IsNaN( _targetVelocity.x ) || float.IsNaN( _targetVelocity.y ) || float.IsNaN( _targetVelocity.z ) )
					{
						_targetVelocity = new Vector3( 0.000001f, 0.00001f, -0.00001f );
					}
				}
				CharacterController.Velocity = _targetVelocity;
				CharacterController.Move();
			}
			else if (IsAttacking)
			{
				CharacterController.MoveTo( Transform.Position + CharacterAnimationController.AnimationHelper.Target.RootMotion.Position.Length * Transform.Rotation.Forward.Normal * 7.5f, true);
			}
			else
			{
				float _targetSpeed = Agent.IsRunActive() && !Agent.IsGuardActive() ? RunSpeed : WalkSpeed;
				Vector3 _targetVelocity = Agent.GetMoveVector().Normal * _targetSpeed;

				CharacterController.Accelerate( _targetVelocity );
				CharacterController.Acceleration = 10.0f;
				CharacterController.ApplyFriction( 5.0f );
				CharacterController.Move();
			}
		}
	}
}
