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

		/// <summary>
		/// X, Y sets distance & offset of camera. Z sets height via the CameraPivot.
		/// </summary>
		[Property]
		public Vector3 CameraOffset { get; set; }

		public bool lockedOn { get; set; }
		public Vector3 lockedOnPosition { get; set; }

		public GameObject CameraPivotGameObject;
		public Angles ForwardAngles;
		private Transform InitialCameraTransform;

		public float horizontalLerpSpeed = 5.0f;  // Speed of horizontal lerp
		public float verticalLerpSpeed = 5.0f;    // Speed of vertical lerp
		public float horizontalThreshold = 0.05f; // Decimal portion of screen space to allow on either side of center
		public float verticalThreshold = 0.8f; // Decimal portion of screen space to allow on either side of center

		protected override void OnUpdate()
		{
			if (lockedOn)
			{

				Vector3 targetPosition; // Position to lerp to
				Rotation targetRotation; // Rotation to lerp to

				GameTransform playerTransform = Transform;
				GameTransform cameraTransform = Camera.Transform;

				Gizmo.Draw.SolidSphere( lockedOnPosition, 1f );

				// HORIZONTAL LOCK ON
				float horizontalScreenPosition = lockedOnPosition.ToScreen().x;
				if (horizontalScreenPosition < 0.5 - horizontalThreshold)
				{
					ForwardAngles.yaw += 1f;
				} else if (horizontalScreenPosition > 0.5 + horizontalThreshold)
				{
					ForwardAngles.yaw -= 1f;
				}

				Log.Info( horizontalScreenPosition );


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
			lockedOnPosition = Transform.Position + new Vector3( 0, 50, 70 );
			lockedOn = true;
		}
	}
}
