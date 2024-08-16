namespace Sandbox;

public sealed class Bonfire : Component
{
	public bool Lit {  get; private set; }
	private SoundHandle AmbientSoundHandle { get; set; }

	public void Light()
	{
		Lit = true;
		IEnumerable<PointLight> pointLights = GameObject.Components.GetAll<PointLight>(find: FindMode.EnabledInSelfAndDescendants);
		foreach (PointLight pointLight in pointLights)
		{
			pointLight.GameObject.Enabled = true;
		}
		IEnumerable<ParticleEffect> particleEffects = GameObject.Components.GetAll<ParticleEffect>( find: FindMode.EnabledInSelfAndDescendants );
		foreach ( ParticleEffect particleEffect in particleEffects )
		{
			particleEffect.GameObject.Enabled = true;
		}
		Sound.Play( "lighting-a-fire", Transform.Position);
		AmbientSoundHandle = Sound.Play( "fire-place", Transform.Position );
	}

	public void Extinguish()
	{
		Lit = false;
		IEnumerable<PointLight> pointLights = GameObject.Components.GetAll<PointLight>( find: FindMode.EnabledInSelfAndDescendants );
		foreach ( PointLight pointLight in pointLights )
		{
			pointLight.GameObject.Enabled = false;
		}
		IEnumerable<ParticleEffect> particleEffects = GameObject.Components.GetAll<ParticleEffect>( find: FindMode.EnabledInSelfAndDescendants );
		foreach ( ParticleEffect particleEffect in particleEffects )
		{
			particleEffect.GameObject.Enabled = false;
		}
		AmbientSoundHandle.Stop();
	}
}
