using Godot;

namespace KaleidoWarp;

/// <summary>
/// Represents a color fade transition that gradually covers or uncovers the screen with a color and/or image.
/// </summary>
/// <remarks>Use the static methods <see cref="Cover(float)"/> and <see cref="Uncover(float)"/> to create
/// transitions for covering or uncovering the screen, respectively. The transition duration, easing, and other shader effects
/// can be further customized via chained calls to achieve various visual styles.
/// </remarks>
public partial class ColorFade : Transition
{
	const string ShaderPath = "color_fade.gdshader";

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
	}

	/// <summary>
	/// Creates a color fade transition (exit/out) that gradually covers the screen with a color and/or image.
	/// </summary>
	/// <param name="duration">The duration of the transition in seconds.</param>
	/// <returns>A new instance of <see cref="ColorFade"/> configured to cover the screen, ready for further configuration.</returns>
	public static ColorFade Cover(float duration = 1f)
	{
		return new ColorFade()
		{
			Duration = duration,
			Reverse = false,
			Ease = Tween.EaseType.InOut,
			TransitionType = Tween.TransitionType.Quad,
		};
	}

	/// <summary>
	/// Creates a color fade transition (enter/in) that gradually uncovers the screen, revealing the underlying content.
	/// </summary>
	/// <param name="duration">The duration of the transition in seconds.</param>
	/// <returns>A new instance of <see cref="ColorFade"/> configured to uncover the screen, ready for further configuration.</returns>
	public static ColorFade Uncover(float duration = 1f)
	{
		return new ColorFade()
		{
			Duration = duration,
			Reverse = true,
			TransitionType = Tween.TransitionType.Quad,
		};
	}
}
