using Sandbox.Network;

namespace Sandbox;

public sealed class SpawnPlayer : Component
{
	protected override async void OnStart()
	{
		/*
		List<LobbyInformation> lobbies;
		lobbies = await Networking.QueryLobbies( "souls_box_2_private_testing" ); // TODO: change this
		if ( lobbies.Count() > 0 )
		{
			GameNetworkSystem.Connect( lobbies.First().LobbyId );
			SpawnPlayerAtRandomSpawnPoint();
			return;
		}
		if ( !GameNetworkSystem.IsActive )
		{
			GameNetworkSystem.CreateLobby();
		}
		SpawnPlayerAtRandomSpawnPoint();
		*/
		//LogSceneHierarchy();
	}

	private void SpawnPlayerAtRandomSpawnPoint()
	{
		IEnumerable<SpawnPoint> spawnPoints = Scene.GetAllComponents<SpawnPoint>();
		Vector3 spawnPointPosition = Vector3.Zero;
		if ( spawnPoints.Count() > 0 )
		{
			spawnPointPosition = spawnPoints.GetRandomItem().Transform.Position;
		}
		PrefabFile prefabFile = ResourceLibrary.Get<PrefabFile>( "\\prefabs\\player.prefab" );
		PrefabScene playerPrefab = SceneUtility.GetPrefabScene( prefabFile );
		GameObject player = playerPrefab.Clone( spawnPointPosition );
		player.Network.TakeOwnership();
	}

	private void LogSceneHierarchy()
	{
		IEnumerable<GameObject> objects = Game.ActiveScene.GetAllObjects(true);
		foreach ( GameObject obj in objects )
		{
			Log.Info( obj.Name + ": " + !obj.Network.IsProxy );
		}
	}
}
