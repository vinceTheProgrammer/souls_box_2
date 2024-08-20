using Sandbox.Network;

namespace Sandbox;

public sealed class SpawnScreen : Component
{
	protected override async void OnStart()
	{
		IEnumerable<SpawnPoint> spawnPoints = Scene.GetAllComponents<SpawnPoint>();
		Vector3 spawnPointPosition = Vector3.Zero;
		PrefabFile prefabFile = ResourceLibrary.Get<PrefabFile>( "\\prefabs\\screen.prefab" );
		PrefabScene screenPrefab = SceneUtility.GetPrefabScene( prefabFile );
		GameObject screen = screenPrefab.Clone( spawnPointPosition );
		screen.Network.TakeOwnership();
		screen.NetworkMode = NetworkMode.Never;
	}
}
