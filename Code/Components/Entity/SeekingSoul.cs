namespace SoulsBox;

public sealed class SeekingSoul : Component
{
	public AgentPlayer Target {  get; set; }

	protected override void OnFixedUpdate()
	{
		Vector3 targetPosition = Target.Transform.Position + new Vector3( 0, 0, 35f );
		if (Target != null) GameObject.Transform.Position = GameObject.Transform.Position.LerpTo( targetPosition, 0.1f );
		if ( Vector3.DistanceBetween( GameObject.Transform.Position, targetPosition) < 0.5f )
		{
			GameObject.Destroy();
		}
	}
}
