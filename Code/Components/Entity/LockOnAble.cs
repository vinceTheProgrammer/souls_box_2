using SoulsBox;
using System;

namespace Sandbox;

public sealed class LockOnAble : Component
{

	[Property]
	public Vector3 LockOnOffset { get; set; }

	public bool ParentIsAlive()
	{
		CharacterAgent agent = GameObject.Components.Get<CharacterAgent>();
		if ( agent != null)
		{
			//Log.Info("alive: " + agent.IsDead);
			return !agent.IsDead;
		}
		else
		{
			return true;
		}
	}
}
