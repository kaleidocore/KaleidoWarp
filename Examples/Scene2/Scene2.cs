using Godot;
using KaleidoWarp;

/// <summary>
/// This class demonstrates a target scene we want to warp to
/// </summary>
public partial class Scene2 : Node2D
{
	static PackedScene MainScene { get; } = GD.Load<PackedScene>("res://Examples/Scene1/Scene1.tscn");

	// Simple timer to show we are running
	float _timer;
	Label TimeLabel => GetNode<Label>("%TimeLabel");

	// UI
	Button BackButton => GetNode<Button>("%BackButton");

	public override void _Ready()
	{
		base._Ready();

		// Just warp back to main
		BackButton.Pressed += () => WarpManager.Instance.WarpToPacked(MainScene, ColorFade.Cover(.3f), ColorFade.Uncover(.3f));
	}

	public override void _Process(double delta)
	{
		_timer += (float)delta;
		TimeLabel.Text = _timer.ToString("F2");
	}
}
