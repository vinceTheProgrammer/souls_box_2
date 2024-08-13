using Sandbox;

namespace SoulsBox
{
	public abstract class CharacterAgent : Component
	{
		[Property]
		public CameraController CameraController { get; set; }

		[Property]
		public CharacterMovementController CharacterMovementController { get; set; }

		[Property]
		public CharacterAnimationController CharacterAnimationController { get; set; }

		[Property]
		public CharacterCombatController CharacterCombatController { get; set; }

		[Property]
		public CharacterVitals CharacterVitals { get; set; }

		[Property]
		public CharacterStats CharacterStats { get; set; }

		[Sync]
		public bool IsRolling { get; set; }

		[Sync]
		public bool IsJumping { get; set; }

		[Sync]
		public bool IsSprinting { get; set; }

		[Sync]
		public bool IsBackstepping { get; set; }

		[Sync]
		public bool IsLightAttacking { get; set; }

		[Sync]
		public bool IsGuarding { get; set; }

		[Sync]
		public bool IsContinuing { get; set; }

		[Sync]
		public bool IsDead { get; set; }

		[Sync]
		public Vector3 MoveVector { get; set; }
	}
}
