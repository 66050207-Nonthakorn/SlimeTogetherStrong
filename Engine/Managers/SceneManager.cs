using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlimeTogetherStrong.Engine.Managers;

public class SceneManager
{
    public static SceneManager Instance { get; private set; } = new SceneManager();

    private Scene _currentScene;
    private readonly Dictionary<string, Scene> _scenes = [];

    private SceneManager() { }

    public void LoadScene(string sceneName)
    {
        if (_scenes.TryGetValue(sceneName, out var scene))
        {
            scene.Load();
            _currentScene = scene;
        }
    }

    public void AddScene(string sceneName, Scene scene)
    {
        _scenes[sceneName] = scene;
    }

    public void Update(GameTime gameTime)
    {
        _currentScene?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _currentScene?.Draw(spriteBatch);
    }

    public Scene GetCurrentScene()
    {
        return _currentScene;
    }
}