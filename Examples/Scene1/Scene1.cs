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
	static float Duration { get; set; } = 1f;
	static Color OverlayColor { get; set; } = Colors.Black;
	static bool UseImage { get; set; } = false;
	static int CurrentTab { get; set; }
	static Direction SlideDirection { get; set; } = Direction.Left;
	static bool StickySlide { get; set; } = false;
	static int VoronoiAngle { get; set; } = 0;
	static int PixelAmount { get; set; } = 200;
	static int PatternIndex { get; set; } = 0;
	static float FeatherAmount { get; set; } = 0f;

	// Simple timer to show we are running
	float _timer;
	Label TimeLabel => GetNode<Label>("%TimeLabel");

	// UI buttons
	SpinBox DurationSpin => GetNode<SpinBox>("%DurationSpin");
	ColorPickerButton ColorPicker => GetNode<ColorPickerButton>("%ColorPicker");
	CheckButton UseImageButton => GetNode<CheckButton>("%UseImage");
	TabContainer Transitions => GetNode<TabContainer>("%Transitions");
	Button ColorFadeButton => GetNode<Button>("%ColorFadeButton");
	Button SlideButton => GetNode<Button>("%SlideButton");
	OptionButton DirectionOptionButton => GetNode<OptionButton>("%DirectionOptionButton");
	CheckButton StickyButton => GetNode<CheckButton>("%StickyButton");
	Button VoronoiButton => GetNode<Button>("%VoronoiButton");
	SpinBox AngleSpin => GetNode<SpinBox>("%AngleSpin");
	Button PixellateButton => GetNode<Button>("%PixellateButton");
	SpinBox PixelAmountSpin => GetNode<SpinBox>("%PixelAmountSpin");
	Button DissolveButton => GetNode<Button>("%DissolveButton");
	OptionButton PatternOptionButton => GetNode<OptionButton>("%PatternOptionButton");
	SpinBox FeatherSpin => GetNode<SpinBox>("%FeatherSpin");
	Button LoaderButton => GetNode<Button>("%LoaderButton");

	// UI helpers
	Texture2D? OverlayImage => UseImage ? GetNode<Sprite2D>("%OverlayImage").Texture : null;
	Texture2D[] DissolveTextures { get; set; } = [];
	Texture2D DissolveTexture => DissolveTextures[PatternIndex];

	public override void _Ready()
	{
		base._Ready();

		PrepareDirections();
		PreparePatterns();

		// Remember UI values
		DurationSpin.Value = Duration;
		DurationSpin.ValueChanged += (v) => Duration = (float)v;
		ColorPicker.Color = OverlayColor;
		ColorPicker.ColorChanged += (c) => OverlayColor = c;
		UseImageButton.ButtonPressed = UseImage;
		UseImageButton.Toggled += (v) => UseImage = v;
		Transitions.CurrentTab = CurrentTab;
		Transitions.TabChanged += index => CurrentTab = (int)index;
		DirectionOptionButton.Selected = (int)SlideDirection;
		DirectionOptionButton.ItemSelected += index => SlideDirection = (Direction)index;
		StickyButton.ButtonPressed = StickySlide;
		StickyButton.Toggled += (v) => StickySlide = v;
		AngleSpin.Value = VoronoiAngle;
		AngleSpin.ValueChanged += (v) => VoronoiAngle = (int)v;
		PixelAmountSpin.Value = PixelAmount;
		PixelAmountSpin.ValueChanged += (v) => PixelAmount = (int)v;
		PatternOptionButton.Selected = PatternIndex;
		PatternOptionButton.ItemSelected += index => PatternIndex = (int)index;
		FeatherSpin.Value = FeatherAmount;
		FeatherSpin.ValueChanged += (v) => FeatherAmount = (float)v;

		// Transition with (some) settings applied
		ColorFadeButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, ColorFade.Cover(Duration).Color(OverlayColor).Image(OverlayImage), ColorFade.Uncover(Duration).Color(OverlayColor).Image(OverlayImage));
		SlideButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Slide.Cover(Duration).Color(OverlayColor).Image(OverlayImage).Direction(SlideDirection).Sticky(StickySlide), Slide.Uncover(Duration).Color(OverlayColor).Image(OverlayImage).Direction(SlideDirection).Sticky(StickySlide));
		VoronoiButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Voronoi.Cover(Duration).Color(OverlayColor).Image(OverlayImage).Angle(VoronoiAngle), Voronoi.Uncover(Duration).Color(OverlayColor).Image(OverlayImage).Angle(VoronoiAngle + 180));
		PixellateButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Pixellate.Cover(Duration).Color(OverlayColor).Image(OverlayImage).Amount(PixelAmount), Pixellate.Uncover(Duration).Color(OverlayColor).Image(OverlayImage).Amount(PixelAmount));
		DissolveButton.Pressed += () => WarpManager.Instance.WarpToPacked(TargetScene, Dissolve.Cover(Duration).Color(OverlayColor).Image(OverlayImage).Pattern(DissolveTexture).Feather(FeatherAmount), Dissolve.Uncover(Duration).Color(OverlayColor).Image(OverlayImage).Pattern(DissolveTexture).Feather(FeatherAmount).Invert());
		LoaderButton.Pressed += () => WarpManager.Instance.WarpToPacked(LoaderScene, Dissolve.Cover(3f).Pattern(p => p.TileReveal).Feather(.1f).Ease(Tween.EaseType.InOut).Curve(Tween.TransitionType.Sine), null);
	}

	public override void _Process(double delta)
	{
		_timer += (float)delta;
		TimeLabel.Text = _timer.ToString("F2");
	}


	void PrepareDirections()
	{
		foreach (Direction dir in System.Enum.GetValues<Direction>())
			DirectionOptionButton.AddItem(dir.ToString(), (int)dir);
	}

	void PreparePatterns()
	{
		// Load and populate the dropdown with default textures for demo purposes.
		const string dir = "res://addons/kaleido_warp/Transitions/Dissolve/patterns";
		DissolveTextures = [.. DirAccess.GetFilesAt(dir).Where(f => f.EndsWith(".png")).Select(f => GD.Load<Texture2D>(dir.PathJoin(f)))];

		for (int i = 0; i < DissolveTextures.Length; i++)
		{
			var tex = DissolveTextures[i];
			PatternOptionButton.AddIconItem(tex, Path.GetFileNameWithoutExtension(tex.ResourcePath), i);
		}
	}
}
