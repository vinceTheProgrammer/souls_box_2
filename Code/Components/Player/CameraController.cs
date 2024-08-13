using Sandbox;
using Sandbox.VR;
using static Sandbox.PhysicsContact;
using System.Numerics;
using System;

namespace SoulsBox
{
	/// <summary>
	/// Controller for the player's camera
	/// </summary>
	[Title( "Souls Box Camera Controller" )]
	[Category( "Souls Box" )]
	[Icon( "video_camera_front" )]
	public sealed class CameraController : Component
	{

		[Property]
		public GameObject Camera {  get; set; }

		[Property]
		public GameObject CameraPivot { get; set; }

		private AgentPlayer Player { get; set; }

		/// <summary>
		/// X, Y sets distance & offset of camera. Z sets height via the CameraPivot.
		/// </summary>
		[Property]
		public Vector3 CameraOffset { get; set; }

		public GameObject CameraPivotGameObject;
		public Angles ForwardAngles;
		private Transform InitialCameraTransform;

		public float HorizontalLerpSpeed = 40.0f;  // Speed of horizontal lerp
		public float VerticalLerpSpeed = 40.0f;    // Speed of vertical lerp
		public float HorizontalThreshold = 0.05f; // Decimal portion of screen space to allow on either side of center
		public float VerticalThreshold = 0.2f; // Decimal portion of screen space to allow on either side of center

		protected override void OnUpdate()
		{
			if ( Network.IsProxy ) return;
			UpdateMoveVectors();

			if (Player.LockedOn && Player.CurrentLockOnAble != null)
			{

				// Standard Cam
				Vector3 _rotateAround = CameraPivot.Transform.Position;
				ForwardAngles = ForwardAngles.WithPitch( MathX.Clamp( ForwardAngles.pitch, -30.0f, 60.0f ) );
				InitialCameraTransform.Position = (_rotateAround + CameraOffset).WithZ( _rotateAround.z );
				Camera.Transform.World = InitialCameraTransform.RotateAround( _rotateAround, ForwardAngles );

				// HORIZONTAL LOCK ON
				float horizontalScreenPosition = Player.CurrentLockOnAblePosition.ToScreen().x;
				if (horizontalScreenPosition < 0.5f - HorizontalThreshold)
				{
					float currentHorizontalThreshold = 0.5f - HorizontalThreshold;
					float distanceToHorizontalThreshold = MathF.Abs( (currentHorizontalThreshold - horizontalScreenPosition) );
					ForwardAngles.yaw += 0.1f * distanceToHorizontalThreshold * HorizontalLerpSpeed;
				} else if (horizontalScreenPosition > 0.5 + HorizontalThreshold)
				{
					float currentHorizontalThreshold = 0.5f + HorizontalThreshold;
					float distanceToHorizontalThreshold = MathF.Abs( (currentHorizontalThreshold - horizontalScreenPosition) );
					ForwardAngles.yaw -= 0.1f * distanceToHorizontalThreshold * HorizontalLerpSpeed;
				}

				// VERTICAL LOCK ON
				float verticalScreenPosition = Player.CurrentLockOnAblePosition.ToScreen().y;
				if ( verticalScreenPosition < 0.5f - VerticalThreshold )
				{
					float currentVerticalThreshold = 0.5f - VerticalThreshold;
					float distanceToVerticalThreshold = MathF.Abs( (currentVerticalThreshold - verticalScreenPosition) );
					ForwardAngles.pitch -= 0.1f * distanceToVerticalThreshold * VerticalLerpSpeed;
				}
				else if ( verticalScreenPosition > 0.5 + VerticalThreshold )
				{
					float currentVerticalThreshold = 0.5f + VerticalThreshold;
					float distanceToVerticalThreshold = MathF.Abs( (currentVerticalThreshold - verticalScreenPosition) );
					ForwardAngles.pitch += 0.1f * distanceToVerticalThreshold * VerticalLerpSpeed;
				}

				var cameraTrace = Scene.Trace.Ray( _rotateAround, Camera.Transform.World.Position ).Size( 5f ).WithoutTags( "player" ).Run();
				Camera.Transform.Position = cameraTrace.EndPosition;
			}
			else
			{
				//ForwardAngles += Input.AnalogLook;
				float _tempVarPointDistance = 100.0f;
				Vector3 _tempPointVector = new Vector3( Transform.Position.WithZ( Transform.Position.z + 65.0f ) + Transform.Rotation.Forward.Normal * _tempVarPointDistance );
				SceneTraceResult camToPointTraceResult = Scene.Trace.Ray( Camera.Transform.Position, _tempPointVector ).Size( 1f ).WithoutTags( "player" ).Run();
				SceneTraceResult playerToPointTraceResult = Scene.Trace.Ray( Transform.Position.WithZ( Transform.Position.z + 65.0f ), _tempPointVector ).Size( 1f ).WithoutTags( "player" ).Run();

				bool playerFacingRight = _tempPointVector.ToScreen().x > 0.5f; // TODO ToScreen is obsolete apparently

				if ( camToPointTraceResult.Hit && !playerToPointTraceResult.Hit )
				{
					float incrementAmount = playerFacingRight ? -1f : 1f;
					Log.Info( "Yaw gotta be kidding me." );
					ForwardAngles.yaw += incrementAmount;
				}

				Vector3 _rotateAround = CameraPivot.Transform.Position;
				ForwardAngles = ForwardAngles.WithPitch( MathX.Clamp( ForwardAngles.pitch, -30.0f, 60.0f ) );
				InitialCameraTransform.Position = (_rotateAround + CameraOffset).WithZ( _rotateAround.z );
				Camera.Transform.World = InitialCameraTransform.RotateAround( _rotateAround, ForwardAngles );
				var cameraTrace = Scene.Trace.Ray( _rotateAround, Camera.Transform.World.Position ).Size( 5f ).WithoutTags( "player" ).Run();
				Camera.Transform.Position = cameraTrace.EndPosition;
			}

			if ( Player.MoveVectorRelativeToCamera.Length > 0 ) Player.LastMoveDirectionRotation = Rotation.FromYaw( (Player.MoveVectorRelativeToCamera).EulerAngles.yaw );

			if ( Player is AgentPlayer player )
			{
				if ( player.IsRolling || player.IsJumping || player.IsBackstepping )
				{
					if ( player.CharacterMovementController != null )
					{
						Log.Info( "I am your father." );
						Vector3 debug = player.CharacterMovementController.CharacterController.Velocity.ProjectOnNormal( Camera.Transform.Rotation.Right.Normal );
						float sign = Math.Sign( Camera.Transform.Rotation.Right.Normal.Dot( player.CharacterMovementController.CharacterController.Velocity ) );
						float debug2 = debug.Length * sign;
						Angles _targetAngles = ForwardAngles.WithYaw(ForwardAngles.yaw - debug2.Clamp( -1.0f, 1.0f ) );
						ForwardAngles = ForwardAngles.LerpTo( _targetAngles, 0.1f );
					}
				}
				else
				{
					Log.Info( "I am here." );
					Angles _targetAngles = ForwardAngles.WithYaw( ForwardAngles.yaw + player.MoveVector.y );
					ForwardAngles = ForwardAngles.LerpTo( _targetAngles, 0.1f );
				}
			}
		}

