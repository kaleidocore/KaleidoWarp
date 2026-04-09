// Source: http://www.github.com/kaleidocore/KaleidoWarp

using Godot;

namespace KaleidoWarp;

/// <summary>
/// This class provides a collection of predefined texture patterns that can be used for the dissolve transition.
/// </summary>
/// <remarks>The framework instantiates this class as needed and provides it to the Pattern selector function of the <see cref="Dissolve"/> transition.</remarks>
public class DefaultPatterns
{
	string BasePath { get; }

	public DefaultPatterns(string basePath)
	{
		BasePath = basePath;
	}

	Texture2D Tex(string name)
	{
		var path = BasePath.PathJoin("patterns").PathJoin($"{name}.png");
		return GD.Load<Texture2D>(path);
	}

	public Texture2D BlindsH => Tex("blinds_h");
	public Texture2D BlindsV => Tex("blinds_v");
	public Texture2D Cells => Tex("cells");
	public Texture2D Circle => Tex("circle");
	public Texture2D Clock => Tex("clock");
	public Texture2D CurtainsH => Tex("curtains_h");
	public Texture2D CurtainsV => Tex("curtains_v");
	public Texture2D Liquid => Tex("liquid");
	public Texture2D PaintBrushH => Tex("paint_brush_h");
	public Texture2D PaintBrushV => Tex("paint_brush_v");
	public Texture2D PixelNoise1 => Tex("pixel_noise_1");
	public Texture2D PixelNoise2 => Tex("pixel_noise_2");
	public Texture2D PixelNoise3 => Tex("pixel_noise_3");
	public Texture2D Scribbles => Tex("scribbles");
	public Texture2D Smoke => Tex("smoke");
	public Texture2D Square => Tex("square");
	public Texture2D Squares => Tex("squares");
	public Texture2D Swirl => Tex("swirl");
	public Texture2D TileReveal => Tex("tile_reveal");
	public Texture2D WipeH => Tex("wipe_h");
	public Texture2D WipeV => Tex("wipe_v");
}
