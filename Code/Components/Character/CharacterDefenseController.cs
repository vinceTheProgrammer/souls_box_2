using Sandbox;

namespace SoulsBox
{
	/// <summary>
	/// Interface between character defense and souls box agent
	/// </summary>
	[Title( "Souls Box Character Defense Controller" )]
	[Category( "Souls Box Character" )]
	[Icon( "sports_mma" )]
	public sealed class CharacterDefenseController : Component
	{
		[Property]
		CharacterAgent Agent { get; set; }

		[Broadcast]
		public void TryReceiveDamage(SBDamage damage, Vector3 hitPosition)
		{
			if ( Agent == null || Agent.CharacterAnimationController == null || Agent.CharacterVitals == null )
				return;

			if ( Agent.CharacterAnimationController.IsTagActive( "SB_Invincible")) return;
			if ( Agent.IsGuarding && !Agent.CharacterAnimationController.IsTagActive( "SB_Staggered" ) && !Agent.CharacterAnimationController.IsTagActive("SB_Attacking"))
			{
				ReceiveGuardedDamage( damage, hitPosition );
				return;
			}
			var attacker = Scene.Directory.FindByGuid( damage.Attacker );
			Agent.CharacterVitals.Hurt( damage.Value, attacker );
			Agent.CharacterVitals.decreasePoise( 20 );
			Sound.Play( "knife-stab", hitPosition );
			SpawnParticle( "\\prefabs\\particles\\blood.prefab", hitPosition );
		}

		private void ReceiveGuardedDamage(SBDamage damage, Vector3 hitPosition)
		{
			if ( Agent == null || Agent.CharacterVitals == null ) return;
			Agent.CharacterVitals.DrainStamina( 20 );
			if (Agent.CharacterVitals.Stamina <= 0 )
			{
				Agent.CharacterVitals.decreasePoise(9999999);
			}
			Sound.Play( "swords-clash", hitPosition );
			SpawnParticle( "\\prefabs\\particles\\sparks.prefab", hitPosition );
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
