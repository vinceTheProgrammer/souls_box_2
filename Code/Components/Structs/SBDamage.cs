using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public struct SBDamage
	{
		public DamageType Type { get; set; }
		public int Value { get; set; }
		public Guid Attacker { get; set; }
		public Guid Weapon { get; set; } 
		public List<StatusEffect> StatusEffects { get; set; } = new List<StatusEffect>();

		public enum DamageType
		{
			PhysicalStrike,
			PhysicalSlash,
			PhysicalThrust,
			Magic,
			Flame,
			Lightning
		}

		public SBDamage(DamageType type, int damage, Guid attacker, Guid weapon) 
		{
			Type = type;
			Value = damage;
			Attacker = attacker;
			Weapon = weapon;
		}

		public SBDamage( DamageType type, int damage, Guid attacker, Guid weapon, StatusEffect statusEffect )
		{
			Type = type;
			Value = damage;
			Attacker = attacker;
			Weapon = weapon;
			StatusEffects.Add( statusEffect );
		}
	}
}
