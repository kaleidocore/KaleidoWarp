using System.IO;
using System.Linq;
using Godot;
using KaleidoWarp;

/// <summary>
/// This class demonstrate a simple UI with warps to other scenes
/// </summary>
public partial class Scene1 : Node2D
{
	// Example target scenes
	static PackedScene TargetScene { get; } = GD.Load<PackedScene>("res://Examples/Scene2/Scene2.tscn");
	static PackedScene LoaderScene { get; } = GD.Load<PackedScene>("res://Examples/Loader/Loader.tscn");

	// Store UI values
	static Color OverlayColor { get; set; } = Colors.Black;
	static bool UseImage { get; set; } = false;
	static int DissolvePatternIndex { get; set; } = 0;

	// Simple timer to show we are running
	float _timer;
	Label TimeLabel => GetNode<Label>("%TimeLabel");

	// UI buttons
	Button ColorFadeButton => GetNode<Button>("%ColorFadeButton");
	Button DissolveButton => GetNode<Button>("%DissolveButton");
	OptionButton DissolveOptionButton => GetNode<OptionButton>("%DissolveOptionButton");
	Button VoronoiButton => GetNode<Button>("%VoronoiButton");
	Button PixellateButton => GetNode<Button>("%PixellateButton");
	Button LoaderButton => GetNode<Button>("%LoaderButton");
	ColorPickerButton ColorPicker => GetNode<ColorPickerButton>("%ColorPicker");
	CheckButton UseImageButton => GetNode<CheckButton>("%UseImage");

	// UI helpers
	Texture2D OverlayImage => UseImage ? GetNode<Sprite2D>("%OverlayImage").Texture : Transition.TransparentPixel;
	Texture2D[] DissolveTextures { get; set; } = [];
	Texture2D SelectedDissolveTexture => DissolveTextures[DissolvePatternIndex];

	public override void _Ready()
	{
		base._Ready();

		PrepareDropdown();

		// Remember UI values
		ColorPicker.Color = OverlayColor;
		ColorPicker.ColorChanged += (c) => OverlayColor = c;
		UseImageButton.ButtonPressed = UseImage;
		UseImageButton.Toggled += (v) => UseImage = v;
		DissolveOptionButton.Selected = DissolvePatternIndex;
		DissolveOptionButton.ItemSelected += index => DissolvePatternIndex = (int)index;

		// Transition with (mostly) default settings
		ColorFadeButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, ColorFade.Cover().Color(OverlayColor).Image(OverlayImage), ColorFade.Uncover().Color(OverlayColor).Image(OverlayImage));
		VoronoiButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Voronoi.Cover().Color(OverlayColor).Image(OverlayImage), Voronoi.Uncover().Color(OverlayColor).Image(OverlayImage));
		PixellateButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Pixellate.Cover().Color(OverlayColor).Image(OverlayImage), Pixellate.Uncover().Color(OverlayColor).Image(OverlayImage));
		DissolveButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Dissolve.Cover().Color(OverlayColor).Image(OverlayImage).Pattern(SelectedDissolveTexture), Dissolve.Uncover().Color(OverlayColor).Image(OverlayImage).Pattern(SelectedDissolveTexture));
		LoaderButton.Pressed += () => WarpManager.Instance.WarpToPacked(LoaderScene, Dissolve.Cover(1f).Pattern(p => p.TileReveal), null);
	}

	public override void _Process(double delta)
	{
		_timer += (float)delta;
		TimeLabel.Text = _timer.ToString("F2");
	}

	void PrepareDropdown()
	{
		// Load and populate the dropdown with default textures for demo purposes.
		const string dir = "res://addons/kaleido_warp/Transitions/Dissolve/patterns";
		DissolveTextures = [.. DirAccess.GetFilesAt(dir).Where(f => f.EndsWith(".png")).Select(f => GD.Load<Texture2D>(dir.PathJoin(f)))];

		for (int i = 0; i < DissolveTextures.Length; i++)
		{
			var tex = DissolveTextures[i];
			DissolveOptionButton.AddIconItem(tex, Path.GetFileNameWithoutExtension(tex.ResourcePath), i);
		}
	}
}
