using Sandbox;
using Sandbox.Citizen;
using SoulsBox;
using System;
using static SoulsBox.CharacterCombatController;

namespace SoulsBox
{
	public sealed class InputManager : Component
	{

		[Property]
		public AgentPlayer Agent { get; set; }

		private float InputHoldTime = 0f;
		private float TimeSinceSprint = 0f;
		private const float SprintThreshold = 0.5f;
		private const float JumpThreshold = 0.1f;

		public static void UpdateAnalogMove(AgentPlayer player)
		{
			player.MoveVector = Input.AnalogMove;
		}

		protected override void OnFixedUpdate()
		{
			if ( Network.IsProxy ) return;

			if ( Agent.IsRolling || Agent.IsJumping )
			{
				InputHoldTime = 0f; // Reset the input hold time when rolling or jumping
				return;
			}

			if ( Input.Down( "sb_sprint" ) )
			{
				InputHoldTime += Time.Delta;

				if ( InputHoldTime >= SprintThreshold )
				{
					TimeSinceSprint = 0;
					Agent.IsSprinting = true;
				}
			}
			else
			{
				if ( InputHoldTime > 0 && InputHoldTime < SprintThreshold )
				{
					if ( Agent.MoveVector.Length > 0 )
					{
						Agent.IsRolling = true;
					}
					else
					{
						Agent.IsBackstepping = true;
					}
				}

				Agent.IsSprinting = false;
				InputHoldTime = 0;
				TimeSinceSprint += Time.Delta;
			}
			if ( Input.Pressed( "sb_jump" ) && TimeSinceSprint < JumpThreshold )
			{
				Agent.IsJumping = true;
			}

			if ( Input.Pressed( "sb_lock_on" ) )
			{
				Agent.ToggleLockOn();
			}

			if ( Input.AnalogLook.yaw > 0.5 && Agent.LockedOn )
			{
				Agent.SwitchTarget( true );

			}
			else if ( Input.AnalogLook.yaw < -0.5 && Agent.LockedOn )
			{
				Agent.SwitchTarget( false );

			}

			if ( Input.Pressed( "sb_light_attack" ) )
			{
				if ( !Agent.IsLightAttacking )
				{
					Agent.IsLightAttacking = true;
				}
				else
				{
					CitizenAnimationHelper AnimationHelper = GameObject.Components.Get<CitizenAnimationHelper>();
					if ( AnimationHelper != null )
					{
						AnimationHelper.Target.Set( "sb_continue_combo", true );
					}
				}
			}
		}
	}
}
