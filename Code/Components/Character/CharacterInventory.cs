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
		public List<Armor> Armors = new List<Armor>();
		public List<Weapon> Weapons = new List<Weapon>();
		public List<Arrow> Arrows = new List<Arrow>();
		public List<Key> Keys = new List<Key>();
		public List<Ring> Rings = new List<Ring>();
		public List<Shard> Shards = new List<Shard>();
		public List<Sorcery> Sorceries = new List<Sorcery>();
		public List<Usable> Usables = new List<Usable>();

		public List<Item> GetList( Item.ItemCategory category )
		{
			return category switch
			{
				Item.ItemCategory.Usable => Usables.Cast<Item>().ToList(),
				Item.ItemCategory.Shard => Shards.Cast<Item>().ToList(),
				Item.ItemCategory.Key => Keys.Cast<Item>().ToList(),
				Item.ItemCategory.Sorcery => Sorceries.Cast<Item>().ToList(),
				Item.ItemCategory.Weapon => Weapons.Cast<Item>().ToList(),
				Item.ItemCategory.Arrow => Arrows.Cast<Item>().ToList(),
				Item.ItemCategory.Armor => Armors.Cast<Item>().ToList(),
				Item.ItemCategory.Ring => Rings.Cast<Item>().ToList(),
				_ => new List<Item>()
			};
		}
	}
}
