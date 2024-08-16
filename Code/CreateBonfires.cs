using Sandbox.Network;
using static Sandbox.VertexLayout;

namespace Sandbox;

public sealed class CreateBonfires : Component
{
	protected override void OnAwake()
	{
		IEnumerable<SpawnPoint> spawnPoints = Scene.GetAllComponents<SpawnPoint>();
		List<GameObject> spawnPointsToDelete = new List<GameObject>();
		Log.Info( spawnPoints );
		List<Vector3> bonfirePositions = new List<Vector3>();
		PrefabFile prefabFile = ResourceLibrary.Get<PrefabFile>( "\\prefabs\\bonfire.prefab" );
		PrefabScene bonfirePrefab = SceneUtility.GetPrefabScene( prefabFile );
		float minDistance = 1000.0f;

		foreach ( var spawnPoint in spawnPoints )
		{
			bool canAddBonfire = true;

			if (SpaceNotClear(spawnPoint.Transform.Position)) canAddBonfire = false;

			if (bonfirePositions.Count > 0 )
			{
				for ( int i = 0; i < bonfirePositions.Count; i++ )
				{
					if ( Vector3.DistanceBetween( spawnPoint.Transform.Position, bonfirePositions[i] ) < minDistance)
					{
						canAddBonfire = false;
						break;
					}
				}
			}

			if ( canAddBonfire ) bonfirePositions.Add( spawnPoint.Transform.Position );
			spawnPointsToDelete.Add( spawnPoint.GameObject );
		}

		if ( bonfirePositions.Count != 0 )
		{
			foreach ( var spawnPoint in spawnPointsToDelete )
			{
				spawnPoint.Destroy();
			}

			foreach ( var bonfirePosition in bonfirePositions )
			{
				GameObject bonfire = bonfirePrefab.Clone( bonfirePosition + new Vector3(0, 0, -8f) );
				Log.Info( "bonfire: " + bonfire.Components.Get<BoxCollider>().Touching.Count() );
				bonfire.Components.Get<Bonfire>().Light();
			}
		}

		Destroy();
	}

	private bool SpaceNotClear(Vector3 position)
	{
		bool isSpaceNotClear = false;

		float height = 80f;
		float width = 130f;
		float length = 130f;
		Vector3 offset = new( 0, 0, -8f );
		Vector3 center = position + new Vector3( 0, 0, height / 2 ) + offset;

		float maxAllowedHeight = 10f;
		
		Vector3 normal1 = GameObject.Transform.Rotation.Forward.Normal;
		Vector3 normal2 = GameObject.Transform.Rotation.Right.Normal;
		Vector3 normal3 = GameObject.Transform.Rotation.Backward.Normal;
		Vector3 normal4 = GameObject.Transform.Rotation.Left.Normal;
		Vector3 normalUp = GameObject.Transform.Rotation.Up.Normal;
		Vector3 normalDown = GameObject.Transform.Rotation.Down.Normal;

		Vector3 end1 = center + normal1 * width / 2;
		Vector3 end2 = center + normal2 * width / 2;
		Vector3 end3 = center + normal3 * width / 2;
		Vector3 end4 = center + normal4 * width / 2;
		Vector3 endUp = center + normalUp * height / 2;
		Vector3 endDown = center + normalDown * ((height / 2) + maxAllowedHeight);

		var tr1 = Scene.Trace.Ray( center, end1 ).Size( 3f ).UseHitboxes( true ).Run();
		var tr2 = Scene.Trace.Ray( center, end2 ).Size( 3f ).UseHitboxes( true ).Run();
		var tr3 = Scene.Trace.Ray( center, end3 ).Size( 3f ).UseHitboxes( true ).Run();
		var tr4 = Scene.Trace.Ray( center, end4 ).Size( 3f ).UseHitboxes( true ).Run();
		var trUp = Scene.Trace.Ray( center, endUp ).Size( 3f ).UseHitboxes( true ).Run();
		var trDown = Scene.Trace.Ray( center, endDown ).Size( 3f ).UseHitboxes( true ).Run();

		//DebugOverlay.Trace( tr1, 120f );
		//DebugOverlay.Trace( tr2, 120f );
		//DebugOverlay.Trace( tr3, 120f );
		//DebugOverlay.Trace( tr4, 120f );


		if ( tr1.Hit || tr2.Hit || tr3.Hit || tr4.Hit || trUp.Hit || !trDown.Hit) isSpaceNotClear = true;

		return isSpaceNotClear;
	}
}