		protected override void OnStart()
		{
			if ( Network.IsProxy ) return;
			var cameraPivotGameObject = Game.ActiveScene.CreateObject();
			cameraPivotGameObject.Name = "CameraPivot";
			var cameraPivotComponent = cameraPivotGameObject.Components.Create<CameraPivot>();
			cameraPivotComponent.Player = this.GameObject;
			CameraPivot = cameraPivotGameObject;
			Camera = Scene.Camera.GameObject;
			Camera.SetParent( CameraPivot );
			Camera.Transform.Position = CameraPivot.Transform.Position + CameraOffset;
			Camera.Transform.Rotation = ((CameraPivot.Transform.Position - Camera.Transform.Position).Normal.EulerAngles).WithPitch(0f);
			InitialCameraTransform = Camera.Transform.World;
			Player = AgentPlayer.Local;
			//LogSceneHierarchy();
		}

		private void LogSceneHierarchy()
		{
			IEnumerable<GameObject> objects = Game.ActiveScene.GetAllObjects( true );
			foreach ( GameObject obj in objects )
			{
				Log.Info( obj.Name + ": " + !obj.Network.IsProxy );
			}
		}

		private void UpdateMoveVectors()
		{
			InputManager.UpdateAnalogMove(Player);
			Player.MoveVectorRelativeToCamera = GetMoveVectorRelativeToCamera();
		}

		private Vector3 GetMoveVectorRelativeToCamera()
		{
			if ( Camera == null ) return Vector3.Zero;
			return Player.MoveVector * Camera.Transform.Rotation;
		}
	}
}
