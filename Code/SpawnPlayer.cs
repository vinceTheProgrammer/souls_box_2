namespace Sandbox;

public sealed class SpawnPlayer : Component
{
	protected override void OnStart()
	{
		IEnumerable<SpawnPoint> spawnPoints = Scene.GetAllComponents<SpawnPoint>();
		Vector3 spawnPointPosition = Vector3.Zero;
		if ( spawnPoints.Count()  > 0 )
		{
			spawnPointPosition = spawnPoints.First().Transform.Position;
		}
		PrefabFile prefabFile = ResourceLibrary.Get<PrefabFile>( "\\prefabs\\player.prefab" );
		PrefabScene playerPrefab = SceneUtility.GetPrefabScene( prefabFile );
		playerPrefab.Clone( spawnPointPosition );
	}
}
