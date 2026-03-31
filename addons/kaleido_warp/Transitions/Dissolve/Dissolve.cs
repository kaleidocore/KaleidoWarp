using System;
using Godot;

namespace KaleidoWarp;

/// <summary>
/// Represents a transition effect that gradually dissolves an image using a specified texture pattern.
/// </summary>
/// <remarks>Use the static methods <see cref="Cover(float)"/> and <see cref="Uncover(float)"/> to create
/// transitions for covering or uncovering the screen, respectively. The transition duration, easing, and other shader effects
/// can be further customized via chained calls to achieve various visual styles.
/// </remarks>
public partial class Dissolve : Transition
{
	const string ShaderPath = "dissolve.gdshader";

	/// <summary>
	/// The texture used to define the dissolve pattern. The shader will use the pixel values of this texture to determine how to dissolve the image based on the Progress property. If not set, a default pattern will be used from the Patterns class.
	/// </summary>
	[Export]
	public Texture2D? DissolveTexture { get; set; }

	/// <summary>
	/// Indicates whether the dissolve pattern should be flipped horizontally.
	/// </summary>
	[Export]
	public bool FlipX { get; set; } = false;

	/// <summary>
	/// Indicates whether the dissolve pattern should be flipped vertically.
	/// </summary>
	[Export]
	public bool FlipY { get; set; }

	/// <summary>
	/// Indicates whether the dissolve pattern should be inverted.
	/// </summary>
	[Export]
	public bool Invert { get; set; }

	/// <summary>
	/// The feathering amount for the dissolve effect, controlling the softness of the transition edges.
	/// </summary>
	[Export]
	public float Feather { get; set; } = .01f;

	/// <summary>
	/// A function that selects the dissolve texture from the available patterns.
	/// </summary>
	public Func<DissolvePatterns, Texture2D> DissolveTextureSelector { get; set; } = p => p.Squares;

	public override void _Ready()
	{
		base._Ready();

		var shader = LoadLocal<Shader>(ShaderPath);
		Material = new ShaderMaterial
		{
			Shader = shader
		};

		DissolveTexture ??= DissolveTextureSelector(new DissolvePatterns(BasePath));
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		var material = (ShaderMaterial)Material;

		material.SetShaderParameter("image", ImageTexture ?? TransparentPixel);

		if (DissolveTexture != null)
			material.SetShaderParameter("dissolve_texture", DissolveTexture);

		material.SetShaderParameter("progress", Reverse ? Progress : 1.0 - Progress);
		material.SetShaderParameter("flip_x", FlipX);
		material.SetShaderParameter("flip_y", FlipY);
		material.SetShaderParameter("invert", !Invert);
		material.SetShaderParameter("feather", Feather);
	}

	/// <summary>
	/// Creates a new Dissolve transition (exit/out) that uses a dissolve pattern to gradually cover the screen over the specified duration.
	/// </summary>
	/// <param name="duration">The duration, in seconds, for which the dissolve effect will run. Defaults to 2 seconds if not specified.</param>
	/// <returns>A new instance of the Dissolve transition configured for covering the screen, ready for further configuration.</returns>
	public static Dissolve Cover(float duration = 2f)
	{
		return new Dissolve()
		{
			Duration = duration,
			Reverse = false,
		};
	}

	/// <summary>
	/// Creates a new Dissolve transition (enter/in) that uses a dissolve pattern to gradually uncover the screen over the specified duration.
	/// </summary>
	/// <param name="duration">The duration, in seconds, for which the dissolve effect will run. Defaults to 2 seconds if not specified.</param>
	/// <returns>A new instance of the Dissolve transition configured for uncovering the screen, ready for further configuration.</returns>
	public static Dissolve Uncover(float duration = 2f)
	{
		return new Dissolve()
		{
			Duration = duration,
			Invert = true,
			Reverse = true,
		};
	}

}
