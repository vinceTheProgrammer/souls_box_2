using System;

namespace Sandbox;

public sealed class LockOnAble : Component
{

	[Property]
	public Vector3 LockOnOffset { get; set; }

	public bool ParentIsAlive()
	{
		return true;
	}
}
