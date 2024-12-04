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

		public int Poise { get; private set; }

		private TimeSince StaminaLastRecovered { get; set; }
		private float CurrentStaminaRecoveryPeriod { get; set; } = 0.022f;
		private float StaminaRecoveryPeriod { get; set; } = 0.022f;
		private int StaminaRecoveryAmount { get; set; } = 1;
		private TimeSince StaminaLastDrained { get; set; }
		private float StaminaStartRecoveryPeriod { get; set; } = 2f;

		private TimeSince LastHit { get; set; }
		private float PoiseRecoveryPeriod { get; set; } = 5f;



		protected override void OnUpdate()
		{
			if (GameObject.Components.Get<AgentPlayer>() != null)
			{
				if (!Network.IsProxy)
				{
					Gizmo.Draw.ScreenText( $"Poise: {Poise}", new Vector2(0, 300) );
				}
			}
		}

		protected override void OnFixedUpdate()
		{

			RegenerateStamina();
			RegeneratePoise();
		}

		protected override void OnStart()
		{
			ResetVitals();
		}

		public void decreasePoise(int amount)
		{
			Poise = Math.Clamp( Poise - Math.Abs( amount ), 0, Agent.CharacterStats.MaxPoise );
			LastHit = 0;

			if ( Poise <= 0 )
			{
				Agent.IsStaggered = true;
				Poise = Agent.CharacterStats.MaxPoise;
			}

		}

		public void Heal (int amount)
		{
			Health = Math.Clamp( Health + Math.Abs( amount ), 0, Agent.CharacterStats.MaxHealth );
		}

		public void Hurt (int amount, GameObject attacker) 
		{
			Health = Math.Clamp( Health - Math.Abs( amount ), 0, Agent.CharacterStats.MaxHealth );

			if (Health <= 0 && !Agent.IsDead)
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
			Poise = Stats.MaxPoise;
		}

		// maybe put somewhere else?
		public void Die(CharacterAgent killer)
		{
			Agent.IsDead = true;
			if (killer is AgentPlayer player)
			{
				//Log.Info( "GGG" );
				if ( Agent is AgentPlayer thisPlayer )
				{
					player.PlayerStats.GiveSouls( thisPlayer.PlayerStats.Souls );
					thisPlayer.PlayerStats.Souls = 0;
					GameObject seekingSoul = CharacterCombatController.SpawnParticle( "\\prefabs\\souls_seeking.prefab", thisPlayer.Transform.Position );
					seekingSoul.Components.Get<SeekingSoul>().Target = player;
				} else
				{
					player.PlayerStats.GiveSouls( 100 );
					GameObject seekingSoul = CharacterCombatController.SpawnParticle( "\\prefabs\\souls_seeking.prefab", Transform.Position );
					seekingSoul.Components.Get<SeekingSoul>().Target = player;
				}

			}
		}

		public void Die()
		{
			//Log.Info( "f" );
			Agent.IsDead = true;
		}

		private void RegeneratePoise()
		{
			if (Poise < Agent.CharacterStats.MaxPoise && LastHit > PoiseRecoveryPeriod)
			{
				Poise = Agent.CharacterStats.MaxPoise;
			}
		}

		private void RegenerateStamina()
		{
			if (StaminaLastDrained > StaminaStartRecoveryPeriod)
			{
				float staminaRecoveryPeriodGuard = StaminaRecoveryPeriod * 5;
				if ( Agent.IsGuarding ) CurrentStaminaRecoveryPeriod = staminaRecoveryPeriodGuard;
				else CurrentStaminaRecoveryPeriod = StaminaRecoveryPeriod;
				if ( StaminaLastRecovered > CurrentStaminaRecoveryPeriod )
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
