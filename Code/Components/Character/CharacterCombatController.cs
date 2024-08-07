using Sandbox;

namespace SoulsBox
{
	/// <summary>
	/// Interface between character combat and souls box agent
	/// </summary>
	[Title( "Souls Box Character Combat Controller" )]
	[Category( "Souls Box Character" )]
	[Icon( "sports_mma" )]
	public sealed class CharacterCombatController : Component
	{

		[Property]
		CharacterAgent Agent { get; set; }

		public enum AttackControl 
		{
			LeftLightAttack,
			LeftHeavyAttack,
			RightLightAttack,
			RightHeavyAttack,
		}

		enum PhysicalAttackType
		{
			Regular,
			Slash,
			Strike,
			Thrust
		}

		enum MagicAttackType
		{
			Sorcery,
			Pyromancy,
			Miracles
		}

		enum AttackCategory
		{
			Melee,
			Sheld,
			Archery,
			Magic
		}


		protected override void OnFixedUpdate()
		{
			// on attack
				// case attack_control
					// AttackControl.LeftLightAttack:
						// HandleLightAttack(attack)
		}
	}
}
