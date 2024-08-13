using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;

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
		[Sync]
		public Rotation LastMoveDirectionRotation { get; set; }

		[Sync]
		public Vector3 MoveVectorRelativeToCamera {  get; set; }

		[Sync]
		public bool LockedOn { get; set; }

		[Sync]
		public Vector3 CurrentLockOnAblePosition { get; set; }

		[Property]
		public PlayerStats PlayerStats { get; set; }

		public LockOnAble CurrentLockOnAble { get; set; }

		public HashSet<LockOnAble> LockOnAbles = new HashSet<LockOnAble>();
		public float LockOnRadius { get; set; } = 1000f;

		public bool CanRespawn { get; set; }

		public Bonfire lastRestedBonfire {  get; set; }

		public bool IsRespawning { get; set; }

		public static AgentPlayer Local
		{
			get
			{
				if ( !_local.IsValid())
				{
					_local = Game.ActiveScene.GetAllComponents<AgentPlayer>().FirstOrDefault( x => x.Network.IsOwner );
				}
				return _local;
			}
		}
		private static AgentPlayer _local = null;

		protected override void OnUpdate()
		{
			OctreeManager.Instance.Draw();
		}

		public void ToggleLockOn()
		{
			bool previousState = LockedOn;
			if ( previousState == false ) 
			{
				CurrentLockOnAble = GetClosestLockOnAbleInView();
				if ( CurrentLockOnAble != null )
				{
					LockedOn = !LockedOn;
				}
			}
			else
			{
				LockedOn = !LockedOn;
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

		protected override void OnFixedUpdate()
		{
			if ( Network.IsProxy ) return;
			UpdateLockOnAbles();
		}

		protected override void OnStart()
		{
			if ( Network.IsProxy ) return;
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
				if ( lockOnAble.ParentIsAlive() && lockOnAble.GameObject.Id != GameObject.Id )
				{
					LockOnAbles.Add( lockOnAble );
				}
			}
			CurrentLockOnAblePosition = CurrentLockOnAble != null ? (CurrentLockOnAble.Transform.Position + CurrentLockOnAble.LockOnOffset) : Vector3.Zero;
		}

		private LockOnAble GetClosestLockOnAble()
		{
			LockOnAble closest = null;
			float closestDistance = float.MaxValue;

			foreach ( var lockOnAble in LockOnAbles )
			{
				float distance = Vector3.DistanceBetween( Transform.Position, (lockOnAble.Transform.Position + lockOnAble.LockOnOffset) );
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
				float distance = Vector3.DistanceBetween( Transform.Position, (lockOnAble.Transform.Position + lockOnAble.LockOnOffset) );
				if ( distance < closestDistance && IsWithinView( lockOnAble.Transform ))
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

				Vector3 toTarget = (lockOnAble.Transform.Position + lockOnAble.LockOnOffset) - Transform.Position;
				Vector3 toCurrentTarget = CurrentLockOnAblePosition - Transform.Position;

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

		public void Respawn()
		{
			Log.Info( "respawn" );

			if (!CanRespawn) return;
			if ( Network.IsProxy ) return;

			Log.Info( "respawno" );


			Bonfire lastRestedBonfire = GetLastRestedOrRandomBonfire();

			IEnumerable<SpawnPoint> spawnPoints = lastRestedBonfire.Components.GetAll<SpawnPoint>( find: FindMode.EnabledInSelfAndDescendants );

			SpawnPoint spawnPoint = spawnPoints.GetRandomItem();

			Transform.Position = spawnPoint.Transform.Position;
			Transform.Rotation = spawnPoint.Transform.Rotation;

			IsDead = false;
			IsRespawning = true;

			CharacterVitals.ResetVitals();
			PlayerStats.Souls = 10;
		}

		private Bonfire GetLastRestedOrRandomBonfire()
		{
			if ( lastRestedBonfire != null ) return lastRestedBonfire;
			IEnumerable<Bonfire> bonfires = Scene.GetAllComponents<Bonfire>();
			return bonfires.GetRandomItem();
		}
	}
}

