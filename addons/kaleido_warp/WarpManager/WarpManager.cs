using System;
using Godot;

namespace KaleidoWarp;
#nullable enable

/// <summary>
/// A manager for handling scene transitions (warps) with customizable exit and entry effects. It supports warping to new scenes specified by file path, PackedScene, or directly to a Node instance. The WarpManager manages the transition states and ensures that the appropriate transition effects are played during scene changes. It also provides functionality to abort or cancel ongoing warps, allowing for flexible control over the transition process.
/// </summary>
public partial class WarpManager : CanvasLayer
{
	static WarpManager? _instance;

	/// <summary>
	/// The singleton instance of the <see cref="WarpManager"/>. This property provides easy access to the global transition manager, which must be set as an autoload in the project settings.
	/// </summary>
	public static WarpManager Instance => _instance ?? throw new InvalidOperationException($"{nameof(WarpManager)} instance not set. Ensure {nameof(WarpManager)} is added as an autoload in the project settings.");

	enum WarpState
	{
		Ready,
		ExitOld,
		EnterNew,
		Finished,
	}

	WarpState _state = WarpState.Finished;
	Node? _targetNode;
	PackedScene? _targetPacked;
	string? _targetPath;
	Transition? _exitOld;
	Transition? _enterNew;
	bool _cancelled;
	Action? _queue;

	/// <summary>
	/// Gets a value indicating whether the current instance is actively processing a transition.
	/// </summary>
	public bool IsBusy => _queue != null || _state != WarpState.Finished;

	public override void _Ready()
	{
		_instance = this;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_state == WarpState.Ready)
		{
			_exitOld?.Play();
			_state = WarpState.ExitOld;
		}

		if (_state == WarpState.ExitOld)
		{
			if (_exitOld?.IsFinished ?? true)
			{
				if (_targetPath != null)
					GetTree().ChangeSceneToFile(_targetPath);
				else if (_targetPacked != null)
					GetTree().ChangeSceneToPacked(_targetPacked);
				else if (_targetNode != null)
					GetTree().ChangeSceneToNode(_targetNode);

				_targetPath = null;
				_targetPacked = null;
				_targetNode = null;

				_exitOld?.Stop();
				_exitOld?.QueueFree();
				_exitOld = null;

				_enterNew?.Play();
				_state = WarpState.EnterNew;
			}
		}

		if (_state == WarpState.EnterNew)
		{
			if (_enterNew?.IsFinished ?? true)
			{
				_enterNew?.Stop();
				_enterNew?.QueueFree();
				_enterNew = null;
				_state = WarpState.Finished;
			}
		}

		if (_state == WarpState.Finished)
		{
			if (_queue != null)
			{
				var action = _queue;
				_queue = null;
				action();
			}
		}
	}

	/// <summary>
	/// Instantly aborts any ongoing warp, stopping and freeing any active transitions and clearing the target scene information. This method can be used to immediately cancel a warp without playing any exit or entry effects.
	/// </summary>
	public void Abort()
	{
		_enterNew?.QueueFree();
		_enterNew = null;

		_exitOld?.QueueFree();
		_exitOld = null;

		_targetPath = null;
		_targetPacked = null;
		_targetNode?.QueueFree();
		_targetNode = null;

		_queue = null;
		_state = WarpState.Finished;
	}

	/// <summary>
	/// Cancels the current warp operation gracefully, reverting or forwarding the process based on the current warp state.
	/// </summary>
	/// <param name="maxDuration">The maximum duration, in seconds, for the cancellation to complete.</param>
	public void Cancel(float maxDuration)
	{
		if (_state == WarpState.Ready)
		{
			Abort();
		}
		else if (_state == WarpState.ExitOld)
		{
			_exitOld?.Cancel(maxDuration, true);
			_enterNew?.QueueFree();
			_enterNew = null;

			_targetPath = null;
			_targetPacked?.Free();
			_targetPacked = null;
			_targetNode?.QueueFree();
			_targetNode = null;
		}
		else if (_state == WarpState.EnterNew)
		{
			_enterNew?.Cancel(maxDuration, false);
		}

		_queue = null;
	}

	/// <summary>
	/// Queues a transition to the specified scene file, optionally applying transition effects when leaving the current scene and entering the new one.
	/// </summary>
	/// <remarks>This is a deferred operation, even if no transition effects are specified.</remarks>
	/// <param name="scenePath">The file path of the scene to warp to. Must be a valid, non-null, and non-empty path.</param>
	/// <param name="transitionOut">An optional transition effect to apply when leaving the current scene. If null, no transition is applied.</param>
	/// <param name="transitionIn">An optional transition effect to apply when entering the new scene. If null, no transition is applied.</param>
	public void WarpToFile(string scenePath, Transition? transitionOut, Transition? transitionIn)
	{
		_queue = () =>
		{
			InitWarp(transitionOut, transitionIn);
			_targetPath = scenePath;
			_queue = null;
		};
	}


	/// <summary>
	/// Queues a transition to the specified packed scene, optionally applying transition effects when leaving the current scene and entering the new one.
	/// </summary>
	/// <remarks>This is a deferred operation, even if no transition effects are specified.</remarks>
	/// <param name="packedScene">The packed scene to transition to. Must be a valid, non-null packed scene.</param>
	/// <param name="transitionOut">An optional transition effect to apply when leaving the current scene. If null, no transition is applied.</param>
	/// <param name="transitionIn">An optional transition effect to apply when entering the new scene. If null, no transition is applied.</param>
	public void WarpToPacked(PackedScene packedScene, Transition? transitionOut, Transition? transitionIn)
	{
		_queue = () =>
		{
			InitWarp(transitionOut, transitionIn);
			_targetPacked = packedScene;
			_queue = null;
		};
	}

	/// <summary>
	/// Queues a transition to the specified node, optionally applying transition effects when leaving the current scene and entering the new one.
	/// </summary>
	/// <remarks>This is a deferred operation, even if no transition effects are specified.</remarks>
	/// <param name="sceneNode">The node to transition to. Must be a valid, non-null node.</param>
	/// <param name="transitionOut">An optional transition effect to apply when leaving the current scene. If null, no transition is applied.</param>
	/// <param name="transitionIn">An optional transition effect to apply when entering the new scene. If null, no transition is applied.</param>
	public void WarpToNode(Node sceneNode, Transition? transitionOut, Transition? transitionIn)
	{
		_queue = () =>
		{
			InitWarp(transitionOut, transitionIn);
			_targetNode = sceneNode;
			_queue = null;
		};
	}

	void InitWarp(Transition? transitionOut, Transition? transitionIn)
	{
		Abort();

		_exitOld = transitionOut;
		_enterNew = transitionIn;

		if (_exitOld != null)
			AddChild(_exitOld);

		if (_enterNew != null)
			AddChild(_enterNew);

		_state = WarpState.Ready;
	}
}
