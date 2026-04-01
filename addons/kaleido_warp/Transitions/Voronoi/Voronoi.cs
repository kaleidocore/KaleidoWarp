using Godot;

namespace KaleidoWarp;

/// <summary>
/// Represents a transition effect that gradually covers or uncovers the screen using a Voronoi pattern.
/// </summary>
/// <remarks>Use the static methods <see cref="Cover(float)"/> and <see cref="Uncover(float)"/> to create
/// transitions for covering or uncovering the screen, respectively. The transition duration, easing, and other shader effects
/// can be further customized via chained calls to achieve various visual styles.
/// </remarks>
public partial class Voronoi : Transition
{
	const string ShaderPath = "voronoi.gdshader";

	/// <summary>
	/// Gets or sets the animation angle in degrees, typically between 0 and 360.
	/// </summary>
	[Export(PropertyHint.Range, "0,360")]
	public int Angle { get; set; } = 0;

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
		material.SetShaderParameter("progress", Reverse ? 1.0 - Progress : Progress);
		material.SetShaderParameter("image", ImageTexture ?? TransparentPixel);
		material.SetShaderParameter("image_fit", (int)ImageFitMode);
		material.SetShaderParameter("angle", Mathf.DegToRad(Angle));
	}

	/// <summary>
	/// Creates a Voronoi transition (exit/outro) that gradually covers the screen over the specified duration.
	/// </summary>
	/// <param name="duration">The duration, in seconds, for which the Voronoi effect will run. Defaults to 2 seconds if not specified.</param>
	/// <returns>A new instance of the Voronoi transition configured for covering the screen, ready for further configuration.</returns>
	public static Voronoi Cover(float duration = 2f)
	{
		return new Voronoi()
		{
			Duration = duration,
			Reverse = false,
			TransitionType = Tween.TransitionType.Sine,
		};
	}

	/// <summary>
	/// Creates a Voronoi transition (entry/intro) that gradually uncovers the screen over the specified duration.
	/// </summary>
	/// <param name="duration">The duration, in seconds, for which the Voronoi effect will run. Defaults to 2 seconds if not specified.</param>
	/// <returns>A new instance of the Voronoi transition configured for uncovering the screen, ready for further configuration.</returns>
	public static Voronoi Uncover(float duration = 2f)
	{
		return new Voronoi()
		{
			Duration = duration,
			Reverse = true,
			TransitionType = Tween.TransitionType.Sine,
			Angle = 180,
		};
	}
}
