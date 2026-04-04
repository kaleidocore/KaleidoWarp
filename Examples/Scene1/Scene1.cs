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
	static bool StickySlide { get; set; } = true;
	static int VoronoiAngle { get; set; } = 0;
	static int PixelAmount { get; set; } = 200;
	static int DissolvePatternIndex { get; set; } = 0;

	// Simple timer to show we are running
	float _timer;
	Label TimeLabel => GetNode<Label>("%TimeLabel");

	// UI buttons
	Button ColorFadeButton => GetNode<Button>("%ColorFadeButton");
	Button SlideButton => GetNode<Button>("%SlideButton");
	CheckButton StickySlideButton => GetNode<CheckButton>("%StickySlideButton");
	Button VoronoiButton => GetNode<Button>("%VoronoiButton");
	SpinBox VoronoiAngleSpin => GetNode<SpinBox>("%VoronoiAngleSpin");
	Button PixellateButton => GetNode<Button>("%PixellateButton");
	SpinBox PixelAmountSpin => GetNode<SpinBox>("%PixelAmountSpin");
	Button DissolveButton => GetNode<Button>("%DissolveButton");
	OptionButton DissolveOptionButton => GetNode<OptionButton>("%DissolveOptionButton");
	Button LoaderButton => GetNode<Button>("%LoaderButton");
	ColorPickerButton ColorPicker => GetNode<ColorPickerButton>("%ColorPicker");
	CheckButton UseImageButton => GetNode<CheckButton>("%UseImage");

	// UI helpers
	Texture2D? OverlayImage => UseImage ? GetNode<Sprite2D>("%OverlayImage").Texture : null;
	Texture2D[] DissolveTextures { get; set; } = [];
	Texture2D DissolveTexture => DissolveTextures[DissolvePatternIndex];

	public override void _Ready()
	{
		base._Ready();

		PrepareDropdown();

		// Remember UI values
		ColorPicker.Color = OverlayColor;
		ColorPicker.ColorChanged += (c) => OverlayColor = c;
		UseImageButton.ButtonPressed = UseImage;
		UseImageButton.Toggled += (v) => UseImage = v;
		StickySlideButton.ButtonPressed = StickySlide;
		StickySlideButton.Toggled += (v) => StickySlide = v;
		VoronoiAngleSpin.Value = VoronoiAngle;
		VoronoiAngleSpin.ValueChanged += (v) => VoronoiAngle = (int)v;
		PixelAmountSpin.Value = PixelAmount;
		PixelAmountSpin.ValueChanged += (v) => PixelAmount = (int)v;
		DissolveOptionButton.Selected = DissolvePatternIndex;
		DissolveOptionButton.ItemSelected += index => DissolvePatternIndex = (int)index;

		// Transition with (mostly) default settings
		ColorFadeButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, ColorFade.Cover().Color(OverlayColor).Image(OverlayImage), ColorFade.Uncover().Color(OverlayColor).Image(OverlayImage));
		SlideButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Slide.Cover().Color(OverlayColor).Image(OverlayImage).Sticky(StickySlide), Slide.Uncover().Color(OverlayColor).Image(OverlayImage).Sticky(StickySlide));
		VoronoiButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Voronoi.Cover().Color(OverlayColor).Image(OverlayImage).Angle(VoronoiAngle), Voronoi.Uncover().Color(OverlayColor).Image(OverlayImage).Angle(VoronoiAngle));
		PixellateButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Pixellate.Cover().Color(OverlayColor).Image(OverlayImage).Amount(PixelAmount), Pixellate.Uncover().Color(OverlayColor).Image(OverlayImage).Amount(PixelAmount));
		DissolveButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Dissolve.Cover().Color(OverlayColor).Image(OverlayImage).Pattern(DissolveTexture), Dissolve.Uncover().Color(OverlayColor).Image(OverlayImage).Pattern(DissolveTexture));
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
