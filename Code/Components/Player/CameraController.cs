using Sandbox;
using Sandbox.VR;
using System.Numerics;
using System;
using static Sandbox.VertexLayout;

namespace SoulsBox
{
	/// <summary>
	/// Controller for the player's camera.
	/// </summary>
	[Title( "Souls Box Camera Controller" )]
	[Category( "Souls Box" )]
	[Icon( "video_camera_front" )]
	public sealed class CameraController : Component
	{
		[Property]
		public GameObject Camera { get; set; }

		[Property]
		public GameObject CameraPivot { get; set; }

		[Property]
		public Vector3 CameraOffset { get; set; }

		public float HorizontalLerpSpeed { get; set; } = 40.0f;
		public float VerticalLerpSpeed { get; set; } = 40.0f;
		public float HorizontalThreshold { get; set; } = 0.05f;
		public float VerticalThreshold { get; set; } = 0.2f;

		private AgentPlayer Player { get; set; }
		private Angles ForwardAngles;
		private Transform InitialCameraTransform;

		private bool stickReleased = true;
		private float lastMouseX = 0f;
		private const float mouseThreshold = 300f;

		protected override void OnStart()
		{
			if ( Network.IsProxy ) return;

			CreateCameraPivot();
			InitializeCameraSettings();
			Player = AgentPlayer.Local;
		}

		protected override void OnUpdate()
		{
			if ( Network.IsProxy ) return;

			//Log.Info(Player.LockedOn);


			if (Player.CreationMode == true)
			{
				HandleFreeCamera();
				Gizmo.Draw.ScreenText( $"Free Cam Speed (Mouse Wheel): {MathF.Round(Player.CharacterMovementController.CreationModeSpeed)}", new Vector2( Screen.Width - 300, 25 ) );
				Gizmo.Draw.ScreenText( ".", new Vector2( Screen.Width /2, Screen.Height / 2 ) );
				return;
			}

			UpdateMoveVectors();

			if ( Player.LockedOn && Player.CurrentLockOnAble != null )
			{
				//Log.Info( "Handle lock on" );
				HandleLockOnCamera();
			}
			else
			{

				//Log.Info( "Handle normal" );
				HandleNormalCamera();
			}

			UpdatePlayerMoveDirection();
		}

		private void CreateCameraPivot()
		{
			var cameraPivotGameObject = Game.ActiveScene.CreateObject();
			cameraPivotGameObject.Name = "CameraPivot";
			var cameraPivotComponent = cameraPivotGameObject.Components.Create<CameraPivot>();
			cameraPivotComponent.Player = this.GameObject;
			CameraPivot = cameraPivotGameObject;
		}

		private void InitializeCameraSettings()
		{
			Camera = Scene.Camera.GameObject;
			Camera.SetParent( CameraPivot );
			Camera.Transform.Position = CameraPivot.Transform.Position + CameraOffset;
			Camera.Transform.Rotation = ((CameraPivot.Transform.Position - Camera.Transform.Position).Normal.EulerAngles).WithPitch( 0f );
			InitialCameraTransform = Camera.Transform.World;
		}

		private void HandleLockOnCamera()
		{
			Vector3 rotateAround = CameraPivot.Transform.Position;
			ForwardAngles = ForwardAngles.WithPitch( MathX.Clamp( ForwardAngles.pitch, -30.0f, 60.0f ) );

			InitialCameraTransform.Position = (rotateAround + CameraOffset).WithZ( rotateAround.z );
			//Log.Info( ForwardAngles );

			Camera.Transform.World = InitialCameraTransform.RotateAround( rotateAround, ForwardAngles );

			HandleTargetSwitching();

			AdjustCameraAngles();
			CorrectCameraPosition( rotateAround );
		}

		private void AdjustCameraAngles()
		{
			AdjustHorizontalAngle( Player.CurrentLockOnAblePosition.ToScreen().x );
			AdjustVerticalAngle( Player.CurrentLockOnAblePosition.ToScreen().y );
		}

