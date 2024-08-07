// specifically manages an octree for lock on mechanics, but I wanna change that eventually

public class OctreeManager
{
	private static OctreeManager instance;
	public static OctreeManager Instance => instance ??= new OctreeManager();

	private Octree<LockOnAble> octree;

	private float worldSize = 10000f;

	private OctreeManager()
	{
		float halfWorldSize = worldSize / 2f;
		octree = new Octree<LockOnAble>( -halfWorldSize, -halfWorldSize, -halfWorldSize, worldSize );
	}

	public void Insert( LockOnAble item )
	{
		octree.Insert( item );
	}

	public void Remove( LockOnAble item )
	{
		octree.Remove( item );
	}

	public void Clear()
	{
		octree.Clear();
	}

	public void Draw()
	{
		octree.Draw();
	}

	public void DestroyAndReInit()
	{
		float halfWorldSize = worldSize / 2f;
		octree = new Octree<LockOnAble>( -halfWorldSize, -halfWorldSize, -halfWorldSize, worldSize );
	}

	public List<LockOnAble> QueryRange( Vector3 center, float radius )
	{
		return octree.QueryRange( center, radius );
	}
}
