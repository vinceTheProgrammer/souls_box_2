using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Components.Types
{
	public sealed class StatusEffect
	{
		public StatusEffect Type { get; set; }
		public int StatusEffectValue { get; set; }
		public enum StatusEffectType
		{
			Poise,
			Bleed,
			Poison,
			Curse
		}
		public StatusEffect(StatusEffect statusEffect, int statusEffectValue)
		{
			Type = statusEffect;
			StatusEffectValue = statusEffectValue;
		}
	}
}
