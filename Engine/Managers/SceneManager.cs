using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlimeTogetherStrong.Engine.Managers;

public class SceneManager
{
    public static SceneManager Instance { get; private set; } = new SceneManager();

    private Scene _currentScene;
    private readonly Stack<Scene> _overlayScenes = new();
    private readonly Dictionary<string, Scene> _scenes = [];
    private string _pendingSceneName = null;
    private Scene _pendingOverlayPush = null;
    private bool _pendingOverlayPop = false;

    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }

    private SceneManager() { }

    public void LoadScene(string sceneName)
    {
        // Defer scene loading to avoid modifying collections during iteration
        _pendingSceneName = sceneName;
    }

    private void LoadSceneImmediate(string sceneName)
    {
        if (_scenes.TryGetValue(sceneName, out var scene))
        {
            // Clear all overlays when switching main scene
            while (_overlayScenes.Count > 0)
            {
                var overlay = _overlayScenes.Pop();
                overlay.Unload();
            }

            _currentScene?.Unload();
            scene.Load();
            scene.IsActive = true;
            _currentScene = scene;
        }
    }

    public void PushOverlay(Scene overlayScene)
    {
        // Defer overlay push to avoid modifying collections during iteration
        _pendingOverlayPush = overlayScene;
    }

    private void PushOverlayImmediate(Scene overlayScene)
    {
        // Deactivate current top scene (main or previous overlay)
        if (_overlayScenes.Count > 0)
        {
            _overlayScenes.Peek().IsActive = false;
        }
        else if (_currentScene != null)
        {
            _currentScene.IsActive = false;
        }

        // Push and load new overlay
        overlayScene.Load();
        overlayScene.IsActive = true;
        _overlayScenes.Push(overlayScene);
    }

    public void PopOverlay()
    {
        // Defer overlay pop to avoid modifying collections during iteration
        _pendingOverlayPop = true;
    }

    private void PopOverlayImmediate()
    {
        if (_overlayScenes.Count > 0)
        {
            var overlay = _overlayScenes.Pop();
            overlay.Unload();

            // Reactivate the scene below (another overlay or main scene)
            if (_overlayScenes.Count > 0)
            {
                _overlayScenes.Peek().IsActive = true;
            }
            else if (_currentScene != null)
            {
                _currentScene.IsActive = true;
            }
        }
    }

    public void AddScene(string sceneName, Scene scene)
    {
        _scenes[sceneName] = scene;
    }

    public void Update(GameTime gameTime)
    {
        // Only update the active scene (top overlay or main scene)
        if (_overlayScenes.Count > 0)
        {
            var topOverlay = _overlayScenes.Peek();
            if (topOverlay.IsActive)
            {
                topOverlay.Update(gameTime);
            }
        }
        else if (_currentScene != null && _currentScene.IsActive)
        {
            _currentScene.Update(gameTime);
        }

        // Process pending operations after update completes
        if (_pendingOverlayPush != null)
        {
            PushOverlayImmediate(_pendingOverlayPush);
            _pendingOverlayPush = null;
        }

        if (_pendingOverlayPop)
        {
            PopOverlayImmediate();
            _pendingOverlayPop = false;
        }

        if (_pendingSceneName != null)
        {
            LoadSceneImmediate(_pendingSceneName);
            _pendingSceneName = null;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw main scene first
        _currentScene?.Draw(spriteBatch);

        // Then draw all overlays on top (bottom to top)
        foreach (var overlay in _overlayScenes.Reverse())
        {
            overlay.Draw(spriteBatch);
        }
    }

    public Scene GetCurrentScene()
    {
        return _currentScene;
    }
}