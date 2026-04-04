using System.Threading.Tasks;
using Godot;
using KaleidoWarp;

/// <summary>
/// This class demonstrates an example of a custom loading screen. 
/// </summary>
public partial class Loader : CanvasLayer
{
	PackedScene? _targetScene;

	ProgressBar ProgressBar => GetNode<ProgressBar>("%ProgressBar");
	AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("%AnimationPlayer");
	public float Progress { get; set; }

	public override void _Ready()
	{
		base._Ready();
		_ = LoadSceneAsync();
	}

	public override void _Process(double delta)
	{
		ProgressBar.Value = Progress;

		// Warp to new scene
		if (_targetScene != null)
		{
			WarpManager.Instance.WarpToPacked(_targetScene, null, Voronoi.Uncover().Angle(270).Ease(Tween.EaseType.Out));
			_targetScene = null;
		}
	}

	async Task LoadSceneAsync()
	{
		bool entered = false, loaded = false, exited = false;

		// Play a custom entry animation
		AnimationPlayer.Play("Enter");
		AnimationPlayer.AnimationFinished += _ => entered = true;

		// Simulate threaded loading via progress tween
		var tween = CreateTween();
		tween.TweenProperty(this, nameof(Progress), 100f, 2f);
		tween.Finished += () => loaded = true;

		while (!entered || !loaded)
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		// Actually load the target scene (pretend threaded)
		var scene = GD.Load<PackedScene>("res://Examples/Scene2/Scene2.tscn");

		// Play custom exit animation
		AnimationPlayer.Play("Exit");
		AnimationPlayer.AnimationFinished += _ => exited = true;

		// Wait until exit animation finishes
		while (!exited)
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		// Signal to warp in the next process frame
		_targetScene = scene;
	}
}
