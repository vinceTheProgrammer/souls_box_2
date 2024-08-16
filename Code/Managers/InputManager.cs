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
		private const float mouseThreshold = 300f;

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

			if (  Agent.CreationMode )
			{
				Agent.CharacterMovementController.CreationModeSpeed = (Agent.CharacterMovementController.CreationModeSpeed + Input.MouseWheel.y * 10).Clamp( 0.1f, 10000f );
			}

			if (  Input.Pressed( "sb_creation_mode" ))
			{
				Agent.ToggleCreationMode();
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
				float mouseX = Mouse.Position.x;

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
				if ( Agent.CharacterVitals.Stamina > 0 )
				{
					if ( !Agent.CharacterAnimationController.IsTagActive( "SB_Attacking" ) && !Agent.CharacterMovementController.CharacterAnimationController.IsTagActive( "SB_Doing_Animation" ) )
					{
						Agent.IsLightAttacking = true;
					}
					else if ( Agent.CharacterAnimationController.IsTagActive( "SB_Can_Continue" ) )
					{
						Agent.IsContinuing = true;
					}
				}
			}

			if (Input.Down("sb_guard") )
			{
				Agent.IsGuarding = true;
			} else if (Input.Released("sb_guard") )
			{
				Agent.IsGuarding = false;
			}
		}
	}
}
