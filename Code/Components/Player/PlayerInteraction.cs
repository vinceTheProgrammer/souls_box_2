using Sandbox;
using System;
using System.Security.Cryptography;

namespace SoulsBox
{
	/// <summary>
	/// Souls box player interaction system
	/// </summary>
	[Title( "Souls Box Player Interaction" )]
	[Category( "Souls Box Player" )]
	[Icon( "casino" )]
	public sealed class PlayerInteraction : Component
	{

		[Property]
		public Vector3 Size {  get; set; }
		[Property]
		public Vector3 Center { get; set; }

		[Property]
		AgentPlayer Player { get; set; }

		BoxCollider Collider { get; set; }
		EntityInteractable FocusedInteractable { get; set; }

		protected override void OnUpdate()
		{
			if (FocusedInteractable != null)
			{
				string interactionText = $"{GetGlyph()}: {FocusedInteractable.Description}";
				Gizmo.Draw.ScreenText(interactionText,	new Vector2(Screen.Width / 2 - (interactionText.Count() * 2), Screen.Height - 20));
			}
		}

		protected override void OnFixedUpdate()
		{
			if ( Network.IsProxy ) return;
			if ( Collider == null || !Collider.Touching.Any() ) return;
			EntityInteractable closestInteractable = Collider.Touching
			.Select( element => element.GameObject.Components.Get<EntityInteractable>() )
			.Where( component => component != null )
			.Select( interactable =>
			{
				Vector3 playerToInteractable = interactable.Transform.Position - Transform.Position;
				float angleToInteractable = MathF.Abs( playerToInteractable.SignedAngle( Transform.Rotation.Forward ) );
				return new { interactable, angleToInteractable };
			} )
			.Where( x => x.angleToInteractable <= 45f )
			.MinBy( x => x.angleToInteractable )?.interactable;
			FocusedInteractable = closestInteractable;
		}

		protected override void OnStart()
		{
			Collider = GameObject.Components.GetOrCreate<BoxCollider>();
			Collider.Center = Center;
			Collider.Scale = Size;
			Collider.IsTrigger = true;
		}

		private string GetGlyph()
		{
			return Input.GetButtonOrigin( "sb_use" );
		}

		public void TryInteraction()
		{
			if ( FocusedInteractable != null )
			{
				FocusedInteractable.Interact( Player );
			}
		}
	}
}
