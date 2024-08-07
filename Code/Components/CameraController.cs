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
		public AgentPlayer player { get; set; }

		/// <summary>
		/// X, Y sets distance & offset of camera. Z sets height via the CameraPivot.
		/// </summary>
		[Property]
		public Vector3 CameraOffset { get; set; }

		public GameObject CameraPivotGameObject;
		public Angles ForwardAngles;
		private Transform InitialCameraTransform;

		public float horizontalLerpSpeed = 40.0f;  // Speed of horizontal lerp
		public float verticalLerpSpeed = 40.0f;    // Speed of vertical lerp
		public float horizontalThreshold = 0.05f; // Decimal portion of screen space to allow on either side of center
		public float verticalThreshold = 0.2f; // Decimal portion of screen space to allow on either side of center

		protected override void OnUpdate()
		{
			if (player.LockedOn && player.CurrentLockOnAble != null)
			{

				// HORIZONTAL LOCK ON
				float horizontalScreenPosition = player.CurrentLockOnAble.Transform.Position.ToScreen().x;
				if (horizontalScreenPosition < 0.5f - horizontalThreshold)
				{
					float currentHorizontalThreshold = 0.5f - horizontalThreshold;
					float distanceToHorizontalThreshold = MathF.Abs( (currentHorizontalThreshold - horizontalScreenPosition) );
					//Log.Info( distanceToHorizontalThreshold * horizontalLerpSpeed );
					ForwardAngles.yaw += 0.1f * distanceToHorizontalThreshold * horizontalLerpSpeed;
				} else if (horizontalScreenPosition > 0.5 + horizontalThreshold)
				{
					float currentHorizontalThreshold = 0.5f + horizontalThreshold;
					float distanceToHorizontalThreshold = MathF.Abs( (currentHorizontalThreshold - horizontalScreenPosition) );
					//Log.Info( distanceToHorizontalThreshold * horizontalLerpSpeed );
					ForwardAngles.yaw -= 0.1f * distanceToHorizontalThreshold * horizontalLerpSpeed;
				}

				// VERTICAL LOCK ON
				float verticalScreenPosition = player.CurrentLockOnAble.Transform.Position.ToScreen().y;
				if ( verticalScreenPosition < 0.5f - verticalThreshold )
				{
					float currentVerticalThreshold = 0.5f - verticalThreshold;
					float distanceToVerticalThreshold = MathF.Abs( (currentVerticalThreshold - verticalScreenPosition) );
					//Log.Info( distanceToVerticalThreshold );
					ForwardAngles.pitch -= 0.1f * distanceToVerticalThreshold * verticalLerpSpeed;
				}
				else if ( verticalScreenPosition > 0.5 + verticalThreshold )
				{
					float currentVerticalThreshold = 0.5f + verticalThreshold;
					float distanceToVerticalThreshold = MathF.Abs( (currentVerticalThreshold - verticalScreenPosition) );
					//Log.Info( distanceToVerticalThreshold );
					ForwardAngles.pitch += 0.1f * distanceToVerticalThreshold * verticalLerpSpeed;
				}

				// ForwardAngles = ForwardAngles.LerpTo(ForwardAngles.WithYaw(ForwardAngles.yaw + 1f), 0.5f);

				//Log.Info( verticalScreenPosition );


				// Standard Cam
				Vector3 _rotateAround = CameraPivot.Transform.Position;
				ForwardAngles = ForwardAngles.WithPitch( MathX.Clamp( ForwardAngles.pitch, -30.0f, 60.0f ) );
				InitialCameraTransform.Position = (_rotateAround + CameraOffset).WithZ( _rotateAround.z );
				Camera.Transform.World = InitialCameraTransform.RotateAround( _rotateAround, ForwardAngles );
				var cameraTrace = Scene.Trace.Ray( _rotateAround, Camera.Transform.World.Position ).Size( 5f ).WithoutTags( "player" ).Run();
				Camera.Transform.Position = cameraTrace.EndPosition;


				/*
				// Calculate the horizontal position (X and Y) between player and target
				Vector3 horizontalMidPoint = (Transform.Position + lockedOnPosition) / 2.0f;

				// Calculate the vertical position (Z) to keep both in frame
				float verticalDistance = MathF.Abs( Transform.Position.z - lockedOnPosition.z );
				float verticalMidPoint = MathX.Lerp( Transform.Position.z, lockedOnPosition.z, 0.5f ) + minVerticalDistance;
				verticalMidPoint = MathX.Clamp( verticalMidPoint, MathF.Min( Transform.Position.z, lockedOnPosition.z ), MathF.Max( Transform.Position.z, lockedOnPosition.z ) );

				// Desired target position of the camera
				targetPosition = new Vector3( horizontalMidPoint.x, horizontalMidPoint.y, verticalMidPoint + cameraHeight );

				// Smoothly move the camera horizontally (X and Y)
				Vector3 horizontalPosition = Vector3.Lerp( Camera.Transform.Position, targetPosition, horizontalLerpSpeed * Time.Delta );
				// Smoothly move the camera vertically (Z)
				horizontalPosition.z = MathX.Lerp( Camera.Transform.Position.z, targetPosition.z, verticalLerpSpeed * Time.Delta );

				// Set the camera's position
				Camera.Transform.Position = horizontalPosition;

				// Optional: Rotate the camera to look at the midpoint
				Camera.Transform.Rotation = (horizontalMidPoint - Camera.Transform.Position).Normal.EulerAngles;
				*/
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
			InitialCameraTransform = Camera.Transform.World;
		}
	}
}
