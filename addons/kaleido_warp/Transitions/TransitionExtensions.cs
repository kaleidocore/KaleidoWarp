using System;
using Godot;

namespace KaleidoWarp;

/// <summary>
/// Fluent transition API for the <see cref="Transition"/> class and its derivatives. These extension methods allow for chaining configuration calls to set properties such as color, texture, easing, and shader parameters in a more concise and readable manner when creating transition instances.
/// </summary>
public static class TransitionExtensions
{
	/// <summary>
	/// The base color to use for the transition.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance to which the color will be applied.</param>
	/// <param name="color">The color to apply to the transition.</param>
	/// <returns>The modified transition instance with the updated color.</returns>
	public static T Duration<T>(this T transition, float duration)
		where T : Transition
	{
		transition.Duration = duration;
		return transition;
	}

	/// <summary>
	/// The base color to use for the transition.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance to which the color will be applied.</param>
	/// <param name="color">The color to apply to the transition.</param>
	/// <returns>The modified transition instance with the updated color.</returns>
	public static T Color<T>(this T transition, Color color)
		where T : Transition
	{
		transition.Color = color;
		return transition;
	}

	/// <summary>
	/// An optional texture resource file to use for the transition, effectively rendered above the base color.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance to which the image will be applied.</param>
	/// <param name="imagePath">The path to the image resource file to apply to the transition.</param>
	/// <returns>The modified transition instance with the updated image.</returns>
	public static T Image<T>(this T transition, string imagePath)
		where T : Transition
		=> Image(transition, GD.Load<Texture2D>(imagePath));

	/// <summary>
	/// An optional <see cref="Texture2D"/> image to use for the transition, effectively rendered above the base color.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance to which the image will be applied.</param>
	/// <param name="image">The image to apply to the transition.</param>
	/// <returns>The modified transition instance with the updated image.</returns>
	public static T Image<T>(this T transition, Texture2D image)
		where T : Transition
	{
		transition.ImageTexture = image;
		return transition;
	}

	/// <summary>
	/// Reverses the direction of the specified transition by toggling its Reverse property.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance whose direction will be reversed.</param>
	/// <returns>The same transition instance with its Reverse property value toggled.</returns>
	public static T Reverse<T>(this T transition)
		where T : Transition
	{
		transition.Reverse = !transition.Reverse;
		return transition;
	}

	/// <summary>
	/// Sets the easing function for the specified transition and returns the updated transition instance.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance to which the easing function will be applied.</param>
	/// <param name="ease">The easing type to apply, which determines the rate of change for the transition's animation.</param>
	/// <returns>The updated transition instance with the specified easing function applied.</returns>
	public static T Ease<T>(this T transition, Tween.EaseType ease)
		where T : Transition
	{
		transition.Ease = ease;
		return transition;
	}

	/// <summary>
	/// Sets the transition type for the specified transition instance and returns the updated instance.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance to update. Cannot be null.</param>
	/// <param name="transitionType">The transition type to assign to the specified transition instance.</param>
	/// <returns>The updated transition instance with the specified transition type applied.</returns>
	public static T TransType<T>(this T transition, Tween.TransitionType transitionType)
		where T : Transition
	{
		transition.TransitionType = transitionType;
		return transition;
	}

	/// <summary>
	/// Sets the texture for the dissolve effect using the default patterns, effectively configuring the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to configure with the pattern function.</param>
	/// <param name="pattern">A selector function that takes a Patterns instance and returns a Texture2D.
	/// pattern.</param>
	/// <returns>The modified dissolve instance with the updated texture selection function.</returns>
	public static T Pattern<T>(this T dissolve, Func<DissolvePatterns, Texture2D> pattern)
		where T : Dissolve
	{
		dissolve.DissolveTextureSelector = pattern;
		return dissolve;
	}

	/// <summary>
	/// Sets a custom texture for the dissolve effect, allowing for a specific pattern to be used in the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to configure with the custom texture.</param>
	/// <param name="pattern">The custom texture to use for the dissolve effect.</param>
	/// <returns>The modified dissolve instance with the custom texture applied.</returns>
	public static T Pattern<T>(this T dissolve, Texture2D pattern)
		where T : Dissolve
		=> Pattern(dissolve, _ => pattern);


	/// <summary>
	/// Inverts the dissolve pattern texture, effectively reversing the visual effect of the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to invert.</param>
	/// <returns>The modified dissolve instance with the inverted pattern applied.</returns>
	public static T Invert<T>(this T dissolve)
		where T : Dissolve
	{
		dissolve.Invert = !dissolve.Invert;
		return dissolve;
	}

	/// <summary>
	/// Flips the dissolve pattern horizontally, creating a mirrored effect along the vertical axis of the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to flip horizontally.</param>
	/// <returns>The modified dissolve instance with the horizontal flip applied.</returns>
	public static T FlipX<T>(this T dissolve)
		where T : Dissolve
	{
		dissolve.FlipX = !dissolve.FlipX;
		return dissolve;
	}

	/// <summary>
	/// Flips the dissolve pattern vertically, creating a mirrored effect along the horizontal axis of the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to flip vertically.</param>
	/// <returns>The modified dissolve instance with the vertical flip applied.</returns>
	public static T FlipY<T>(this T dissolve)
		where T : Dissolve
	{
		dissolve.FlipY = !dissolve.FlipY;
		return dissolve;
	}

	/// <summary>
	/// Sets the feathering amount for the dissolve effect, enabling smoother or sharper transitions at the dissolve edge.
	/// </summary>
	/// <typeparam name="T">The type of dissolve object. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve object to which the feathering amount will be applied.</param>
	/// <param name="feather">The amount of feathering to apply to the dissolve effect. Higher values result in a smoother transition.</param>
	/// <returns>The modified dissolve object with the updated feathering amount.</returns>
	public static T Feather<T>(this T dissolve, float feather)
		where T : Dissolve
	{
		dissolve.Feather = feather;
		return dissolve;
	}

	/// <summary>
	/// Sets ths angle of the Voronoi pattern, which determines the orientation of the pattern used in the transition effect.
	/// </summary>
	/// <typeparam name="T">The type of the Voronoi instance. Must inherit from the Voronoi class.</typeparam>
	/// <param name="voronoi">The Voronoi instance to configure with the specified angle.</param>
	/// <param name="angle">The angle to set for the Voronoi pattern.</param>
	/// <returns>The modified Voronoi instance with the updated angle.</returns>
	public static T Angle<T>(this T voronoi, int angle)
		where T : Voronoi
	{
		voronoi.Angle = angle;
		return voronoi;
	}
}
