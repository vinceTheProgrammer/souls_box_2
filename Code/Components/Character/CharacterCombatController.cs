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


		protected override void OnUpdate()
		{
			if (Agent.IsDead) return;
			if (Agent.CharacterAnimationController.IsTagActive("SB_Hitbox_Active"))
			{
				if ( CurrentWeapon == null ) return;
				if (CanDealDamage)
				{
					AttemptDealDamage();
				}
			}
		}

		protected override void OnStart()
		{
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
				Log.Info( hitboxTrace.GameObject.Name );
				AgentPlayer hitAgent = hitboxTrace.GameObject.Components.Get<AgentPlayer>();
				if ( hitAgent != null )
				{
					hitAgent.CharacterVitals.Hurt( 10, GameObject );
					CanDealDamage = false;
				}
				IDamageable damageable = hitboxTrace.GameObject.Components.Get<IDamageable>();
				if ( damageable != null )
				{
					Log.Info( "damage" );
					damageable.OnDamage( new DamageInfo(10, GameObject, CurrentWeapon)   );
				}
			}
		}
	}
}
