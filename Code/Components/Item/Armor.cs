using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public sealed class Armor : Item
	{
		public float PhysicalDefense { get; set; }
		public float StrikeDefense { get; set; }
		public float SlashDefense { get; set; }
		public float ThrustDefense { get; set; }
		public float MagicDefense { get; set; }
		public float FireDefense { get; set; }
		public float LightningDefense { get; set; }
		public float Poise {  get; set; }
		public float BleedResist { get; set; }
		public float PoisonResist {  get; set; }
		public float CurseResist { get; set; }
		public int MaxDurability { get; set; }
		public int Durability { get; set; }
		public float Weight { get; set; }

		public enum ArmorType
		{
			Head,
			Chest,
			Hands,
			Legs
		}
	}
}
