using Sandbox.Components.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public sealed class SBDamage : DamageInfo
	{
		public DamageType Type { get; set; }
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

		public SBDamage(DamageType type, int damage, GameObject attacker, GameObject weapon, Hitbox hitbox ) 
		{
			Type = type;
			Damage = damage;
			Attacker = attacker;
			Weapon = weapon;
			Hitbox = hitbox;
		}

		public SBDamage( DamageType type, int damage, GameObject attacker, GameObject weapon, Hitbox hitbox, StatusEffect statusEffect )
		{
			Type = type;
			Damage = damage;
			Attacker = attacker;
			Weapon = weapon;
			Hitbox = hitbox;
			StatusEffects.Add( statusEffect );
		}

		public int GetDamageValue()
		{
			return (int) Math.Round(Damage);
		}

	}
}
