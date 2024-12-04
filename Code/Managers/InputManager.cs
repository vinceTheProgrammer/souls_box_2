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

		public static void UpdateAnalogMove(AgentPlayer player)
		{
			player.MoveVector = Input.AnalogMove;
		}

		protected override void OnUpdate()
		{
			if ( Network.IsProxy ) return;
			if ( Player.IsUsingBonfire )
			{
				if ( Input.EscapePressed || (Input.UsingController && Input.Pressed("sb_jump")))
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

			if (Input.Pressed("sb_use") && !Player.MenuEnabled)
			{
				Player.PlayerInteraction.TryInteraction();
			}

			if (  Player.CreationMode )
			{
				float mouseWheelDelta = 0f;
				if (Input.UsingController)
				{
					if (Input.Down("sb_creation_mode_speed_inc"))
					{
						mouseWheelDelta = 1f;
					} else if (Input.Down("sb_creation_mode_speed_dec"))
					{
						mouseWheelDelta = -1f;
					} else
					{
						mouseWheelDelta = 0f;
					}
				} else
				{
					mouseWheelDelta = Input.MouseWheel.y;
				}
				Player.CharacterMovementController.CreationModeSpeed = (Player.CharacterMovementController.CreationModeSpeed + mouseWheelDelta * 10).Clamp( 0.1f, 10000f );
			}

			if (  Input.Pressed( "sb_creation_mode" ))
			{
				Player.ToggleCreationMode();
			}

			if ( Input.Pressed( "sb_lock_on" ) )
			{
				Player.ToggleLockOn();
			}

			if (Input.Pressed("sb_switch_item"))
			{
				Player.CharacterEquipment.CycleSelectedUsable();
			}

			if ( !Player.CharacterMovementController.CharacterController.IsOnGround ) return;

			if ( !Player.CharacterAnimationController.IsTagActive( "SB_Full_Attack" ) )
			{
				if ( Input.Down( "sb_sprint" ) && !Player.IsGuarding )
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
				if ( Input.Pressed( "sb_jump" ) && TimeSinceSprint < JumpThreshold && !Player.IsGuarding )
				{
					Player.IsJumping = true;
				}
			}
			

			if ( Input.Pressed( "sb_light_attack" ) && !Player.IsGuarding )
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

			if (Input.Down("sb_guard") && !Input.Down( "sb_sprint" ) )
			{
				Player.IsGuarding = true;
			} else if (Input.Released("sb_guard") )
			{
				Player.IsGuarding = false;
			}
		}
	}
}
