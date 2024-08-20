namespace SoulsBox;

public sealed class Bonfire : EntityInteractable
{
	public bool Lit {  get; private set; }
	private SoundHandle AmbientSoundHandle { get; set; }

	const string UnlitInteractDescription = "Light bonfire";
	const string LitInteractDescription = "Rest at bonfire";

	public void Light()
	{
		Lit = true;
		IEnumerable<PointLight> pointLights = GameObject.Components.GetAll<PointLight>(find: FindMode.EverythingInSelfAndDescendants );
		foreach (PointLight pointLight in pointLights)
		{
			pointLight.GameObject.Enabled = true;
		}
		IEnumerable<ParticleEffect> particleEffects = GameObject.Components.GetAll<ParticleEffect>( find: FindMode.EverythingInSelfAndDescendants );
		foreach ( ParticleEffect particleEffect in particleEffects )
		{
			particleEffect.GameObject.Enabled = true;
		}
		Sound.Play( "lighting-a-fire", Transform.Position);
		AmbientSoundHandle = Sound.Play( "fire-place", Transform.Position );
		Description = LitInteractDescription;
	}

	public void Extinguish()
	{
		Lit = false;
		IEnumerable<PointLight> pointLights = GameObject.Components.GetAll<PointLight>( find: FindMode.EverythingInSelfAndDescendants );
		foreach ( PointLight pointLight in pointLights )
		{
			pointLight.GameObject.Enabled = false;
		}
		IEnumerable<ParticleEffect> particleEffects = GameObject.Components.GetAll<ParticleEffect>( find: FindMode.EverythingInSelfAndDescendants );
		foreach ( ParticleEffect particleEffect in particleEffects )
		{
			particleEffect.GameObject.Enabled = false;
		}
		AmbientSoundHandle.Stop();
		Description = UnlitInteractDescription;
	}

	protected override void OnStart()
	{
		Extinguish();
	}

	public override void Interact( AgentPlayer player )
	{
		if ( Network.IsProxy ) return;
		if (Lit)
		{
			player.ToggleBonfireRest( this );
		}
		else
		{
			Light();
			player.IsLightingBonfire = true;
		}
	}
}
