using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
using SoulsBox;
using static Sandbox.PhysicsContact;

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

		[Property]
		public CameraController CameraController { get; set; }

		[Property]
		public PlayerInteraction PlayerInteraction { get; set; }

		public bool MenuEnabled { get; set; }

		public bool CreationMode { get; private set; }

		public GameTransform LastNormalCameraTransform { get; set; }

		public LockOnAble CurrentLockOnAble { get; set; }

		public HashSet<LockOnAble> LockOnAbles = new HashSet<LockOnAble>();
		public float LockOnRadius { get; set; } = 1000f;

		public bool CanRespawn { get; set; }

		public Bonfire LastRestedBonfire {  get; set; }

		public bool IsRespawning { get; set; }

		public bool IsUsingBonfire { get; set; }
		public bool IsLightingBonfire { get; set; }

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

		private bool LockOnAbleIsWithinView(LockOnAble lockOnAble)
		{
			float leftBound = 0f;
			float rightBound = 1f;
			float topBound = 0f;
			float bottomBound = 1f;
			Vector3 screenCoords = lockOnAble.Transform.Position.ToScreen();
			Vector3 toTarget = (lockOnAble.Transform.Position + lockOnAble.LockOnOffset) - CameraController.Camera.Transform.Position;
			return screenCoords.x >= leftBound && screenCoords.x <= rightBound && screenCoords.y >= topBound && screenCoords.y <= bottomBound && !(Vector3.Dot( CameraController.Camera.Transform.Rotation.Forward, toTarget.Normal ) < 0.0f);
		}	

		protected override void OnFixedUpdate()
		{
			if ( Network.IsProxy ) return;
			UpdateLockOnAbles();
			//Log.Info( "isDead: " + IsDead );
			//Log.Info("isRespawning: " + IsRespawning );
		}

		protected override void OnStart()
		{
			if ( Network.IsProxy ) return;
			OctreeManager.Instance.DestroyAndReInit();
			GiveDebugItems2();
			//GiveDebugItems();
			CharacterStats.CalculateStats( PlayerStats );
		}

		public void UpdateLockOnAbles()
		{
			LockOnAbles.Clear();
			OctreeManager.Instance.Clear();

			if ( CurrentLockOnAble != null && !CurrentLockOnAble.ParentIsAlive() ) {
				CurrentLockOnAble = GetClosestLockOnAbleInView();
				if ( CurrentLockOnAble == null ) LockedOn = false;
			}

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
				} else
				{
					LockOnAbles.Remove( lockOnAble );
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
				if ( distance < closestDistance && LockOnAbleIsWithinView( lockOnAble ))
				{
					closest = lockOnAble;
					closestDistance = distance;
				}
			}

			return closest;
		}

		public void SwitchTarget( bool isLeft )
		{
			LockOnAble bestTarget = null;
			float bestAngle = float.MaxValue;

			foreach ( var lockOnAble in LockOnAbles )
			{
				if ( lockOnAble == CurrentLockOnAble ) continue;

				Vector3 toTarget = (lockOnAble.Transform.Position + lockOnAble.LockOnOffset) - CameraController.Camera.Transform.Position;
				Vector3 toCurrentTarget = CurrentLockOnAblePosition - CameraController.Camera.Transform.Position;

				float angle = toCurrentTarget.SignedAngle( toTarget );

				// Ignore targets behind the player
				if ( Vector3.Dot( CameraController.Camera.Transform.Rotation.Forward, toTarget.Normal ) < 0.0f ) continue;

				if ( !isLeft && angle < 0 && angle > -bestAngle )
				{
					bestAngle = -angle;
					bestTarget = lockOnAble;
				}
				else if ( isLeft && angle > 0 && angle < bestAngle )
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
			if (!CanRespawn) return;
			if ( Network.IsProxy ) return;

			Bonfire lastRestedBonfire = GetLastRestedOrRandomBonfire();

			IEnumerable<SpawnPoint> spawnPoints = lastRestedBonfire.Components.GetAll<SpawnPoint>( find: FindMode.EnabledInSelfAndDescendants );

			SpawnPoint spawnPoint = spawnPoints.GetRandomItem();

			Transform.Position = spawnPoint.Transform.Position;
			Transform.Rotation = spawnPoint.Transform.Rotation;

			IsDead = false;
			IsRespawning = true;

			CharacterVitals.ResetVitals();
			PlayerStats.Souls = 0;

			Arrow testArrow = new Arrow();
			testArrow.Name = "Cool Arrow";

			Arrow testArrow2 = new Arrow { Name = "testy arrow" };

			CharacterInventory.Arrows.Add( testArrow );
			CharacterInventory.Arrows.Add( testArrow2 );
		}

		private Bonfire GetLastRestedOrRandomBonfire()
		{
			if ( LastRestedBonfire != null ) return LastRestedBonfire;
			IEnumerable<Bonfire> bonfires = Scene.GetAllComponents<Bonfire>();
			return bonfires.GetRandomItem();
		}

		public void ToggleCreationMode()
		{
			if (!CreationMode)
			{
				LastNormalCameraTransform = CameraController.Camera.Transform;
			}
			else
			{
				CameraController.Camera.Transform.Position = LastNormalCameraTransform.Position;
				CameraController.Camera.Transform.Rotation =LastNormalCameraTransform.Rotation;
			}
			CreationMode = !CreationMode;
		}

		public void ToggleMenu()
		{
			Menu menu = Scene.Components.GetInChildren<Menu>(includeDisabled: true);
			if ( menu.Enabled )
			{
				MenuEnabled = false;
				menu.Enabled = false;
			} else
			{
				MenuEnabled = true;
				menu.Enabled = true;
				Sandbox.Mouse.Position = new Vector2 ( 0, 0 );
			}
		}

		public void ToggleBonfireRest(Bonfire bonfire = null)
		{
			if (IsUsingBonfire)
			{
				IsUsingBonfire = false;
				Menu menu = Scene.Components.GetInChildren<Menu>( includeDisabled: true );
				menu.Enabled = false;
				menu.CurrentScreen = Menu.MenuScreen.Settings;
			} else
			{
				LastRestedBonfire = bonfire;
				Transform.Rotation = Transform.Rotation.Angles().WithYaw((bonfire.Transform.Position - Transform.Position).Normal.EulerAngles.yaw);
				IsUsingBonfire = true;
				Menu menu = Scene.Components.GetInChildren<Menu>( includeDisabled: true );
				menu.Enabled = true;
				menu.CurrentScreen = Menu.MenuScreen.Bonfire;
			}
		}

		private void GiveDebugItems2()
		{
			Usable steamFlask = new Usable 
			{ 
				ID = "steam_flask",
				Name = "Steam Flask",
				ShortDescription = "Replenish life force.",
				UseAction = () =>
				{
					CharacterVitals.Heal( 300 );
				}
			};
			CharacterInventory.Usables.Add( steamFlask );

			Weapon stingSword = new Weapon
			{
				ID = "sting_sword",
				Name = "Sting Sword",
				ShortDescription = "Basic strait sword.",

			};
		}

			private void GiveDebugItems()
		{
			Arrow testArrow = new Arrow { Name = "nice arrow bro" };
			CharacterInventory.Arrows.Add( testArrow );
			Arrow testArrow2 = new Arrow { Name = "cool arrow" };
			CharacterInventory.Arrows.Add( testArrow2 );
			Arrow testArrow3 = new Arrow { Name = "hot arrow" };
			CharacterInventory.Arrows.Add( testArrow3 );

			Weapon testWeapon = new Weapon { Name = "epic sword" };
			Weapon testWeapon2 = new Weapon { Name = "epic sword 2" };
			Weapon testWeapon3 = new Weapon { Name = "epic sword 3" };
			Weapon testWeapon4 = new Weapon { Name = "epic sword 4" };
			CharacterInventory.Weapons.Add(testWeapon);
			CharacterInventory.Weapons.Add(testWeapon2);
			CharacterInventory.Weapons.Add(testWeapon3);
			CharacterInventory.Weapons.Add(testWeapon4);


			Ring testRing = new Ring { Name = "ring of epicness" };
			Ring testRing2 = new Ring { Name = "ring of coolness" };
			CharacterInventory.Rings.Add(testRing);
			CharacterInventory.Rings.Add(testRing2);

			Armor armor = new Armor { Name = "test armor" };
			Armor armor2 = new Armor { Name = "Bronze armor" };
			Armor armor1 = new Armor { Name = "Golden Armor" };
			CharacterInventory.Armors.Add(armor);
			CharacterInventory.Armors.Add(armor2);
			CharacterInventory.Armors.Add(armor1);

			Sorcery sorcery = new Sorcery { Name = "Spell of cool" };
			Sorcery sorcery2 = new Sorcery { Name = "Spell of joy" };
			Sorcery sorcery3 = new Sorcery { Name = "Spell of mad" };
			Sorcery sorcery4 = new Sorcery { Name = "Spell of sad" };
			CharacterInventory.Sorceries.Add(sorcery);
			CharacterInventory.Sorceries.Add(sorcery2);
			CharacterInventory.Sorceries.Add( sorcery3 );
			CharacterInventory.Sorceries.Add( sorcery4 );
			CharacterInventory.Sorceries.Add( sorcery4 );
			CharacterInventory.Sorceries.Add( sorcery4 );
			CharacterInventory.Sorceries.Add( sorcery4 );
			CharacterInventory.Sorceries.Add( sorcery4 );
			CharacterInventory.Sorceries.Add( sorcery4 );


			Usable usable = new Usable { Name = "salt" };
			Usable usable2 = new Usable { Name = "flask" };
			Usable usable3 = new Usable { Name = "soul" };
			CharacterInventory.Usables.Add( usable );
			CharacterInventory.Usables.Add ( usable2 );
			CharacterInventory.Usables.Add( usable3 );

			Key key1 = new Key { Name = "key1" };
			Key key2 = new Key { Name = "key2" };
			Key key3 = new Key { Name = "key3" };
			CharacterInventory.Keys.Add( key1 );
			CharacterInventory.Keys.Add( key2 );
			CharacterInventory.Keys.Add( key3 );
		}
	}
}

