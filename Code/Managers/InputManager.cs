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
		private bool stickReleased = true;
		private float lastMouseX = 0f;
		private const float mouseThreshold = 15f;

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

			if ( Input.UsingController )
			{
				float yaw = Input.AnalogLook.yaw;

				if ( Agent.LockedOn )
				{
					if ( stickReleased && yaw > 0.5f )
					{
						Agent.SwitchTarget( true );
						stickReleased = false;
					}
					else if ( stickReleased && yaw < -0.5f )
					{
						Agent.SwitchTarget( false );
						stickReleased = false;
					}

					// Check if the stick is released
					if ( yaw > -0.1f && yaw < 0.1f )
					{
						stickReleased = true;
					}
				}
			}
			else // Using mouse
			{
				float mouseX = Input.AnalogLook.yaw;

				if ( Agent.LockedOn )
				{
					float deltaX = mouseX - lastMouseX;

					if ( deltaX > mouseThreshold )
					{
						Agent.SwitchTarget( false );
						lastMouseX = mouseX;
					}
					else if ( deltaX < -mouseThreshold )
					{
						Agent.SwitchTarget( true );
						lastMouseX = mouseX;
					}
				}
			}

			if ( Input.Pressed( "sb_light_attack" ) )
			{
				if ( !Agent.IsLightAttacking )
				{
					Agent.IsLightAttacking = true;
				}
				else
				{
					Agent.IsContinuing = true;
				}
			}
		}
	}
}
