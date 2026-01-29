using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.UI;

namespace SlimeTogetherStrong.Engine;

public abstract class Scene
{
    protected List<GameObject> GameObjects { get; } = [];
    public bool IsActive { get; set; } = true;

    public virtual void Load()
    {
        foreach (var gameObject in GameObjects)
        {
            gameObject.Initialize();
        }
    }

    public void Unload()
    {
        GameObjects.Clear();
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (var gameObject in GameObjects)
        {
            gameObject.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw regular game objects first
        foreach (var gameObject in GameObjects)
        {
            if (gameObject is not UIElement)
            {
                gameObject.Draw(spriteBatch);
            }
        }

        // Draw UI elements last (on top)
        foreach (var gameObject in GameObjects)
        {
            if (gameObject is UIElement uiElement)
            {
                uiElement.DrawElement(spriteBatch);
            }
        }
    }

    public void AddGameObject(GameObject gameObject)
    {
        GameObjects.Add(gameObject);
    }

    public void RemoveGameObject(GameObject gameObject)
    {
        GameObjects.Remove(gameObject);
    }
}