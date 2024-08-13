using Sandbox;
using System;

namespace SoulsBox
{
	/// <summary>
	/// Vitals for souls box character
	/// </summary>
	[Title( "Souls Box Character Vitals" )]
	[Category( "Souls Box Character" )]
	[Icon( "favorite" )]
	public sealed class CharacterVitals : Component
	{
		[Property]
		CharacterAgent Agent { get; set; }

		[RequireComponent]
		CharacterStats Stats { get; set; }

		public int Health { get; private set; }

		public int Stamina { get; private set; }

		protected override void OnUpdate()
		{
			if (GameObject.Components.Get<AgentPlayer>() != null)
			{
				if (!Network.IsProxy)
				{
					Gizmo.Draw.ScreenText( $"Health: {Health}\nStamina: {Stamina}", new Vector2(0, 0) );
				}
			}
		}

		protected override void OnFixedUpdate()
		{
			GiveStamina( 1 );
		}

		protected override void OnStart()
		{
			ResetVitals();
		}

		public void Heal (int amount)
		{
			Health = Math.Clamp( Health + Math.Abs( amount ), 0, Agent.CharacterStats.MaxHealth );
		}

		public void Hurt (int amount, GameObject attacker) 
		{
			Health = Math.Clamp( Health - Math.Abs( amount ), 0, Agent.CharacterStats.MaxHealth );

			if (Health <= 0)
			{
				CharacterAgent agent = attacker.Components.Get<CharacterAgent>();
				if ( agent != null ) Die( agent );
				else Die();
			}

			DamageInfo damageInfo = new DamageInfo();
			damageInfo.Attacker = attacker;
			Agent.CharacterAnimationController.AnimateHitReaction(damageInfo);
		}

		public void GiveStamina (int amount)
		{
			Stamina =  Math.Clamp(Stamina + Math.Abs( amount ), 0, Agent.CharacterStats.MaxStamina);
		}

		public void DrainStamina (int amount)
		{
			Stamina = Math.Clamp( Stamina - Math.Abs( amount ), 0, Agent.CharacterStats.MaxStamina );
		}

		public void ResetVitals()
		{
			Health = Stats.MaxHealth;
			Stamina = Stats.MaxStamina;
		}

		// maybe put somewhere else?
		public void Die(CharacterAgent killer)
		{
			Agent.IsDead = true;
			if (killer is AgentPlayer player && Agent is AgentPlayer thisPlayer)
			{
				player.PlayerStats.GiveSouls( thisPlayer.PlayerStats.Souls );
				thisPlayer.PlayerStats.Souls = 0;
			}
		}

		public void Die()
		{
			Agent.IsDead = true;
		}
	}
}
