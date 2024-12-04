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
		public string Displayname { get; set; }

		[Property]
		public int Level { get; set; }
		public int Souls {  get; set; }

		[Property]
		public int Vitality { get; set; }
		[Property]
		public int Attunement { get; set; }
		[Property]
		public int Endurance { get; set; }
		[Property]
		public int Strength { get; set; }
		[Property]
		public int Dexterity { get; set; }
		[Property]
		public int Resistance { get; set; }
		[Property]
		public int Intelligence { get; set; }
		[Property]
		public int Faith { get; set; }
		public int Humanity { get; set; }

		protected override void OnStart()
		{
			if ( Network.IsProxy ) return;
			Displayname = Network.OwnerConnection.DisplayName;
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

		public override int GetHashCode()
		{
			HashCode c = new HashCode();
			c.Add( Displayname );
			c.Add( Level );
			c.Add( Souls );
			c.Add( Vitality );
			c.Add( Attunement );
			c.Add( Endurance );
			c.Add( Strength );
			c.Add( Dexterity );
			c.Add( Resistance );
			c.Add( Intelligence );
			c.Add( Faith );
			c.Add( Humanity );
			return c.ToHashCode();
		}
	}
}
