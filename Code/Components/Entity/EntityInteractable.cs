using Sandbox;

namespace SoulsBox
{
	public abstract class EntityInteractable : Component
	{
		[Property]
		public string Description {  get; set; }

		public abstract void Interact( AgentPlayer player );
	}
}
