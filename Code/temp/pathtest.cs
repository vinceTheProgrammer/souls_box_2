public class PathTest : Component {
	[Property, ReadOnly] public SkinnedModelRenderer Body {get; set;}

	protected override void OnEnabled() {
		Body = Components.Create<SkinnedModelRenderer>();
		Body.Model = Model.Load("models/citizen/citizen.vmdl");
		Body.AnimationGraph = AnimationGraph.Load("pathtest.vanmgrph");
		base.OnEnabled();
	}

	protected override void OnUpdate() {
		Transform.Position += Body.RootMotion.Position.RotateAround(Vector3.Zero, Transform.Rotation);
		Transform.Rotation = (Transform.Rotation.Angles() + Body.RootMotion.Rotation.Angles()).ToRotation();
		base.OnUpdate();
	}
}
