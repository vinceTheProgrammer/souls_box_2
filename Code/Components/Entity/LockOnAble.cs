using SoulsBox;
using System;

namespace Sandbox;

public sealed class LockOnAble : Component
{

	[Property]
	public Vector3 LockOnOffset { get; set; }

	public bool ParentIsAlive()
	{
		if ( GameObject == null ) return false;
		CharacterAgent agent = GameObject.Components.Get<CharacterAgent>();
		if ( agent != null)
		{
			return !agent.IsDead;
		}
		else
		{
			return false;
		}
	}
}
