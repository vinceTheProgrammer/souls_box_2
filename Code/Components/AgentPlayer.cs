using Sandbox;
using Sandbox.Citizen;
using Sandbox.Diagnostics;
using System;

namespace SoulsBox
{
	/// <summary>
	/// Interface between Souls Box character and an IRL human agent.
	/// </summary>
	[Title( "Souls Box Player Agent" )]
	[Category( "Souls Box" )]
	[Icon( "man" )]
	public sealed class AgentPlayer : CharacterAgent
	{

		[Property]
		public override CameraController CameraController { get; set; }

		[Property]
		public GameObject Camera { get; set; }

		private Rotation LastMoveDirectionRotation;
		private float inputHoldTime = 0f;
		private float timeSinceSprint = 0f;
		private const float sprintThreshold = 0.5f;
		private const float jumpThreshold = 0.1f;


		public void UpdateLockOnAbles()
		{
			lockOnAbles.Clear();
			OctreeManager.Instance.Clear();

			IEnumerable<LockOnAble> allLockOnAbles = Scene.GetAllComponents<LockOnAble>();
			foreach ( var lockOnAble in allLockOnAbles )
			{
				OctreeManager.Instance.Insert( lockOnAble );
			}

			var lockOnAblesInRange = OctreeManager.Instance.QueryRange( Transform.Position, LockOnRadius );
			foreach ( var lockOnAble in lockOnAblesInRange )
			{
				if ( lockOnAble.ParentIsAlive() )
				{
					lockOnAbles.Add( lockOnAble );
				}
			}
		}

		private LockOnAble GetClosestLockOnAble()
		{
			LockOnAble closest = null;
			float closestDistance = float.MaxValue;

			foreach ( var lockOnAble in lockOnAbles )
			{
				float distance = Vector3.DistanceBetween( Transform.Position, lockOnAble.Transform.Position );
				if ( distance < closestDistance )
				{
					closest = lockOnAble;
					closestDistance = distance;
				}
			}

			return closest;
		}

		private LockOnAble GetClosestLockOnAbleInView()
		{
			LockOnAble closest = null;
			float closestDistance = float.MaxValue;

			foreach ( var lockOnAble in lockOnAbles )
			{
				float distance = Vector3.DistanceBetween( Transform.Position, lockOnAble.Transform.Position );
				if ( distance < closestDistance && IsWithinView( lockOnAble.Transform ) )
				{
					closest = lockOnAble;
					closestDistance = distance;
				}
			}

			return closest;
		}

		public void SwitchTarget( bool isLeft )
		{
			if ( currentLockOnAble == null ) return;

			LockOnAble bestTarget = null;
			float bestAngle = float.MaxValue;

			foreach ( var lockOnAble in lockOnAbles )
			{
				if ( lockOnAble == currentLockOnAble ) continue;

				Vector3 toTarget = lockOnAble.Transform.Position - Transform.Position;
				Vector3 toCurrentTarget = currentLockOnAble.Transform.Position - Transform.Position;

				float angle = SignedAngle( toCurrentTarget, toTarget);
				if ( isLeft && angle < 0 && angle > -bestAngle )
				{
					bestAngle = -angle;
					bestTarget = lockOnAble;
				}
				else if ( !isLeft && angle > 0 && angle < bestAngle )
				{
					bestAngle = angle;
					bestTarget = lockOnAble;
				}
			}

			if ( bestTarget != null )
			{
				currentLockOnAble = bestTarget;
			}
		}

		public override Vector3 GetMoveVector()
		{
			return Input.AnalogMove * Camera.Transform.Rotation;
		}

		public override bool IsGuardActive()
		{
			return Input.Down( "Guard" );
		}

		public override bool IsRunActive()
		{
			return isSprinting;
		}

		protected override void OnUpdate()
		{
			if (!lockedOn)
			{
				CameraController.ForwardAngles += Input.AnalogLook;
				float _tempVarPointDistance = 100.0f;
				Vector3 _tempPointVector = new Vector3( Transform.Position.WithZ( Transform.Position.z + 65.0f ) + Transform.Rotation.Forward.Normal * _tempVarPointDistance );
				SceneTraceResult camToPointTraceResult = Scene.Trace.Ray( Camera.Transform.Position, _tempPointVector ).Size( 1f ).WithoutTags( "player" ).Run();
				SceneTraceResult playerToPointTraceResult = Scene.Trace.Ray( Transform.Position.WithZ( Transform.Position.z + 65.0f ), _tempPointVector ).Size( 1f ).WithoutTags( "player" ).Run();

				bool playerFacingRight = _tempPointVector.ToScreen().x > 0.5f; // TODO ToScreen is obsolete apparently

				if ( camToPointTraceResult.Hit && !playerToPointTraceResult.Hit )
				{
					float incrementAmount = playerFacingRight ? -1f : 1f;
					CameraController.ForwardAngles.yaw += incrementAmount;
				}
			}
			OctreeManager.Instance.Draw();
		}

