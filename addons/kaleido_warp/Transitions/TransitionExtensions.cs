// Source: http://www.github.com/kaleidocore/KaleidoWarp

using System;
using Godot;

namespace KaleidoWarp;
#nullable enable

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
	public static T Image<T>(this T transition, string imagePath, ImageFit fit = ImageFit.None)
		where T : Transition
		=> Image(transition, GD.Load<Texture2D>(imagePath), fit);

	/// <summary>
	/// An optional <see cref="Texture2D"/> image to use for the transition, effectively rendered above the base color.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance to which the image will be applied.</param>
	/// <param name="image">The image to apply to the transition.</param>
	/// <returns>The modified transition instance with the updated image.</returns>
	public static T Image<T>(this T transition, Texture2D? image, ImageFit fit = ImageFit.None)
		where T : Transition
	{
		transition.ImageTexture = image;
		transition.ImageFitMode = fit;
		return transition;
	}

	/// <summary>
	/// Reverses the direction of the transition by toggling its Reverse property.
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
	/// Sets the easing function for the transition and returns the updated transition instance.
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
	/// Sets the curve type for the transition and returns the updated instance.
	/// </summary>
	/// <typeparam name="T">The type of the transition. Must inherit from the Transition class.</typeparam>
	/// <param name="transition">The transition instance to update. Cannot be null.</param>
	/// <param name="transitionType">The transition type to assign to the specified transition instance.</param>
	/// <returns>The updated transition instance with the specified transition type applied.</returns>
	public static T Curve<T>(this T transition, Tween.TransitionType curve)
		where T : Transition
	{
		transition.Curve = curve;
		return transition;
	}

	/// <summary>
	/// Sets the sticky behavior for the slide transition, which determines if the overlay image should remain fixed in place during the transition animation, creating a "sticky" effect where the image appears to be glued to the screen rather than moving with the slide.
	/// </summary>
	/// <typeparam name="T">The type of slide transition. Must inherit from Slide.</typeparam>
	/// <param name="transition">The slide transition to configure.</param>
	/// <param name="sticky">A value indicating whether the transition should be sticky.</param>
	/// <returns>The slide transition with the updated sticky setting.</returns>
	public static T Sticky<T>(this T transition, bool sticky)
		where T : Slide
	{
		transition.Sticky = sticky;
		return transition;
	}

	/// <summary>
	/// Sets the direction of the slide transition.
	/// </summary>
	/// <typeparam name="T">The type of slide to configure. Must inherit from Slide.</typeparam>
	/// <param name="transition">The slide transition to configure. Cannot be null.</param>
	/// <param name="direction">The direction to apply to the slide transition.</param>
	/// <returns>The slide transition with the specified direction applied.</returns>
	public static T Direction<T>(this T transition, Direction direction)
		where T : Slide
	{
		transition.Direction = direction;
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
	public static T Pattern<T>(this T dissolve, Func<DefaultPatterns, Texture2D> pattern)
		where T : Dissolve
	{
		dissolve.DefaultSelector = pattern;
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
	/// Sets a custom texture for the dissolve effect, allowing for a specific pattern to be used in the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to configure with the custom texture.</param>
	/// <param name="patternPath">The path to the custom texture to use for the dissolve effect.</param>
	/// <returns>The modified dissolve instance with the custom texture applied.</returns>
	public static T Pattern<T>(this T dissolve, string patternPath)
		where T : Dissolve
		=> Pattern(dissolve, _ => GD.Load<Texture2D>(patternPath));

	/// <summary>
	/// Inverts the dissolve pattern texture, effectively reversing the visual effect of the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to invert.</param>
	/// <returns>The modified dissolve instance with the inverted pattern applied.</returns>
	public static T Invert<T>(this T dissolve)
		where T : Dissolve
		=> Invert(dissolve, !dissolve.Invert);

	/// <summary>
	/// Inverts the dissolve pattern texture, effectively reversing the visual effect of the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to invert.</param>
	/// <param name="invert">A boolean value indicating whether to invert the dissolve pattern. If true, the pattern will be inverted; if false, it will remain unchanged.</param>
	/// <returns>The modified dissolve instance with the inverted pattern applied.</returns>
	public static T Invert<T>(this T dissolve, bool invert)
		where T : Dissolve
	{
		dissolve.Invert = invert;
		return dissolve;
	}

	/// <summary>
	/// Toggles the horizontal flip of the dissolve pattern, creating a mirrored effect along the vertical axis of the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to flip horizontally.</param>
	/// <returns>The modified dissolve instance with the horizontal flip applied.</returns>
	public static T FlipX<T>(this T dissolve)
		where T : Dissolve
		=> FlipX(dissolve, !dissolve.FlipX);


	/// <summary>
	/// Sets the horizontal flip of the dissolve pattern, creating a mirrored effect along the vertical axis of the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to flip horizontally.</param>
	/// <param name="flipX">A boolean value indicating whether to flip the dissolve pattern horizontally. If true, the pattern will be flipped; if false, it will remain unchanged.</param>
	/// <returns>The modified dissolve instance with the horizontal flip applied.</returns>
	public static T FlipX<T>(this T dissolve, bool flipX)
		where T : Dissolve
	{
		dissolve.FlipX = flipX;
		return dissolve;
	}

	/// <summary>
	/// Toggles the vertical flip of the dissolve pattern, creating a mirrored effect along the horizontal axis of the dissolve animation.
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
	/// Sets the vertical flip of the dissolve pattern, creating a mirrored effect along the horizontal axis of the dissolve animation.
	/// </summary>
	/// <typeparam name="T">The type of the dissolve instance. Must inherit from the Dissolve class.</typeparam>
	/// <param name="dissolve">The dissolve instance to flip vertically.</param>
	/// <param name="flipY">A boolean value indicating whether to flip the dissolve pattern vertically. If true, the pattern will be flipped; if false, it will remain unchanged.</param>
	/// <returns>The modified dissolve instance with the vertical flip applied.</returns>
	public static T FlipY<T>(this T dissolve, bool flipY)
		where T : Dissolve
	{
		dissolve.FlipY = flipY;
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

	/// <summary>
	/// Sets the amount (ratio) of pixelation to apply to the specified transition.
	/// </summary>
	/// <typeparam name="T">The type of the transition, which must inherit from Pixellate.</typeparam>
	/// <param name="transition">The transition instance to configure. Must not be null.</param>
	/// <param name="amount">The amount of pixelation to apply. Higher values result in more pronounced pixelation.</param>
	/// <returns>The transition instance with the updated pixelation amount.</returns>
	public static T Amount<T>(this T transition, float amount)
		where T : Pixellate
	{
		transition.Amount = amount;
		return transition;
	}

	/// <summary>
	/// Sets the origin point for the pixellation effect, which determines the center point from which the pixelation will radiate or be applied in the transition animation.
	/// </summary>
	/// <typeparam name="T">The type of the transition, which must inherit from Pixellate.</typeparam>
	/// <param name="transition">The transition instance to configure. Must not be null.</param>
	/// <param name="origin">The origin point for the pixellation effect, specified as a normalized vector where (0, 0) represents the top-left corner and (1, 1) represents the bottom-right corner.</param>
	/// <returns>The transition instance with the updated origin point.</returns>
	public static T Origin<T>(this T transition, Vector2 origin)
		where T : Pixellate
	{
		transition.Origin = origin;
		return transition;
	}

}
