using Sandbox;
using Sandbox.Diagnostics;

namespace SoulsBox
{
	/// <summary>
	/// Interface between Souls Box character and an IRL human agent.
	/// </summary>
	[Title( "Souls Box Player Agent" )]
	[Category( "Souls Box" )]
	[Icon( "man" )]
	public sealed class AgentPlayer : CharacterAgent
	{

		[Property]
		public override CameraController CameraController { get; set; }

		[Property]
		public GameObject Camera { get; set; }

		private Rotation LastMoveDirectionRotation;
		private float inputHoldTime = 0f;
		private float timeSinceSprint = 0f;
		private const float sprintThreshold = 0.5f;
		private const float jumpThreshold = 0.1f;

		public override Vector3 GetMoveVector()
		{
			return Input.AnalogMove * Camera.Transform.Rotation;
		}

		public override bool IsGuardActive()
		{
			return Input.Down( "Guard" );
		}

		public override bool IsRunActive()
		{
			return isSprinting;
		}

		protected override void OnUpdate()
		{
			if (!CameraController.lockedOn)
			{
				CameraController.ForwardAngles += Input.AnalogLook;
				float _tempVarPointDistance = 100.0f;
				Vector3 _tempPointVector = new Vector3( Transform.Position.WithZ( Transform.Position.z + 65.0f ) + Transform.Rotation.Forward.Normal * _tempVarPointDistance );
				SceneTraceResult camToPointTraceResult = Scene.Trace.Ray( Camera.Transform.Position, _tempPointVector ).Size( 1f ).WithoutTags( "player" ).Run();
				SceneTraceResult playerToPointTraceResult = Scene.Trace.Ray( Transform.Position.WithZ( Transform.Position.z + 65.0f ), _tempPointVector ).Size( 1f ).WithoutTags( "player" ).Run();

				bool playerFacingRight = _tempPointVector.ToScreen().x > 0.5f; // TODO ToScreen is obsolete apparently

				if ( camToPointTraceResult.Hit && !playerToPointTraceResult.Hit )
				{
					float incrementAmount = playerFacingRight ? -1f : 1f;
					CameraController.ForwardAngles.yaw += incrementAmount;
				}
			}
			
		}

		protected override void OnFixedUpdate()
		{
			//Log.Info( timeSinceSprint );

			if ( isRolling || isJumping )
			{
				inputHoldTime = 0f; // Reset the input hold time when rolling or jumping
				return;
			}

			if ( !isRolling && !isJumping && !(CameraController.lockedOn) )
			{
				if ( GetMoveVector().Length > 0 ) LastMoveDirectionRotation = Rotation.FromYaw( (GetMoveVector()).EulerAngles.yaw );
				Transform.Rotation = Rotation.Lerp( Transform.Rotation, LastMoveDirectionRotation, 0.1f );
			} else if (!isRolling && !isJumping)
			{
				Vector3 targetToPlayerDisplacement = (CameraController.lockedOnPosition -Transform.Position);
				Rotation faceDirection = Rotation.FromYaw( targetToPlayerDisplacement.Normal.EulerAngles.yaw );
				Transform.Rotation = Rotation.Lerp( Transform.Rotation, faceDirection, 0.5f );
			}

			if ( Input.Down( "sb_sprint" ) )
			{
				inputHoldTime += Time.Delta;

				if ( inputHoldTime >= sprintThreshold )
				{
					timeSinceSprint = 0;
					isSprinting = true;
				}
			}
			else
			{
				if ( inputHoldTime > 0 && inputHoldTime < sprintThreshold )
				{
					if ( GetMoveVector().Length > 0 )
					{
						// Rolling
						isRolling = true;
					}
					else
					{
						// Backstepping
						//Log.Info( "Backstep" );
						isBackstepping = true;
					}
				}

				isSprinting = false;
				inputHoldTime = 0;
				timeSinceSprint += Time.Delta;
			}
			if ( Input.Pressed( "sb_jump" ) && timeSinceSprint < jumpThreshold )
			{
				isJumping = true;
			}
		}
	}
}

