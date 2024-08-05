using System;
using System.Collections.Generic;

public class Octree<T> where T : Component
{
	private OctreeNode<T> root;

	public Octree( float x, float y, float z, float size )
	{
		root = new OctreeNode<T>( x, y, z, size );
	}

	public void Insert( T item )
	{
		root.Insert( item );
	}

	public void Remove( T item )
	{
		root.Remove( item );
	}

	public List<T> QueryRange( Vector3 center, float radius )
	{
		return root.QueryRange( center, radius );
	}

	public void Clear()
	{
		root.Clear();
	}

	public void Draw()
	{
		root.Draw();
	}
}

public class OctreeNode<T> where T : Component
{
	private const int Capacity = 10000;
	private List<T> items;
	private OctreeNode<T>[] children;
	private float x, y, z, size;
	private bool divided;

	public OctreeNode( float x, float y, float z, float size )
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.size = size;
		this.items = new List<T>();
		this.divided = false;
	}

	public void Insert( T item )
	{
		if ( !Contains( item.Transform.Position ) ) return;

		if ( items.Count < Capacity )
		{
			items.Add( item );
		}
		else
		{
			if ( !divided ) Subdivide();

			foreach ( var child in children )
			{
				child.Insert( item );
			}
		}
	}

	public void Remove( T item )
	{
		if ( !Contains( item.Transform.Position ) ) return;

		if ( items.Contains( item ) )
		{
			items.Remove( item );
		}
		else if ( divided )
		{
			foreach ( var child in children )
			{
				child.Remove( item );
			}
		}
	}

	public List<T> QueryRange( Vector3 center, float radius )
	{
		var result = new List<T>();

		if ( !Intersects( center, radius ) ) return result;

		foreach ( var item in items )
		{
			if ( Vector3.DistanceBetween( item.Transform.Position, center ) <= radius )
			{
				result.Add( item );
			}
		}

		if ( divided )
		{
			foreach ( var child in children )
			{
				result.AddRange( child.QueryRange( center, radius ) );
			}
		}

		return result;
	}

	public void Draw()
	{
		Gizmo.Draw.LineBBox( BBox.FromPositionAndSize(new Vector3( x + size / 2, y + size / 2, z + size / 2), size ));

		// Recursively draw child nodes
		if ( divided )
		{
			foreach ( var child in children )
			{
				child.Draw();
			}
		}
	}

	public void Clear()
	{
		items.Clear();

		if ( divided )
		{
			foreach ( var child in children )
			{
				child.Clear();
			}
			children = null;
			divided = false;
		}
	}

	private void Subdivide()
	{
		float halfSize = size / 2f;

		children = new OctreeNode<T>[8];
		children[0] = new OctreeNode<T>( x, y, z, halfSize );
		children[1] = new OctreeNode<T>( x + halfSize, y, z, halfSize );
		children[2] = new OctreeNode<T>( x, y + halfSize, z, halfSize );
		children[3] = new OctreeNode<T>( x + halfSize, y + halfSize, z, halfSize );
		children[4] = new OctreeNode<T>( x, y, z + halfSize, halfSize );
		children[5] = new OctreeNode<T>( x + halfSize, y, z + halfSize, halfSize );
		children[6] = new OctreeNode<T>( x, y + halfSize, z + halfSize, halfSize );
		children[7] = new OctreeNode<T>( x + halfSize, y + halfSize, z + halfSize, halfSize );

		divided = true;
	}

	private bool Contains( Vector3 position )
	{
		return position.x >= x && position.x < x + size &&
			   position.y >= y && position.y < y + size &&
			   position.z >= z && position.z < z + size;
	}

	private bool Intersects( Vector3 center, float radius )
	{
		float closestX = Math.Max( x, Math.Min( center.x, x + size ) );
		float closestY = Math.Max( y, Math.Min( center.y, y + size ) );
		float closestZ = Math.Max( z, Math.Min( center.z, z + size ) );

		float distanceX = center.x - closestX;
		float distanceY = center.y - closestY;
		float distanceZ = center.z - closestZ;

		return (distanceX * distanceX + distanceY * distanceY + distanceZ * distanceZ) <= (radius * radius);
	}
}
