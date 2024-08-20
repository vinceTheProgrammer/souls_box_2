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

		private TimeSince StaminaLastRecovered { get; set; }
		private float StaminaRecoveryPeriod { get; set; } = 0.01f;
		private int StaminaRecoveryAmount { get; set; } = 1;
		private TimeSince StaminaLastDrained { get; set; }
		private float StaminaStartRecoveryPeriod { get; set; } = 2f;

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

			RegenerateStamina();
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
			StaminaLastDrained = 0;
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
				GameObject seekingSoul = CharacterCombatController.SpawnParticle( "\\prefabs\\souls_seeking.prefab", thisPlayer.Transform.Position );
				seekingSoul.Components.Get<SeekingSoul>().Target = player;
			}
		}

		public void Die()
		{
			Agent.IsDead = true;
		}

		private void RegenerateStamina()
		{
			if (StaminaLastDrained > StaminaStartRecoveryPeriod)
			{
				if ( StaminaLastRecovered > StaminaRecoveryPeriod )
				{
					Stamina = Math.Clamp( Stamina + StaminaRecoveryAmount, 0, Agent.CharacterStats.MaxStamina );
					StaminaLastRecovered = 0;
				}
			}
			
		}

		public override int GetHashCode()
		{
			HashCode c = new HashCode();
			c.Add( Health );
			c.Add( Stamina );
			return c.ToHashCode();
		}
	}
}