		private void AdjustHorizontalAngle( float screenX )
		{
			if ( screenX < 0.5f - HorizontalThreshold )
			{
				AdjustYawTowards( 0.5f - HorizontalThreshold, screenX, HorizontalLerpSpeed );
			}
			else if ( screenX > 0.5f + HorizontalThreshold )
			{
				AdjustYawTowards( 0.5f + HorizontalThreshold, screenX, HorizontalLerpSpeed );
			}
		}

		private void AdjustVerticalAngle( float screenY )
		{
			if ( screenY < 0.5f - VerticalThreshold )
			{
				//Log.Info( "up: moving down" );
				AdjustPitchTowards( 0.5f - VerticalThreshold, screenY, VerticalLerpSpeed );
			}
			else if ( screenY > 0.5f + VerticalThreshold )
			{
				//Log.Info( "down: moving up" );
				AdjustPitchTowards( 0.5f + VerticalThreshold, screenY, VerticalLerpSpeed );
			}
		}

		private void AdjustYawTowards( float threshold, float screenX, float lerpSpeed )
		{
			float distanceToThreshold = MathF.Abs( threshold - screenX );

			ForwardAngles.yaw += Math.Sign( threshold - screenX ) * 0.1f * distanceToThreshold * lerpSpeed;

			ForwardAngles.yaw = (ForwardAngles.yaw + 360.0f) % 360.0f;
		}

		private void AdjustPitchTowards( float threshold, float screenY, float lerpSpeed )
		{
			float distanceToThreshold = MathF.Abs( threshold - screenY );
			ForwardAngles.pitch = (ForwardAngles.pitch - Math.Sign( threshold - screenY ) * 0.1f * distanceToThreshold * lerpSpeed).Clamp(-30f, 60f);
		}

		private void CorrectCameraPosition( Vector3 rotateAround )
		{
			var cameraTrace = Scene.Trace.Ray( rotateAround, Camera.Transform.World.Position ).Size( 5f ).WithoutTags( "player" ).WithoutTags("character").Run();
			Camera.Transform.Position = cameraTrace.EndPosition;
		}

		private void HandleNormalCamera()
		{
			ForwardAngles += Input.AnalogLook;

			AdjustYawForObstructions();
			AdjustYawBasedOnMovement();

			Vector3 rotateAround = CameraPivot.Transform.Position;
			ForwardAngles = ForwardAngles.WithPitch( MathX.Clamp( ForwardAngles.pitch, -30.0f, 60.0f ) );

			InitialCameraTransform.Position = (rotateAround + CameraOffset).WithZ( rotateAround.z );
			//Log.Info( ForwardAngles );

			Camera.Transform.World = InitialCameraTransform.RotateAround( rotateAround, ForwardAngles );

			CorrectCameraPosition( rotateAround );
		}

		// Automatically adjust camera yaw based on the player's movement direction
		private void AdjustYawBasedOnMovement()
		{
			// If the player is performing a dynamic action like rolling, jumping, or backstepping
			if ( Player.IsRolling || Player.IsJumping || Player.IsBackstepping )
			{
				// Calculate the velocity along the camera's right vector
				Vector3 projectedVelocity = Player.CharacterMovementController.CharacterController.Velocity.ProjectOnNormal( Camera.Transform.Rotation.Right.Normal );

				// Determine the sign based on the player's velocity direction
				float velocitySign = Math.Sign( Camera.Transform.Rotation.Right.Normal.Dot( Player.CharacterMovementController.CharacterController.Velocity ) );

				// Adjust yaw based on the velocity magnitude and direction
				float yawAdjustment = projectedVelocity.Length * velocitySign;
				Angles targetAngles = ForwardAngles.WithYaw( ForwardAngles.yaw - yawAdjustment.Clamp( -1.0f, 1.0f ) );

				// Smoothly interpolate the camera's yaw towards the target yaw
				ForwardAngles = ForwardAngles.LerpTo( targetAngles, 0.1f );
			}
			else
			{
				// Adjust yaw based on the player's movement input
				Angles targetAngles = ForwardAngles.WithYaw( ForwardAngles.yaw + Player.MoveVector.y );
				//Log.Info( "Yaw for movement" );
				ForwardAngles = ForwardAngles.LerpTo( targetAngles, 0.1f );
			}
		}

