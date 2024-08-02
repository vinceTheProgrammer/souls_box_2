using Sandbox;

namespace SoulsBox
{
	public abstract class CharacterAgent : Component
	{
		public abstract CameraController CameraController { get; set; }

		public bool isRolling { get; set; }

		public bool isJumping { get; set; }

		public bool isSprinting { get; set; }

		public bool isBackstepping { get; set; }

		public bool lockedOn { get; set; }

		public Vector3 lockedOnPosition { get; set; }

		public abstract bool IsRunActive();

		public abstract bool IsGuardActive();

		public abstract Vector3 GetMoveVector();
	}
}
