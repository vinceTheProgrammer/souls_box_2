using Sandbox;
using System;

namespace SoulsBox
{
	/// <summary>
	/// Interface between Souls Box character and an AI agent.
	/// </summary>
	[Title( "Souls Box AI Agent" )]
	[Category( "Souls Box" )]
	[Icon( "man" )]
	public sealed class AgentAI : CharacterAgent
	{
		private Rotation LastMoveDirectionRotation;

		private void SetMoveVector()
		{
			float _speed = 0.5f;
			float _radius = 1.0f;

			float time = Time.Now;
			// Calculate the angle based on time and speed
			float angle = time * _speed;

			// Calculate the x and z coordinates using trigonometric functions
			float x = _radius * MathF.Cos( angle );
			float z = _radius * MathF.Sin( angle );

			// Return the position as a Vector3 (assuming y is constant)
			MoveVector = new Vector3( x, z, 0 );
		}

		protected override void OnUpdate()
		{
			SetMoveVector();
			if ( MoveVector.Length > 0 ) LastMoveDirectionRotation = Rotation.FromYaw( (MoveVector).EulerAngles.yaw );
			Transform.Rotation = Rotation.Lerp( Transform.Rotation, LastMoveDirectionRotation, 0.1f );
		}
	}
}