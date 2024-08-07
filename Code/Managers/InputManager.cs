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
		public CharacterAgent Agent { get; set; }

		public event EventHandler OnJump;
		public event EventHandler OnToggleLockOn;
		public event EventHandler OnBackstep;
		public event EventHandler<bool> OnSprint;
		public event EventHandler OnRoll;

		public event EventHandler<OnSwitchTargetEventArgs> OnSwitchTarget;
		
		public event EventHandler<OnAttackEventArgs> OnAttack;
		public class OnSwitchTargetEventArgs : EventArgs
		{
			public bool isLeft;
		}
		public class OnAttackEventArgs : EventArgs
		{
			public AttackControl attackControl;
		}

		private float InputHoldTime = 0f;
		private float TimeSinceSprint = 0f;
		private const float SprintThreshold = 0.5f;
		private const float JumpThreshold = 0.1f;

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
					OnSprint?.Invoke( this, true );
				}
			}
			else
			{
				if ( InputHoldTime > 0 && InputHoldTime < SprintThreshold )
				{
					if ( Agent.GetMoveVector().Length > 0 )
					{
						OnRoll?.Invoke( this, EventArgs.Empty );
					}
					else
					{
						OnBackstep?.Invoke( this, EventArgs.Empty );
					}
				}

				OnSprint?.Invoke( this, false );
				InputHoldTime = 0;
				TimeSinceSprint += Time.Delta;
			}
			if ( Input.Pressed( "sb_jump" ) && TimeSinceSprint < JumpThreshold )
			{
				OnJump?.Invoke( this, EventArgs.Empty );
			}

			if ( Input.Pressed( "sb_lock_on" ) )
			{
				OnToggleLockOn?.Invoke( this, EventArgs.Empty );
			}

			if ( Input.AnalogLook.yaw > 0.5 && Agent.LockedOn )
			{
				OnSwitchTarget?.Invoke( this, new OnSwitchTargetEventArgs { isLeft = true } );

			}
			else if ( Input.AnalogLook.yaw < -0.5 && Agent.LockedOn )
			{
				OnSwitchTarget?.Invoke( this, new OnSwitchTargetEventArgs { isLeft = false } );

			}

			if ( Input.Pressed( "sb_light_attack" ) )
			{
				if ( !Agent.IsLightAttacking )
				{
					OnAttack?.Invoke( this, new OnAttackEventArgs { attackControl = AttackControl.RightLightAttack } );
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
