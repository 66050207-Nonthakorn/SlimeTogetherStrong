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
        foreach (var gameObject in GameObjects)
        {
            // If it's a UIElement, use DrawElement to draw with children
            if (gameObject is UIElement uiElement)
            {
                uiElement.DrawElement(spriteBatch);
            }
            else
            {
                gameObject.Draw(spriteBatch);
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