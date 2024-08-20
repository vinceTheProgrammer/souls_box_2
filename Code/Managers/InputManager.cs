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
		public AgentPlayer Player { get; set; }

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

		protected override void OnUpdate()
		{
			if ( Network.IsProxy ) return;
			if ( Player.IsUsingBonfire )
			{
				if ( Input.EscapePressed )
				{
					Input.EscapePressed = false;
					Player.ToggleBonfireRest();
				}
				return;
			}

			if (Input.EscapePressed)
			{
				Input.EscapePressed = false;
				Player.ToggleMenu();
			}

			if ( Player.IsRolling || Player.IsJumping )
			{
				InputHoldTime = 0f; // Reset the input hold time when rolling or jumping
				return;
			}

			if (Input.Pressed("sb_use"))
			{
				Player.PlayerInteraction.TryInteraction();
			}

			if (  Player.CreationMode )
			{
				Player.CharacterMovementController.CreationModeSpeed = (Player.CharacterMovementController.CreationModeSpeed + Input.MouseWheel.y * 10).Clamp( 0.1f, 10000f );
			}

			if (  Input.Pressed( "sb_creation_mode" ))
			{
				Player.ToggleCreationMode();
			}

			if ( Input.Pressed( "sb_lock_on" ) )
			{
				Player.ToggleLockOn();
			}

			if ( Input.UsingController )
			{
				float yaw = Input.AnalogLook.yaw;

				if ( Player.LockedOn )
				{
					if ( stickReleased && yaw > 0.5f )
					{
						Player.SwitchTarget( true );
						stickReleased = false;
					}
					else if ( stickReleased && yaw < -0.5f )
					{
						Player.SwitchTarget( false );
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

				if ( Player.LockedOn )
				{
					float deltaX = mouseX - lastMouseX;

					if ( deltaX > mouseThreshold )
					{
						Player.SwitchTarget( false );
						lastMouseX = mouseX;
					}
					else if ( deltaX < -mouseThreshold )
					{
						Player.SwitchTarget( true );
						lastMouseX = mouseX;
					}
				}
			}

			if ( !Player.CharacterMovementController.CharacterController.IsOnGround ) return;

			if ( Input.Down( "sb_sprint" ) )
			{
				InputHoldTime += Time.Delta;

				if ( InputHoldTime >= SprintThreshold )
				{
					TimeSinceSprint = 0;
					Player.IsSprinting = true;
				}
			}
			else
			{
				if ( InputHoldTime > 0 && InputHoldTime < SprintThreshold )
				{
					if ( Player.MoveVector.Length > 0 )
					{
						Player.IsRolling = true;
					}
					else
					{
						Player.IsBackstepping = true;
					}
				}

				Player.IsSprinting = false;
				InputHoldTime = 0;
				TimeSinceSprint += Time.Delta;
			}
			if ( Input.Pressed( "sb_jump" ) && TimeSinceSprint < JumpThreshold )
			{
				Player.IsJumping = true;
			}

			if ( Input.Pressed( "sb_light_attack" ) )
			{
				if ( Player.CharacterVitals.Stamina > 0 )
				{
					if ( !Player.CharacterAnimationController.IsTagActive( "SB_Attacking" ) && !Player.CharacterMovementController.CharacterAnimationController.IsTagActive( "SB_Doing_Animation" ) )
					{
						Player.IsLightAttacking = true;
					}
					else if ( Player.CharacterAnimationController.IsTagActive( "SB_Can_Continue" ) )
					{
						Player.IsContinuing = true;
					}
				}
			}

			if (Input.Down("sb_guard") )
			{
				Player.IsGuarding = true;
			} else if (Input.Released("sb_guard") )
			{
				Player.IsGuarding = false;
			}
		}
	}
}
