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

		[Property]
		public AgentPlayer Player { get; set; }

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
			if (Player.LockedOn && Player.CurrentLockOnAble != null)
			{

				// HORIZONTAL LOCK ON
				float horizontalScreenPosition = Player.CurrentLockOnAble.Transform.Position.ToScreen().x;
				if (horizontalScreenPosition < 0.5f - HorizontalThreshold)
				{
					float currentHorizontalThreshold = 0.5f - HorizontalThreshold;
					float distanceToHorizontalThreshold = MathF.Abs( (currentHorizontalThreshold - horizontalScreenPosition) );
					//Log.Info( distanceToHorizontalThreshold * horizontalLerpSpeed );
					ForwardAngles.yaw += 0.1f * distanceToHorizontalThreshold * HorizontalLerpSpeed;
				} else if (horizontalScreenPosition > 0.5 + HorizontalThreshold)
				{
					float currentHorizontalThreshold = 0.5f + HorizontalThreshold;
					float distanceToHorizontalThreshold = MathF.Abs( (currentHorizontalThreshold - horizontalScreenPosition) );
					//Log.Info( distanceToHorizontalThreshold * horizontalLerpSpeed );
					ForwardAngles.yaw -= 0.1f * distanceToHorizontalThreshold * HorizontalLerpSpeed;
				}

				// VERTICAL LOCK ON
				float verticalScreenPosition = Player.CurrentLockOnAble.Transform.Position.ToScreen().y;
				if ( verticalScreenPosition < 0.5f - VerticalThreshold )
				{
					float currentVerticalThreshold = 0.5f - VerticalThreshold;
					float distanceToVerticalThreshold = MathF.Abs( (currentVerticalThreshold - verticalScreenPosition) );
					//Log.Info( distanceToVerticalThreshold );
					ForwardAngles.pitch -= 0.1f * distanceToVerticalThreshold * VerticalLerpSpeed;
				}
				else if ( verticalScreenPosition > 0.5 + VerticalThreshold )
				{
					float currentVerticalThreshold = 0.5f + VerticalThreshold;
					float distanceToVerticalThreshold = MathF.Abs( (currentVerticalThreshold - verticalScreenPosition) );
					//Log.Info( distanceToVerticalThreshold );
					ForwardAngles.pitch += 0.1f * distanceToVerticalThreshold * VerticalLerpSpeed;
				}


				// Standard Cam
				Vector3 _rotateAround = CameraPivot.Transform.Position;
				ForwardAngles = ForwardAngles.WithPitch( MathX.Clamp( ForwardAngles.pitch, -30.0f, 60.0f ) );
				InitialCameraTransform.Position = (_rotateAround + CameraOffset).WithZ( _rotateAround.z );
				Camera.Transform.World = InitialCameraTransform.RotateAround( _rotateAround, ForwardAngles );
				var cameraTrace = Scene.Trace.Ray( _rotateAround, Camera.Transform.World.Position ).Size( 5f ).WithoutTags( "player" ).Run();
				Camera.Transform.Position = cameraTrace.EndPosition;
			}
			else
			{
				Vector3 _rotateAround = CameraPivot.Transform.Position;
				ForwardAngles = ForwardAngles.WithPitch( MathX.Clamp( ForwardAngles.pitch, -30.0f, 60.0f ) );
				InitialCameraTransform.Position = (_rotateAround + CameraOffset).WithZ( _rotateAround.z );
				Camera.Transform.World = InitialCameraTransform.RotateAround( _rotateAround, ForwardAngles );
				var cameraTrace = Scene.Trace.Ray( _rotateAround, Camera.Transform.World.Position ).Size( 5f ).WithoutTags( "player" ).Run();
				Camera.Transform.Position = cameraTrace.EndPosition;
			}
			
		}

		protected override void OnStart()
		{
			Camera = Scene.Camera.GameObject;
			//Player = Scene.Components.Get<AgentPlayer>();
			//CameraPivot = Scene.Components.Get<CameraPivot>().GameObject;
			Camera.SetParent( CameraPivot );
			InitialCameraTransform = Camera.Transform.World;
		}
	}
}
