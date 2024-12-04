using Sandbox;
using System;

namespace SoulsBox
{
	/// <summary>
	/// Interface between Souls Box character and an AI agent.
	/// </summary>
	[Title( "Souls Box Boss AI Agent" )]
	[Category( "Souls Box" )]
	[Icon( "man" )]
	public sealed class AgentBossAI : CharacterAgent
	{
		private AgentPlayer CurrentTarget { get; set; }

		private TimeUntil NextPhaseTime { get; set; }

		private Phase CurrentPhase { get; set; }

		private enum Phase
		{
			Idle,
			Cower,
			Charge,
			Slam,
			Beam
		}

		private AgentPlayer GetRandomPlayer()
		{
			var players = Scene.Components.GetAll<AgentPlayer>();
			Random random = new Random();
			return players.ElementAt( random.Next( players.Count() ) );
		}

		private Vector3 GetToTargetVector()
		{
			return (CurrentTarget.Transform.Position - Transform.Position).Normal;
		}

		private void ExecuteCurrentPhase()
		{
			switch ( CurrentPhase )
			{
				case Phase.Idle:
					IdlePhase();
					break;
				case Phase.Cower:
					CowerPhase();
					break;
				case Phase.Charge:
					ChargePhase();
					break;
				case Phase.Beam:
					BeamPhase();
					break;
				case Phase.Slam:
					SlamPhase();
					break;
				default:
					IdlePhase();
					break;
			}
		}

		private void SlamPhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVector().EulerAngles.yaw );
		}

		private void BeamPhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVector().EulerAngles.yaw );
		}

		private void ChargePhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVector().EulerAngles.yaw );
		}

		private void CowerPhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVector().EulerAngles.yaw );
		}

		private void IdlePhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVector().EulerAngles.yaw );
		}

		private void ChooseNextPhase()
		{
			Phase NextPhase = GetRandomPhase();
			NextPhaseTime = GetRandomTimeUntil(NextPhase);
			CurrentTarget = GetRandomPlayer();
			CurrentPhase = NextPhase;

		}

		private static Phase GetRandomPhase()
		{
			Random random = new Random();

			Array values = Enum.GetValues( typeof( Phase ) );

			return (Phase)values.GetValue( random.Next( values.Length ) );
		}

		private static float GetRandomTimeUntil(Phase nextPhase)
		{

			float min = GetPhaseMinTime(nextPhase);
			float max = GetPhaseMaxTime(nextPhase);

			Random random = new Random();

			int minMilliseconds = (int)TimeSpan.FromSeconds( min ).TotalMilliseconds;
			int maxMilliseconds = (int)TimeSpan.FromSeconds( max ).TotalMilliseconds;

			int randomMilliseconds = random.Next( minMilliseconds, maxMilliseconds );

			return TimeSpan.FromMilliseconds( randomMilliseconds ).Seconds;
		}

		private static float GetPhaseMinTime(Phase phase)
		{
			switch (phase)
			{
				case Phase.Idle:
					return 1;
				case Phase.Cower:
					return 2;
				case Phase.Charge:
					return 3;
				case Phase.Beam:
					return 2;
				case Phase.Slam:
					return 3;
				default:
					return 2;
			}
		}

		private static float GetPhaseMaxTime(Phase phase)
		{
			switch ( phase )
			{
				case Phase.Idle:
					return 5;
				case Phase.Cower:
					return 8;
				case Phase.Charge:
					return 7;
				case Phase.Beam:
					return 5;
				case Phase.Slam:
					return 6;
				default:
					return 5;
			}
		}

		protected override void OnFixedUpdate()
		{
			if ( Network.IsProxy ) return;
			if ( NextPhaseTime ) ChooseNextPhase();
			ExecuteCurrentPhase();
		}
	}
}
