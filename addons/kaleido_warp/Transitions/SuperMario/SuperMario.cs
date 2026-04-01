using Godot;
namespace KaleidoWarp;

public partial class SuperMario : Transition
{
	const string ShaderPath = "super_mario.gdshader";

	[Export]
	public float Speed { get; set; } = 50f;

	public override void _Ready()
	{
		base._Ready();

		var shader = LoadLocal<Shader>(ShaderPath);
		Material = new ShaderMaterial
		{
			Shader = shader
		};
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		var material = (ShaderMaterial)Material;
		material.SetShaderParameter("progress", Reverse ? 1f - Progress : Progress);
		material.SetShaderParameter("image", ImageTexture ?? TransparentPixel);
		material.SetShaderParameter("image_fit", (int)ImageFitMode);
		material.SetShaderParameter("speed", Speed);
	}

	public static SuperMario Cover(float duration = 2f)
	{
		return new SuperMario()
		{
			Duration = duration,
			Reverse = false,
			Ease = Tween.EaseType.InOut,
		};
	}

	public static SuperMario Uncover(float duration = 2f)
	{
		return new SuperMario()
		{
			Duration = duration,
			Reverse = true,
			Ease = Tween.EaseType.Out,
		};
	}
}
