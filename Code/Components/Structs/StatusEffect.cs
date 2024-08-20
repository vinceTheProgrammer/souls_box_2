using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public struct StatusEffect
	{
		public StatusEffectType Type { get; set; }
		public int StatusEffectValue { get; set; }
		public enum StatusEffectType
		{
			Poise,
			Bleed,
			Poison,
			Curse
		}
		public StatusEffect(StatusEffectType statusEffect, int statusEffectValue)
		{
			Type = statusEffect;
			StatusEffectValue = statusEffectValue;
		}
	}
}
