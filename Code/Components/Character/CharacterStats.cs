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
		public int MaxHealth { get; set; } = 400;
		public int MaxStamina { get; set; } = 81;
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
		public int MaxPoise { get; set; } = 46;
		public int BleedResist { get; set; }
		public int PoisonResist { get; set; }
		public int CurseResist { get; set; }
		public int ItemDiscovery {  get; set; }
		public int AttunementSlots { get; set; }


		public void CalculateStats(PlayerStats attributes)
		{
			MaxHealth = CalculateMaxHealth( attributes.Vitality );
			AttunementSlots = CalculateAttunementSlots( attributes.Attunement );
			MaxStamina = CalculateMaxStamina(attributes.Endurance );
			MaxEquipLoad = CalculateMaxEquipLoad(attributes.Endurance);
			BleedResist = CalculateBleedResist(attributes.Endurance);
		}

		private int CalculateBleedResist(int endurance)
		{
			if ( endurance <= 30 ) return (int)Math.Round(3.1 * endurance + 7);
			else return (int)Math.Round( (3 / (double)4) * endurance + 77 );
		}

		private float CalculateMaxEquipLoad(int endurance)
		{
			return endurance + 40;
		}

		private int CalculateMaxStamina(int endurance)
		{
			if ( endurance <= 1 ) return 80;
			else if ( endurance <= 40 )
			{
				double stamina = (1 / (double)71) * Math.Pow( (endurance + 50), 2 ) + 45;
				return (int)Math.Round( stamina );
			}
			else return 160;
		}

		private int CalculateMaxHealth( int vitality )
		{
			if ( vitality <= 0 )
			{
				throw new ArgumentOutOfRangeException( nameof( vitality ), "Vitality must be greater than 0." );
			}

			double health = 200 * Math.Pow( Math.Log( vitality, 10), 3 ) + 400;
			return (int)Math.Round( health );
		}

		private int CalculateAttunementSlots(int attunement)
		{
			if ( attunement <= 9 ) return 0;
			else if ( attunement <= 11 ) return 1;
			else if ( attunement <= 13 ) return 2;
			else if ( attunement <= 15 ) return 3;
			else if ( attunement <= 18 ) return 4;
			else if ( attunement <= 22 ) return 5;
			else if ( attunement <= 27 ) return 6;
			else if ( attunement <= 33 ) return 7;
			else if ( attunement <= 40 ) return 8;
			else if ( attunement <= 49 ) return 9;
			else return 10;
		}

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
			c.Add( MaxPoise );
			c.Add( BleedResist );
			c.Add( PoisonResist );
			c.Add( CurseResist );
			c.Add( ItemDiscovery );
			c.Add( AttunementSlots );
			return c.ToHashCode();
		}
	}
}
