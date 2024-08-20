using Sandbox;

namespace SoulsBox
{
	public sealed class EntityInventory : Component
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
