using System;
using Godot;

namespace KaleidoWarp;

public enum ImageFit
{
	None,
	Contain,
	Cover,
	Stretch
}

/// <summary>
/// The base class for all transitions. Provides common properties and functionality for managing transition progress, duration, and appearance. Specific transition types can inherit from this class and implement their own visual effects based on the Progress property.
/// </summary>
public abstract partial class Transition : ColorRect
{
	Tween? _tween;

	/// <summary>
	/// A no-op transparent pixel texture used as a default when no texture is provided for a transition.
	/// </summary>
	public static readonly ImageTexture TransparentPixel = CreateTransparentPixel();

	static ImageTexture CreateTransparentPixel()
	{
		var image = Godot.Image.CreateEmpty(1, 1, false, Godot.Image.Format.Rgba8);
		image.Fill(Colors.Transparent);
		return Godot.ImageTexture.CreateFromImage(image);
	}

	/// <summary>
	/// The transition duration in seconds.
	/// </summary>
	[Export]
	public float Duration { get; set; } = 1f;

	/// <summary>
	/// The current progress of the transition, ranging from 0 (start) to 1 (end).
	/// </summary>
	[Export(hint: PropertyHint.Range, "0,1,0.01")]
	public float Progress { get; set; } = 0f;

	/// <summary>
	/// Gets or sets a value indicating whether the transition should be performed in reverse order.
	/// </summary>
	[Export]
	public bool Reverse { get; set; }

	/// <summary>
	/// Gets or sets the easing function used to control the rate of change for the transition animation tween.
	/// </summary>
	[Export]
	public Tween.EaseType Ease { get; set; } = Tween.EaseType.InOut;

	/// <summary>
	/// Gets or sets the type of transition used for the tweening animation.
	/// </summary>
	[Export]
	public Tween.TransitionType TransitionType { get; set; } = Tween.TransitionType.Linear;

	/// <summary>
	/// Gets or sets the base texture of the transition.
	/// </summary>
	[Export]
	public Texture2D? ImageTexture { get; set; }

	[Export]
	public ImageFit ImageFitMode { get; set; } = ImageFit.Stretch;

	/// <summary>
	/// The path of the directory containing the transition script, used for loading related resources such as shaders or textures. This is automatically set in the _Ready method based on the script's resource path.
	/// </summary>
	protected string BasePath { get; private set; } = string.Empty;

	/// <summary>
	/// Gets a value indicating whether the transition has completed.
	/// </summary>
	public virtual bool IsFinished => _tween?.IsRunning() == false;

	protected Transition()
	{
		Color = Colors.Black;
	}

	public override void _Ready()
	{
		base._Ready();

		ProcessMode = ProcessModeEnum.Always;
		Size = GetViewportRect().Size;
		SetAnchorsPreset(LayoutPreset.FullRect);
		Visible = false;

		var scriptPath = GetScript().As<Script>().ResourcePath;
		BasePath = scriptPath.GetBaseDir();
	}

	/// <summary>
	/// Loads a resource of the specified type from a local path relative to the base path.
	/// </summary>
	/// <typeparam name="T">The type of the resource to load. Must be a reference type.</typeparam>
	/// <param name="subPath">The relative path to the resource to be loaded. This path is combined with the base path.</param>
	/// <returns>An instance of type T representing the loaded resource.</returns>
	/// <exception cref="Exception">Thrown if the resource cannot be loaded from the specified path.</exception>
	protected T LoadLocal<T>(string subPath)
		where T : class
	{
		var path = BasePath.PathJoin(subPath);
		return GD.Load<T>(path) ?? throw new Exception($"Failed to load resource at {path}");
	}

	/// <summary>
	/// Starts the transition.
	/// </summary>
	/// <remarks>This method is only used internally by the <see cref="WarpManager"/>.</remarks>
	public void Play()
	{
		Visible = true;
		Progress = 0;
		_tween?.Kill();
		_tween = CreateTween();
		var pt = _tween.TweenProperty(this, nameof(Progress), 1f, Duration);
		pt.SetEase(Ease);
		pt.SetTrans(TransitionType);
	}

	/// <summary>
	/// Stops the transition.
	/// </summary>
	/// <remarks>This method is only used internally by the <see cref="WarpManager"/>.</remarks>
	public void Stop()
	{
		Visible = false;
		_tween?.Kill();
		_tween = null;
	}

	/// <summary>
	/// Cancels the transition gracefully within the given time, optionally reverting it.
	/// </summary>
	/// <remarks>This method is only used internally by the <see cref="WarpManager"/>.</remarks>
	/// <param name="maxDuration">The maximum duration for the cancel animation.</param>
	/// <param name="revert">Indicates whether the transition should be reverted.</param>
	public void Cancel(float maxDuration, bool revert)
	{
		var oldProgress = Progress;
		var remaining = revert ? Duration * oldProgress : Duration * (1f - oldProgress);
		Duration = MathF.Min(remaining, maxDuration);
		var target = revert ? 0f : 1f;

		_tween?.Kill();
		_tween = CreateTween();
		var pt = _tween.TweenProperty(this, nameof(Progress), target, Duration);
		pt.SetEase(Tween.EaseType.Out);
		pt.SetTrans(Tween.TransitionType.Cubic);
	}
}