		// Automatically adjust camera yaw to allow the player to see around corners
		private void AdjustYawForObstructions()
		{
			const float pointDistanceAhead = 100.0f;

			// Calculate a point ahead of the player, offset vertically
			Vector3 pointAhead = Transform.Position.WithZ( Transform.Position.z + 65.0f ) + Transform.Rotation.Forward.Normal * pointDistanceAhead;

			// Perform a ray trace from the camera to the point ahead
			SceneTraceResult cameraToPointTrace = Scene.Trace.Ray( Camera.Transform.Position, pointAhead ).Size( 1f ).WithoutTags( "player" ).Run();

			// Perform a ray trace from the player to the point ahead
			SceneTraceResult playerToPointTrace = Scene.Trace.Ray( Transform.Position.WithZ( Transform.Position.z + 65.0f ), pointAhead ).Size( 1f ).WithoutTags( "player" ).Run();

			// Determine if the player is facing right based on the screen position of the point
			bool isFacingRight = pointAhead.ToScreen().x > 0.5f; // Note: Consider updating ToScreen method

			// Adjust yaw if the camera's ray is obstructed but the player's is not
			if ( cameraToPointTrace.Hit && !playerToPointTrace.Hit )
			{
				float yawAdjustment = isFacingRight ? -1f : 1f;
				Angles targetAngles = ForwardAngles.WithYaw( ForwardAngles.yaw + yawAdjustment );
				//Log.Info( "Yaw for obstructions" );
				ForwardAngles = ForwardAngles.LerpTo( targetAngles, 0.1f );
			}
		}

		private void UpdatePlayerMoveDirection()
		{
			if ( Player.MoveVectorRelativeToCamera.Length > 0 )
			{
				Player.LastMoveDirectionRotation = Rotation.FromYaw( Player.MoveVectorRelativeToCamera.EulerAngles.yaw );
			}
		}

		private void UpdateMoveVectors()
		{
			InputManager.UpdateAnalogMove( Player );
			Player.MoveVectorRelativeToCamera = GetMoveVectorRelativeToCamera();
		}

		private Vector3 GetMoveVectorRelativeToCamera()
		{
			if ( Camera == null ) return Vector3.Zero;
			return Player.MoveVector * Camera.Transform.Rotation;
		}

		private void HandleFreeCamera()
		{
			ForwardAngles += Input.AnalogLook;
			ForwardAngles = ForwardAngles.WithPitch( MathX.Clamp( ForwardAngles.pitch, -90.0f, 90.0f ) );
			Camera.Transform.Rotation = ForwardAngles;
		}

		private void HandleTargetSwitching()
		{
			//Log.Info( "HandleSwitching" );
			if ( Input.UsingController )
			{
				float yaw = Input.AnalogLook.yaw;

				if ( stickReleased && yaw > 0.5f )
				{
					Player.SwitchTarget( true );
					stickReleased = false;
				}
				else if ( stickReleased && yaw < -0.5f )
				{
					Player.SwitchTarget( false );
					stickReleased = false;
				}

				// Check if the stick is released
				if ( yaw > -0.1f && yaw < 0.1f )
				{
					stickReleased = true;
				}
			}
			else // Using mouse
			{
				float mouseX = Mouse.Position.x;

				float deltaX = mouseX - lastMouseX;

				if ( deltaX > mouseThreshold )
				{
					Player.SwitchTarget( false );
					lastMouseX = mouseX;
				}
				else if ( deltaX < -mouseThreshold )
				{
					Player.SwitchTarget( true );
					lastMouseX = mouseX;
				}
			}
		}
	}
}
