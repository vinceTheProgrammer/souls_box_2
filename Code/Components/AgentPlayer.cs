using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;

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

		private Rotation LastMoveDirectionRotation;

		public override Vector3 GetMoveVector()
		{
			return Input.AnalogMove * CameraController.Camera.Transform.Rotation;
		}

		public override bool IsGuardActive()
		{
			return Input.Down( "Guard" );
		}

		public override bool IsRunActive()
		{
			return IsSprinting;
		}

		protected override void OnUpdate()
		{
			if (!LockedOn)
			{
				CameraController.ForwardAngles += Input.AnalogLook;
				float _tempVarPointDistance = 100.0f;
				Vector3 _tempPointVector = new Vector3( Transform.Position.WithZ( Transform.Position.z + 65.0f ) + Transform.Rotation.Forward.Normal * _tempVarPointDistance );
				SceneTraceResult camToPointTraceResult = Scene.Trace.Ray( CameraController.Camera.Transform.Position, _tempPointVector ).Size( 1f ).WithoutTags( "player" ).Run();
				SceneTraceResult playerToPointTraceResult = Scene.Trace.Ray( Transform.Position.WithZ( Transform.Position.z + 65.0f ), _tempPointVector ).Size( 1f ).WithoutTags( "player" ).Run();

				bool playerFacingRight = _tempPointVector.ToScreen().x > 0.5f; // TODO ToScreen is obsolete apparently

				if ( camToPointTraceResult.Hit && !playerToPointTraceResult.Hit )
				{
					float incrementAmount = playerFacingRight ? -1f : 1f;
					CameraController.ForwardAngles.yaw += incrementAmount;
				}
			}
			OctreeManager.Instance.Draw();
		}

		public void toggleLockOn()
		{
			bool previousState = LockedOn;
			LockedOn = !LockedOn;
			if ( previousState == false ) 
			{
				CurrentLockOnAble = GetClosestLockOnAbleInView();
			}
		}

		private bool IsWithinView(GameTransform gameTransform)
		{
			float leftBound = 0f;
			float rightBound = 1f;
			float topBound = 0f;
			float bottomBound = 1f;
			Vector3 screenCoords = gameTransform.Position.ToScreen();
			return screenCoords.x >= leftBound && screenCoords.x <= rightBound && screenCoords.y >= topBound && screenCoords.y <= bottomBound;
		}


		// move somewhere else later lol
		

		protected override void OnFixedUpdate()
		{
			UpdateLockOnAbles();

			if ( !IsRolling && !IsJumping && !(LockedOn && !IsSprinting) )
			{
				if ( GetMoveVector().Length > 0 ) LastMoveDirectionRotation = Rotation.FromYaw( (GetMoveVector()).EulerAngles.yaw );
				Transform.Rotation = Rotation.Lerp( Transform.Rotation, LastMoveDirectionRotation, 0.1f );
			} else if (!IsRolling && !IsJumping)
			{
				Vector3 targetToPlayerDisplacement = (CurrentLockOnAble.Transform.Position - Transform.Position);
				Rotation faceDirection = Rotation.FromYaw( targetToPlayerDisplacement.Normal.EulerAngles.yaw );
				Transform.Rotation = Rotation.Lerp( Transform.Rotation, faceDirection, 0.5f );
			}
			
		}

		protected override void OnStart()
		{

			OctreeManager.Instance.DestroyAndReInit();
		}

		public void UpdateLockOnAbles()
		{
			LockOnAbles.Clear();
			OctreeManager.Instance.Clear();

			IEnumerable<LockOnAble> allLockOnAbles = Scene.GetAllComponents<LockOnAble>();
			foreach ( var lockOnAble in allLockOnAbles )
			{
				OctreeManager.Instance.Insert( lockOnAble );
			}

			var lockOnAblesInRange = OctreeManager.Instance.QueryRange( Transform.Position, LockOnRadius );
			foreach ( var lockOnAble in lockOnAblesInRange )
			{
				if ( lockOnAble.ParentIsAlive() )
				{
					LockOnAbles.Add( lockOnAble );
				}
			}
		}

		private LockOnAble GetClosestLockOnAble()
		{
			LockOnAble closest = null;
			float closestDistance = float.MaxValue;

			foreach ( var lockOnAble in LockOnAbles )
			{
				float distance = Vector3.DistanceBetween( Transform.Position, lockOnAble.Transform.Position );
				if ( distance < closestDistance )
				{
					closest = lockOnAble;
					closestDistance = distance;
				}
			}

			return closest;
		}

		private LockOnAble GetClosestLockOnAbleInView()
		{
			LockOnAble closest = null;
			float closestDistance = float.MaxValue;

			foreach ( var lockOnAble in LockOnAbles )
			{
				float distance = Vector3.DistanceBetween( Transform.Position, lockOnAble.Transform.Position );
				if ( distance < closestDistance && IsWithinView( lockOnAble.Transform ) )
				{
					closest = lockOnAble;
					closestDistance = distance;
				}
			}

			return closest;
		}

		public void SwitchTarget( bool isLeft )
		{
			if ( CurrentLockOnAble == null ) return;

			LockOnAble bestTarget = null;
			float bestAngle = float.MaxValue;

			foreach ( var lockOnAble in LockOnAbles )
			{
				if ( lockOnAble == CurrentLockOnAble ) continue;

				Vector3 toTarget = lockOnAble.Transform.Position - Transform.Position;
				Vector3 toCurrentTarget = CurrentLockOnAble.Transform.Position - Transform.Position;

				float angle = toCurrentTarget.SignedAngle( toTarget );
				if ( isLeft && angle < 0 && angle > -bestAngle )
				{
					bestAngle = -angle;
					bestTarget = lockOnAble;
				}
				else if ( !isLeft && angle > 0 && angle < bestAngle )
				{
					bestAngle = angle;
					bestTarget = lockOnAble;
				}
			}

			if ( bestTarget != null )
			{
				CurrentLockOnAble = bestTarget;
			}
		}
	}
}

