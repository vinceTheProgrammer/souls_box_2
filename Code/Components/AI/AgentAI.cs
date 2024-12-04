using Sandbox;
using System;
using System.Numerics;

namespace SoulsBox
{
	/// <summary>
	/// Interface between Souls Box character and an AI agent.
	/// </summary>
	[Title( "Souls Box AI Agent" )]
	[Category( "Souls Box" )]
	[Icon( "man" )]
	public sealed class AgentAI : CharacterAgent
	{
		private TimeSince TimeSinceLastAttack {  get; set; }
		private float MinAttackPeriod { get; set; } = 0.5f;

		private float MaxAttackPeriod { get; set; } = 3f;

		private float AttackPeriod { get; set; } = 3f;

		private bool Block { get; set; }
		private bool Attack { get; set; }

		private TimeSince BlockStarted { get; set; }

		private float BlockLength { get; set; }

		private float MinBlockLength { get; set; } = 0.5f;

		private float MaxBlockLength { get; set; } = 5f;


		private float MoveSpeed { get; set; } = 100f;

		private AgentPlayer CurrentTarget { get; set; }

		private TimeUntil NextPhaseTime { get; set; }

		private Phase CurrentPhase { get; set; }

		private float AttackRange { get; set; } = 50f;

		private enum Phase
		{
			Idle,
			Charge,
			Back,
			Circle,
			Random,
			Attack
		}

		private AgentPlayer GetRandomPlayer()
		{
			var players = Scene.Components.GetAll<AgentPlayer>();
			Random random = new Random();
			if ( players.Count() == 0 ) return null;
			return players.ElementAt( random.Next( players.Count() - 1) );
		}

		private Vector3 GetToTargetVectorNormal()
		{
			return GetToTargetVector().Normal;
		}

		private Vector3 GetToTargetVector()
		{
			if ( CurrentTarget == null ) return Vector3.Zero;
			return (CurrentTarget.Transform.Position - Transform.Position);
		}

		private float GetDistanceToTarget()
		{
			return GetToTargetVector().Length;
		}

		private void ChooseRandomNonAttackPhase()
		{
			Phase NextPhase = GetRandomNonAttackPhase();
			NextPhaseTime = GetRandomTimeUntil( NextPhase );
			CurrentTarget = GetRandomPlayer();
			CurrentPhase = NextPhase;
		}

		private Vector3 GetRandomMoveVector()
		{
			Random random = new Random();
			float _speed = random.Next();
			float _radius = random.Next();
			float time = Time.Now;
			float angle = time * _speed;
			float x = _radius * MathF.Cos( angle );
			float z = _radius * MathF.Sin( angle );
			return new Vector3( x, z, 0 );
		}

		private Vector3 GetCircleMoveVector()
		{
			float _speed = 0.5f;
			float _radius = 1.0f;
			float time = Time.Now;
			float angle = time * _speed;
			float x = _radius * MathF.Cos( angle );
			float z = _radius * MathF.Sin( angle );
			return new Vector3( x, z, 0 );
		}

		private static float GetRandomTimeUntil( Phase nextPhase )
		{

			float min = GetPhaseMinTime( nextPhase );
			float max = GetPhaseMaxTime( nextPhase );

			Random random = new Random();

			int minMilliseconds = (int)TimeSpan.FromSeconds( min ).TotalMilliseconds;
			int maxMilliseconds = (int)TimeSpan.FromSeconds( max ).TotalMilliseconds;

			int randomMilliseconds = random.Next( minMilliseconds, maxMilliseconds );

			return TimeSpan.FromMilliseconds( randomMilliseconds ).Seconds;
		}

		private static Phase GetRandomPhase()
		{
			Random random = new Random();

			Array values = Enum.GetValues( typeof( Phase ) );

			return (Phase)values.GetValue( random.Next( values.Length ) );
		}

		private static Phase GetRandomNonAttackPhase()
		{
			Random random = new Random();

			Array values = Enum.GetValues( typeof( Phase ) );

			return (Phase)values.GetValue( random.Next( values.Length - 2 ) );
		}

		private static float GetPhaseMinTime( Phase phase )
		{
			switch ( phase )
			{
				case Phase.Idle:
					return 1;
				case Phase.Attack:
					return 2;
				case Phase.Charge:
					return 3;
				default:
					return 2;
			}
		}

