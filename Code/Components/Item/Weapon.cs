using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public sealed class Weapon : Component
	{
		public CharacterCombatController.PhysicalAttackType	AttackType { get; set; }
		public WeaponType Type { get; set; }
		public int PhysicalAttack {  get; set; }
		public int MagicAttack {  get; set; }
		public int FireAttack {  get; set; }
		public int LightningAttack { get; set; }
		public int CriticalAttack { get; set; }
		public int BleedEffect { get; set; }
		public int PoisonEffect { get; set; }
		public int DivineEffect { get; set; }
		public int OccultEffect { get; set; }
		public float PhysicalDamageReduction { get; set; }
		public float MagicDamageReduction { get; set; }
		public float FireDamageReduction { get; set; }
		public float LightningDamageReduction { get; set; }
		public int Stability {  get; set; }
		public int RequiredStrength { get; set; }
		public int RequiredDexterity { get; set; }
		public int RequiredIntelligence { get; set; }
		public int RequiredFaith {  get; set; }
		public int MaxDurability { get; set; }
		public float Weight { get; set; }
		public enum WeaponType
		{
			StraightSword,
			Dagger,
			Spear,
			Greatsword,
			UltraGreatsword,
			CurvedSword,
			Katana,
			CurvedGreatsword,
			PiercingSword,
			Axe,
			GreatAxe,
			Hammer,
			GreatHammer,
			FistOrClaw,
			Halberd,
			Whip,
			Bow,
			Greatbow,
			Crossbow,
			Catalyst,
			Flame,
			Talisman
		}
	}
}
