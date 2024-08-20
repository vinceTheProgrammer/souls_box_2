using Sandbox;

namespace SoulsBox
{
	/// <summary>
	/// Interface between character combat and souls box agent
	/// </summary>
	[Title( "Souls Box Character Combat Controller" )]
	[Category( "Souls Box Character" )]
	[Icon( "sports_mma" )]
	public sealed class CharacterCombatController : Component
	{

		[Property]
		CharacterAgent Agent { get; set; }

		GameObject CurrentWeapon { get; set; }

		public bool CanDealDamage { get; set; }

		public enum AttackControl 
		{
			LeftLightAttack,
			LeftHeavyAttack,
			RightLightAttack,
			RightHeavyAttack,
		}

		public enum PhysicalAttackType
		{
			Regular,
			Slash,
			Strike,
			Thrust
		}

		public enum MagicAttackType
		{
			Sorcery,
			Pyromancy,
			Miracles
		}

		public enum AttackCategory
		{
			Melee,
			Sheld,
			Archery,
			Magic
		}


		protected override void OnFixedUpdate()
		{
			if (Agent.IsDead) return;
			if (Agent.CharacterAnimationController.IsTagActive("SB_Hitbox_Active"))
			{
				if ( Network.IsProxy ) return;
				if ( CurrentWeapon == null ) return;
				if (CanDealDamage)
				{
					AttemptDealDamage();
				}
			}
		}

		protected override void OnStart()
		{
			if ( Network.IsProxy ) return;
			CurrentWeapon = GetCurrentWeaponGameObject();
		}

		public GameObject GetCurrentWeaponGameObject()
		{
			var weapon = GameObject.Components.GetInDescendants<Weapon>();
			if ( weapon != null ) return weapon.GameObject;
			else return null;
		}

		private void AttemptDealDamage()
		{
			float radius = 10f;
			Vector3 from = CurrentWeapon.Transform.Position + (CurrentWeapon.Transform.Rotation.Forward.Normal * 22f);
			Vector3 to = CurrentWeapon.Transform.Position + (CurrentWeapon.Transform.Rotation.Backward.Normal * 22f);
			Capsule hitboxCapsule = new Capsule( from, to, radius );
			var hitboxTrace = Scene.Trace.Capsule( hitboxCapsule ).IgnoreGameObjectHierarchy( GameObject ).UseHitboxes( true ).Run();
			if ( hitboxTrace.Hit )
			{
				//if ( hitboxTrace.GameObject != null ) Log.Info( hitboxTrace.GameObject.Name );
				CharacterAgent hitAgent = hitboxTrace.GameObject.Components.Get<CharacterAgent>();
				if ( hitAgent != null )
				{
					SBDamage damage = new( SBDamage.DamageType.PhysicalSlash, 30, GameObject.Id, CurrentWeapon.Id );
					hitAgent.CharacterDefenseController.TryReceiveDamage( damage, hitboxTrace.HitPosition + ((from + to) / 2) );
					CanDealDamage = false;
					if (Agent is AgentPlayer) SpawnParticle( "\\prefabs\\particles\\blood.prefab", hitboxTrace.HitPosition + ((from + to) / 2) );
					return;
				}
				IDamageable damageable = hitboxTrace.GameObject.Components.Get<IDamageable>();
				if ( damageable != null )
				{
					damageable.OnDamage( new DamageInfo(100, GameObject, CurrentWeapon)   );
				}
				SpawnParticle( "\\prefabs\\particles\\sparks.prefab", hitboxTrace.HitPosition + ((from + to) / 2) );
				/*
				if (hitboxTrace.GameObject.Name.ToLower() == "world physics" || hitboxTrace.GameObject.Name.ToLower() == "world_physics" )
				{
					SpawnParticle( "\\prefabs\\particles\\sparks.prefab", hitboxTrace.HitPosition + ((from + to) / 2));
				}
				*/
			}
		}

		// move somewhere else later
		public static GameObject SpawnParticle(string filepath, Vector3 position)
		{
			PrefabFile prefabFile = ResourceLibrary.Get<PrefabFile>( filepath );
			PrefabScene particlePrefab = SceneUtility.GetPrefabScene( prefabFile );
			return particlePrefab.Clone( position );
		}
	}
}