		public void toggleLockOn()
		{
			bool previousState = lockedOn;
			lockedOn = !lockedOn;
			if ( previousState == false ) 
			{
				currentLockOnAble = GetClosestLockOnAble();
			}
		}

		private bool IsWithinView(GameTransform gameTransform)
		{
			float leftBound = 0f;
			float rightBound = 1f;
			float topBound = 0f;
			float bottomBound = 1f;
			Vector3 screenCoords = gameTransform.Position.ToScreen();
			return screenCoords.x >= leftBound && screenCoords.x <= rightBound && screenCoords.y >= topBound && screenCoords.y <= bottomBound;
		}


		// move somewhere else later lol
		public static float SignedAngle( Vector3 from, Vector3 to)
		{
			float angle = Vector3.GetAngle( from, to );
			float crossY = from.x * to.z - from.z * to.x;
			return crossY > 0 ? angle : -angle;
		}

		protected override void OnFixedUpdate()
		{
			UpdateLockOnAbles();
			//Log.Info( String.Join( ", ", lockOnAbles ) );
			if (currentLockOnAble != null )
			{
				Log.Info( IsWithinView(currentLockOnAble.Transform) );
			}
			
			/*
			LockOnAble closestLockOnAble = GetClosestLockOnAble();
			if (closestLockOnAble != null )
			{
				lockedOn = true;
				currentLockOnAble = closestLockOnAble;
			} else
			{
				lockedOn = false;
			}
			*/


			if ( isRolling || isJumping )
			{
				inputHoldTime = 0f; // Reset the input hold time when rolling or jumping
				return;
			}

			if ( !isRolling && !isJumping && !(lockedOn && !isSprinting) )
			{
				if ( GetMoveVector().Length > 0 ) LastMoveDirectionRotation = Rotation.FromYaw( (GetMoveVector()).EulerAngles.yaw );
				Transform.Rotation = Rotation.Lerp( Transform.Rotation, LastMoveDirectionRotation, 0.1f );
			} else if (!isRolling && !isJumping)
			{
				Vector3 targetToPlayerDisplacement = (currentLockOnAble.Transform.Position - Transform.Position);
				Rotation faceDirection = Rotation.FromYaw( targetToPlayerDisplacement.Normal.EulerAngles.yaw );
				Transform.Rotation = Rotation.Lerp( Transform.Rotation, faceDirection, 0.5f );
			}

			if ( Input.Down( "sb_sprint" ) )
			{
				inputHoldTime += Time.Delta;

				if ( inputHoldTime >= sprintThreshold )
				{
					timeSinceSprint = 0;
					isSprinting = true;
				}
			}
			else
			{
				if ( inputHoldTime > 0 && inputHoldTime < sprintThreshold )
				{
					if ( GetMoveVector().Length > 0 )
					{
						// Rolling
						isRolling = true;
					}
					else
					{
						// Backstepping
						//Log.Info( "Backstep" );
						isBackstepping = true;
					}
				}

				isSprinting = false;
				inputHoldTime = 0;
				timeSinceSprint += Time.Delta;
			}
			if ( Input.Pressed( "sb_jump" ) && timeSinceSprint < jumpThreshold )
			{
				isJumping = true;
			}

			if (Input.Pressed("sb_lock_on"))
			{
				toggleLockOn();
			}

			if (Input.AnalogLook.yaw > 0.5 && lockedOn)
			{
				Log.Info( "left!" );
				SwitchTarget( true );
			} else if (Input.AnalogLook.yaw < -0.5 && lockedOn )
			{
				Log.Info( "right!" );
				SwitchTarget( false );
			}

			if (Input.Pressed("sb_light_attack"))
			{
				if (!isLightAttacking)
				{
					isLightAttacking = true;
				} else
				{
					CitizenAnimationHelper AnimationHelper = GameObject.Components.Get<CitizenAnimationHelper>();
					if ( AnimationHelper != null )
					{
						AnimationHelper.Target.Set( "sb_continue_combo", true );
					}
				}	
			}
		}

		protected override void OnStart()
		{

			OctreeManager.Instance.DestroyAndReInit();
		}
	}
}

