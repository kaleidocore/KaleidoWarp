using Godot;

namespace KaleidoWarp;
#nullable enable

/// <summary>
/// Represents a slide transition that moves the screen in a specified direction to cover or uncover the content.
/// </summary>
/// <remarks>Use the static methods <see cref="Cover(float)"/> and <see cref="Uncover(float)"/> to create
/// transitions for covering or uncovering the screen, respectively. The transition duration, easing, and other shader effects
/// can be further customized via chained calls to achieve various visual styles.
/// </remarks>
public partial class Slide : Transition, ITransitionFactory<Slide>
{
	const string ShaderPath = "slide.gdshader";

	/// <summary>
	/// Specifies the direction to slide the current scene. The default value is <see cref="Direction.Right"/>.
	/// </summary>
	[Export]
	public Direction Direction = Direction.Right;

	/// <summary>
	/// Controls whether the overlay image should "stick" to the screen during the slide transition, meaning it will not move with the sliding effect. When set to true, the image will remain fixed in place while the transition occurs.
	/// </summary>
	[Export]
	public bool Sticky = true;

	public override void _Ready()
	{
		base._Ready();
		LoadShader(ShaderPath);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		var material = (ShaderMaterial)Material;
		material.SetShaderParameter("progress", Reverse ? 1.0 - Progress : Progress);
		material.SetShaderParameter("image", ImageTexture ?? TransparentPixel);
		material.SetShaderParameter("image_fit", (int)ImageFitMode);
		material.SetShaderParameter("direction", (int)Direction);
		material.SetShaderParameter("sticky", Sticky);
	}

	/// <summary>
	/// Creates a slide transition (exit/out) that gradually slides the scene out, replacing it with a color and/or image.
	/// </summary>
	/// <param name="duration">The duration of the transition in seconds.</param>
	/// <returns>A new instance of <see cref="Slide"/> configured to cover the screen, ready for further configuration.</returns>
	public static Slide Cover(float duration = 1f)
	{
		return new Slide
		{
			Duration = duration,
			Ease = Tween.EaseType.InOut,
			Curve = Tween.TransitionType.Quad,
			Reverse = false
		};
	}

	/// <summary>
	/// Creates a slide transition (enter/in) that gradually slides the scene in, revealing it from a color and/or image.
	/// </summary>
	/// <param name="duration">The duration of the transition in seconds.</param>
	/// <returns>A new instance of <see cref="Slide"/> configured to uncover the screen, ready for further configuration.</returns>
	public static Slide Uncover(float duration = 1f)
	{
		return new Slide()
		{
			Duration = duration,
			Ease = Tween.EaseType.InOut,
			Curve = Tween.TransitionType.Quad,
			Reverse = true
		};
	}
}