		private static float GetPhaseMaxTime( Phase phase )
		{
			switch ( phase )
			{
				case Phase.Idle:
					return 1;
				case Phase.Attack:
					return 8;
				case Phase.Charge:
					return 10;
				default:
					return 2;
			}
		}
		private void ExecuteCurrentPhase()
		{

			if ( CurrentTarget == null ) return;
			switch ( CurrentPhase )
			{
				case Phase.Idle:
					IdlePhase();
					break;
				case Phase.Attack:
					AttackPhase();
					break;
				case Phase.Charge:
					ChargePhase();
					break;
				case Phase.Circle:
					CirclePhase();
					break;
				case Phase.Random:
					RandomPhase();
					break;
				case Phase.Back:
					BackPhase();
					break;
				default:
					IdlePhase();
					break;
			}
		}

		private void IdlePhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVectorNormal().EulerAngles.yaw );
		}

		private void ChargePhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVectorNormal().EulerAngles.yaw );
			MoveVector = GetToTargetVectorNormal() * MoveSpeed;
		}

		private void CirclePhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVectorNormal().EulerAngles.yaw );
			MoveVector = GetCircleMoveVector().Normal * MoveSpeed;
		}

		private void BackPhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVectorNormal().EulerAngles.yaw );
			MoveVector = -(GetToTargetVectorNormal()) * MoveSpeed;
		}

		private void RandomPhase()
		{
			Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVectorNormal().EulerAngles.yaw );
			MoveVector = GetRandomMoveVector().Normal * MoveSpeed;
		}

		private void SetRandomAttackPeriod()
		{
			Random random = new Random();

			int minMilliseconds = (int)TimeSpan.FromSeconds( MinAttackPeriod ).TotalMilliseconds;
			int maxMilliseconds = (int)TimeSpan.FromSeconds( MaxAttackPeriod ).TotalMilliseconds;

			int randomMilliseconds = random.Next( minMilliseconds, maxMilliseconds );

			AttackPeriod = TimeSpan.FromMilliseconds( randomMilliseconds ).Seconds;
		}

		private void SetRandomBlockLength()
		{
			Random random = new Random();

			int minMilliseconds = (int)TimeSpan.FromSeconds( MinBlockLength ).TotalMilliseconds;
			int maxMilliseconds = (int)TimeSpan.FromSeconds( MinBlockLength ).TotalMilliseconds;

			int randomMilliseconds = random.Next( minMilliseconds, maxMilliseconds );

			BlockLength = TimeSpan.FromMilliseconds( randomMilliseconds ).Seconds;
		}

		private void AttackPhase()
		{
			/*
			Random random = new Random();
			if (random.Next(0, 10) > 1 && !Attack && !Block)
			{
				SetRandomBlockLength();
				BlockStarted = 0;
				IsGuarding = true;
				Block = true;
			}

			if (Block && BlockStarted > BlockLength)
			{
				IsGuarding = false;
				Block = false;
			}
			*/


			if ( TimeSinceLastAttack > AttackPeriod && !Block )
			{
				if ( !CharacterAnimationController.IsTagActive( "SB_Attacking" ) ) Transform.Rotation = Transform.Rotation.Angles().WithYaw( GetToTargetVectorNormal().EulerAngles.yaw );
				if ( CharacterVitals.Stamina > 0 )
				{
					if ( !CharacterAnimationController.IsTagActive( "SB_Attacking" ) && !CharacterMovementController.CharacterAnimationController.IsTagActive( "SB_Doing_Animation" ) )
					{
						IsLightAttacking = true;
					}
					else if ( CharacterAnimationController.IsTagActive( "SB_Can_Continue" ) )
					{
						IsContinuing = true;
					}
				}
				TimeSinceLastAttack = 0;
				Attack = true;
			}
			else
			{
				Attack = false;
			}
		}

		protected override void OnFixedUpdate()
		{
			if ( Network.IsProxy ) return;
			if ( NextPhaseTime )
			{
				CurrentTarget = GetRandomPlayer();
				SetRandomAttackPeriod();
			}
			if ( CurrentTarget == null ) return;
			if ( GetDistanceToTarget() < AttackRange ) CurrentPhase = Phase.Attack;
			else
			{
				if ( NextPhaseTime ) ChooseRandomNonAttackPhase();
			}
			ExecuteCurrentPhase();
		}
	}
}
