using Sandbox;
using System;
using System.Security.Cryptography;

namespace SoulsBox
{
	/// <summary>
	/// Stats for souls box character
	/// </summary>
	[Title( "Souls Box Player Stats" )]
	[Category( "Souls Box Player" )]
	[Icon( "casino" )]
	public sealed class PlayerStats : Component
	{
		public int Level { get; set; }
		public int Souls {  get; set; }
		public int Vitality { get; set; }
		public int Attunement { get; set; }
		public int Endurance { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Resistance { get; set; }
		public int Intelligence { get; set; }
		public int Faith { get; set; }
		public int Humanity { get; set; }

		protected override void OnUpdate()
		{
			if ( GameObject.Components.Get<AgentPlayer>() != null )
			{
				if ( !Network.IsProxy )
				{
					Gizmo.Draw.ScreenText( $"Souls: {Souls}", new Vector2( Screen.Width - 100, Screen.Height - 50 ) );
				}
			}
		}

		[Broadcast]
		public void GiveSouls(int amount)
		{
			if ( Network.IsProxy ) return;
			Souls = Math.Clamp( Souls + amount, 0, int.MaxValue );
		}

		[Broadcast]
		public void TakeSouls( int amount )
		{
			if ( Network.IsProxy ) return;
			Souls = Math.Clamp(Souls - amount, 0, int.MaxValue);
		}
	}
}
