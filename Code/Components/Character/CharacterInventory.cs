using Sandbox;

namespace SoulsBox
{
	/// <summary>
	/// Inventory for souls box character
	/// </summary>
	[Title( "Souls Box Character Inventory" )]
	[Category( "Souls Box Character" )]
	[Icon( "inventory_2" )]
	public sealed class CharacterInventory : Component
	{
		public List<Armor> Armors;
		public List<Weapon> Weapons;
		public List<Arrow> Arrows;
		public List<Key> Keys;
		public List<Ring> Rings;
		public List<Shard> Shards;
		public List<Sorcery> Sorceries;
		public List<Usable> Usable;
	}
}
