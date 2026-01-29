using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;

namespace SlimeTogetherStrong.Engine.UI;

public class UIElement : GameObject
{
    protected List<GameObject> Children { get; } = [];

    public void AddChild(GameObject child)
    {
        Children.Add(child);
    }

    public void RemoveChild(GameObject child)
    {
        Children.Remove(child);
    }

    public override void Initialize()
    {
        base.Initialize();
        foreach (var child in Children)
        {
            child.Initialize();
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        foreach (var child in Children)
        {
            child.Update(gameTime);
        }
    }

    public void DrawElement(SpriteBatch spriteBatch)
    {
        // Draw self
        base.Draw(spriteBatch);
        
        // Draw children
        foreach (var child in Children)
        {
            child.Draw(spriteBatch);
        }
    }
}
