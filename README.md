# KaleidoWarp for Godot

A flexible and extensible scene transition addon for Godot (.NET/C#) that allows for smooth, animated transitions between scenes.

## Minimal Example
```csharp
using KaleidoWarp;

WarpManager.Instance.WarpToFile("res://Scenes/Scene2.tscn", ColorFade.Cover(), ColorFade.Uncover());
```

Or slightly more elaborate:
```csharp
using KaleidoWarp;

WarpManager.Instance.WarpToFile(
    "res://Scenes/Scene2.tscn",
    ColorFade.Cover().Color(Colors.Blue),
    Voronoi.Uncover().Color(Colors.Blue).Angle(45)
);
```

---

## WarpManager API

The primary API mirrors Godot's default scene navigation while adding optional transitions:
```csharp
public void WarpToFile(string scenePath, Transition? transitionOut, Transition? transitionIn)
public void WarpToPacked(PackedScene packedScene, Transition? transitionOut, Transition? transitionIn)
public void WarpToNode(Node sceneNode, Transition? transitionOut, Transition? transitionIn)
```

---

## Transitions

KaleidoWarp comes with 4 built-in transition types, each individually configurable:

| Transition | Description |
|------------|-------------|
| `ColorFade` | Fades the screen to an opaque overlay |
| `Voronoi` | A randomized bubbly pattern that sweeps across the screen at a given angle |
| `SuperMario` | A pixellating effect reminiscent of the classic Super Mario pixel fade |
| `Dissolve` | Uses a monochrome pattern texture to define when and where each screen pixel is overlaid and blended |

---

## Defining Transitions

By convention each transition type implements two static factory methods:
```csharp
// Gradually covers the screen — scene exit/outro
public static SomeTransition Cover(float duration);

// Gradually uncovers the screen — scene entry/intro
public static SomeTransition Uncover(float duration);
```

---

## Transition API

All transitions inherit from the abstract base class `Transition` and expose a fluent configuration API:
```csharp
transition
    .Duration(1f)               // Duration in seconds
    .Color(Colors.Black)        // Base color of the transition overlay
    .Image("res://overlay.png") // Optional overlay image (path or Texture2D)
    .Ease(Tween.EaseType.InOut) // Easing across the transition duration
    .TransType(Tween.TransitionType.Quad) // Animation curve
    .Reverse();                 // Reverses animation direction
```

> **Note:** `Duration()` and `Reverse()` are typically not needed directly since they are initialized by `Cover()`/`Uncover()`.

---

## ColorFade

A basic fade with no additional properties beyond the base API.
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
