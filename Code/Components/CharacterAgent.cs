using Sandbox;

namespace SoulsBox
{
	public abstract class CharacterAgent : Component
	{
		public abstract CameraController CameraController { get; set; }

		public bool IsRolling { get; set; }

		public bool IsJumping { get; set; }

		public bool IsSprinting { get; set; }

		public bool IsBackstepping { get; set; }

		public bool IsLightAttacking { get; set; }

		public bool LockedOn { get; set; }

		public LockOnAble CurrentLockOnAble { get; set; }

		public HashSet<LockOnAble> LockOnAbles = new HashSet<LockOnAble>();

		public float LockOnRadius = 1000f;

		public abstract bool IsRunActive();

		public abstract bool IsGuardActive();

		public abstract Vector3 GetMoveVector();
	}
}
