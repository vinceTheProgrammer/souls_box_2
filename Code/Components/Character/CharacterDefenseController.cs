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
			if ( Agent.CharacterAnimationController.IsTagActive( "SB_Invincible")) return;
			if ( Agent.IsGuarding )
			{
				ReceiveGuardedDamage( damage, hitPosition );
				return;
			}
			Agent.CharacterVitals.Hurt( damage.Value, GameObject );
			Sound.Play( "knife-stab", hitPosition );
			SpawnParticle( "\\prefabs\\particles\\blood.prefab", hitPosition );
		}

		protected override void OnFixedUpdate()
		{
			//Log.Info( Agent.IsGuarding );
		}

		private void ReceiveGuardedDamage(SBDamage damage, Vector3 hitPosition)
		{
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
