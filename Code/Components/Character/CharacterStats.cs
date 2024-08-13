using Sandbox;

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
		public int MaxPoise { get; set; }
		public int BleedResist { get; set; }
		public int PoisonResist { get; set; }
		public int CurseResist { get; set; }
		public int ItemDiscovery {  get; set; }
		public int AttunementSlots { get; set; }
    }
}
