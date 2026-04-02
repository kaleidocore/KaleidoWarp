# 🎬 KaleidoWarp for Godot

A flexible and extensible scene transition addon for Godot (.NET/C#) that allows for smooth, animated transitions between scenes.

## Installation 📦
You have three options:


a) Just install the addon via the Godot Asset Library from inside Godot.

_or_

b) Clone the repo locally via Github Desktop or CLI, then copy (merge) the `addons/` folder into your project folder.

_or_

c) Download the repo as a .zip, extract anywhere,  then copy (merge) the `addons/` folder into your project folder.

Important:
> After installation, make sure the KaleidoWarp plugin is enabled in the project settings. You might have to build the Godot project at least once before Godot can activate the plugin.

## Examples 📜

A minimal one-liner:
```csharp
using KaleidoWarp;

WarpManager.Instance.WarpToFile("res://Scenes/Scene2.tscn", ColorFade.Cover(), ColorFade.Uncover());
```

Or slightly more elaborate:
```csharp
using KaleidoWarp;

WarpManager.Instance.WarpToFile(
    "res://Scenes/Scene2.tscn",
    Pixellate.Cover(3f).Color(Colors.Blue).Amount(300f),
    Voronoi.Uncover(2f).Color(Colors.Blue).Angle(45)
);
```

Further examples can also be found and previewed in the example scenes.

## WarpManager API

The primary API of the `WarpManager` class mirrors Godot's default scene navigation while adding optional transitions:
```csharp
public void WarpToFile(string scenePath, Transition? transitionOut, Transition? transitionIn)
public void WarpToPacked(PackedScene packedScene, Transition? transitionOut, Transition? transitionIn)
public void WarpToNode(Node sceneNode, Transition? transitionOut, Transition? transitionIn)
```


## Transitions

The addon comes with 4 built-in, shader based transition styles, each individually configurable:

| Transition class | Description |
|------------|-------------|
| `(Transition)` | Abstract base class for transitions |
| `ColorFade` | A basic screen fade |
| `Voronoi` | A randomized bubbly pattern that sweeps across the screen at a given angle |
| `Pixellation` | A pixellating effect reminiscent of the classic Super Mario pixel fade |
| `Dissolve` | Uses a grayscale pattern texture to define when and where each screen pixel is overlaid and blended |

Should these somehow not cover your needs you are free to implement your own custom transitions inherited from `Transition` (`Transition.tscn`), which handles most of the groundwork. And don't forget to share them here!


## Configuring Transitions 🎭

All transition implement the following two static factory methods:

```csharp
// Gradually covers the screen — scene exit/outro
public static T Cover(float duration);

// Gradually uncovers the screen — scene entry/intro
public static T Uncover(float duration);
```

The factories are primarily for convencience and the main difference betweem `Cover()` and `Uncover()` is that the latter sets the transition up to play in reverse.
You can mix and match transition styles for outro/intro however you like - as long as both of them have the same color and image they should overlap seamlessly.

## Transition common base API

All transitions inherit from the abstract base class `Transition` and expose a fluent configuration API:
```csharp
transition
    .Duration(1f)                                 // Duration in seconds
    .Color(Colors.Black)                          // Base color of the transition overlay
    .Image("res://overlay.png", ImageFit.None)    // Optional overlay image (path or Texture2D)
    .Ease(Tween.EaseType.InOut)                   // Easing across the transition duration
    .Curve(Tween.TransitionType.Quad)             // Animation curve
    .Reverse();                                   // Reverses animation direction
```

> **Note:** `Duration()` and `Reverse()` are typically not needed directly since they are initialized by `Cover()`/`Uncover()`.

---

## ColorFade transition

This is a basic fade-in/fade-out transition.

The `ColorFade` transition does not add any additional properties beyond the base `Transition` API.

```csharp
// Fade screen to green over 2 seconds
ColorFade.Cover(2f).Color(Colors.Green);

// Fade screen from blue over 2 seconds
ColorFade.Uncover(2f).Color(Colors.Blue);

// Fade to a texture, using black as background for transparent areas
ColorFade.Cover(3f).Image("res://my_overlay.png");

// Fade from a texture, using red as background for transparent areas
ColorFade.Uncover(3f).Color(Colors.Red).Image("res://my_overlay.png");
```

## Voronoi transition

This is a randomly generated, bubbly pattern of the color/image overlay that sweep across the screen.

The `Voronoi` transition adds the following properties:
```csharp
transition
    .Angle(0); // The sweep angle across the screen, default 0 (left to right)
```

Example:
```csharp
// A blue sweep from top-left to bottom-right
Voronoi.Cover(2f).Color(Colors.Blue).Angle(45);

// An image sweep, from left to right but for an entry transition.
ColorFade.Uncover(2f).Image("res://my_overlay.png", FitMode.Stretch).Angle(180);
```

## Pixellation transition

This pixellates the scene gradually, eventually fading out to the color/image overlay.

The `Pixellation` transition adds the following properties:
```csharp
transition
    .Amount(100f)                         // The amount (ratio) of pixellation
    .Origin(new Vector2(0.5f, 0.5f));     // The effect origin in normalized screen coords, i.e. the "zoom position"
```

Example:
```csharp
// Pixellate with a factor of 200x and fade to black
Pixellate.Cover(5f).Amount(200f);

// Reveal the new scene, less blocky and from bottom-right
Pixellate.Uncover(3f).Amount(70f).Origin(new(1,1));
```

## Dissolve transition

Dissolve transitions enable transition animations through the use of dissolve textures. A dissolve texture is just a grayscale image that the shader will sample from and use as a mask when rendering the transition color/image overlay. It starts with the dissolve color threshold set to fully black, gradually increasing it to fully white across the transition duration. At any given point in time, transition overlay pixels will only be drawn if the corresponding pixel in the dissolve texture is below the current threshold. This can effectively create infinite variations of transition animations.
The addon ships with a bunch of default dissolve textures (mostly stolen from [http://github.com/sempitern0](https://github.com/sempitern0/warp)) and made accessible via the DissolvePattern class, but you can also provide your own.

The `Dissolve` transition adds the following properties:
```csharp
transition
    .Pattern(p => p.Squares)    // The dissolve pattern texture to use (A DissolvePattern lambda, a custom Texture2D, or a path to a custom texture)
    .Invert()                   // Inverts the dissolve pattern texture, effectively reversing the visual effect of the dissolve animation.
    .FlipX()                    // Flips the X coords of the dissolve texture, creating a mirrored effect along the horizontal axis of the dissolve animation.
    .FlipY()                    // Flips the Y coords of the dissolve texture, creating a mirrored effect along the vertical axis of the dissolve animation.
    .Feather(0.01f);            // Sets the feathering amount for the dissolve effect, enabling smoother or sharper transitions at the dissolve edge.
```

Example using the default patterns:
```csharp
// Exit the scene using a red circle shape that shrinks from the screen edges and inwards, finally displaying my_overlay.png.
Dissolve.Cover(2f).Color(Colors.Red).Image("res://my_overlay.png").Pattern(p => p.Circle);

// Reveal the new scene, but start from the center and grow outwards
Dissolve.Uncover(2f).Color(Colors.Red).Image("res://my_overlay.png").Pattern(p => p.Circle).Invert();
```

## Custom loaders
For smooth loading of heavy scenes you may want to create a custom loader. However, under this framework loader scenes are nothing special - just transition (warp) to your loader scene normally, load your target scene however you want and transition (warp) to it when ready.
