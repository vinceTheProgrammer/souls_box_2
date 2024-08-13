using Sandbox;

namespace SoulsBox
{
	/// <summary>
	/// Equipment for souls box character
	/// </summary>
	[Title( "Souls Box Character Equipment" )]
	[Category( "Souls Box Character" )]
	[Icon( "shield" )]
	public sealed class CharacterEquipment : Component
	{
		public Weapon RWeapon1 { get; set; }
		public Weapon RWeapon2 { get; set; }
		public Weapon LWeapon1 { get; set; }
		public Weapon LWeapon2 { get; set;}
		public Usable Item1 { get; set; }
		public Usable Item2 { get; set; }
		public Usable Item3 { get; set; }
		public Usable Item4 { get; set; }
		public Usable Item5 { get; set; }
		public Arrow LArrows1 { get; set; }
		public Arrow LArrows2 { get; set; }
		public Arrow RArrows1 { get; set; }
		public Arrow RArrows2 { get; set; }
		public Armor Head {  get; set; }
		public Armor Chest { get; set; }
		public Armor Hands { get; set; }
		public Armor Legs { get; set; }
		public Ring Ring1 { get; set; }
		public Ring Ring2 { get; set; }
	}
}
