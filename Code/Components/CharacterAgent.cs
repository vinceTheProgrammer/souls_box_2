using Sandbox;

namespace SoulsBox
{
	public abstract class CharacterAgent : Component
	{
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
		public Vector3 MoveVector { get; set; }
	}
}
