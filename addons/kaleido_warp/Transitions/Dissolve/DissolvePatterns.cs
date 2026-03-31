using Godot;

namespace KaleidoWarp;

/// <summary>
/// This class provides a collection of predefined texture patterns that can be used for dissolve transition effects.
/// </summary>
/// <remarks>The framework instantiates this class as needed and provides it to the Pattern selector function of the <see cref="Dissolve"/> transition.</remarks>
public class DissolvePatterns
{
	string BasePath { get; }

	public DissolvePatterns(string basePath)
	{
		BasePath = basePath;
	}

	Texture2D Tex(string name)
	{
		var path = BasePath.PathJoin("patterns").PathJoin($"{name}.png");
		return GD.Load<Texture2D>(path);
	}

	public Texture2D Circle => Tex("circle");
	public Texture2D Conical => Tex("conical");
	public Texture2D Curtains => Tex("curtains");
	public Texture2D NoiseBlur => Tex("noise_blur");
	public Texture2D NoiseCell => Tex("noise_cell");
	public Texture2D NoiseNormal => Tex("noise_normal");
	public Texture2D NoisePixelated => Tex("noise_pixelated");
	public Texture2D PaintBrushH => Tex("paint_brush_h");
	public Texture2D PaintBrushV => Tex("paint_brush_v");
	public Texture2D Scribbles => Tex("scribbles");
	public Texture2D Square => Tex("square");
	public Texture2D Squares => Tex("squares");
	public Texture2D Swirl => Tex("swirl");
	public Texture2D TileReveal => Tex("tile_reveal");
	public Texture2D WipeH => Tex("wipe_h");
	public Texture2D WipeV => Tex("wipe_v");
}
