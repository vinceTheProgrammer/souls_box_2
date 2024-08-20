using Sandbox;
using System;

namespace SoulsBox
{
	/// <summary>
	/// Stats for souls box character
	/// </summary>
	[Title( "Souls Box Character Stats" )]
	[Category( "Souls Box Character" )]
	[Icon( "casino" )]
	public sealed class CharacterStats : Component
    {
		public int MaxHealth { get; set; } = 100;
		public int MaxStamina { get; set; } = 100;
        public float EquipLoad {  get; set; }
		public float MaxEquipLoad { get; set; }
		public int RWeapon1 { get; set; }
		public int RWeapon2 { get; set;}
		public int LWeapon1 { get; set; }
		public int LWeapon2 { get; set;}
		public int PhysicalDefense { get; set; }
		public int PhysicalDefenseNatural { get; set; }
		public int StrikeDefense { get; set; }
		public int StrikeDefenseNatural { get; set; }
		public int SlashDefense { get; set; }
		public int SlashDefenseNatural { get; set; }
		public int ThrustDefense { get; set; }
		public int ThrustDefenseNatural { get; set; }
		public int MagicDefense { get; set; }
		public int MagicDefenseNatural { get; set; }
		public int FlameDefense { get; set; }
		public int FlameDefenseNatural { get; set; }
		public int LightningDefense { get; set; }
		public int LightningDefenseNatural { get; set; }
		public int Poise { get; set; }
		public int BleedResist { get; set; }
		public int PoisonResist { get; set; }
		public int CurseResist { get; set; }
		public int ItemDiscovery {  get; set; }
		public int AttunementSlots { get; set; }

		public override int GetHashCode()
		{
			HashCode c = new HashCode();
			c.Add( MaxHealth );
			c.Add( MaxStamina );
			c.Add( EquipLoad );
			c.Add( MaxEquipLoad );
			c.Add( RWeapon1 );
			c.Add( RWeapon2 );
			c.Add( LWeapon1 );
			c.Add( LWeapon2 );
			c.Add( PhysicalDefense );
			c.Add( PhysicalDefenseNatural );
			c.Add( StrikeDefense );
			c.Add( StrikeDefenseNatural );
			c.Add( SlashDefense );
			c.Add( SlashDefenseNatural );
			c.Add( ThrustDefense );
			c.Add( ThrustDefenseNatural );
			c.Add( MagicDefense );
			c.Add( MagicDefenseNatural );
			c.Add( FlameDefense );
			c.Add( FlameDefenseNatural );
			c.Add( LightningDefense );
			c.Add( LightningDefenseNatural );
			c.Add( Poise );
			c.Add( BleedResist );
			c.Add( PoisonResist );
			c.Add( CurseResist );
			c.Add( ItemDiscovery );
			c.Add( AttunementSlots );
			return c.ToHashCode();
		}
	}
}
