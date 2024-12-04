using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public abstract class Item : Component
	{
		public string ID {  get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public string Tips { get; set; }
		public string Description { get; set; }
		public string LoreDescription { get; set; }

		public int Quantity { get; set; }

		public ItemCategory itemCategory { get; set; }

		public enum ItemCategory
		{
			Usable,
			Shard,
			Key,
			Sorcery,
			Weapon,
			Arrow,
			Armor,
			Ring
		}
	}
}
