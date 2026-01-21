using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SlimeTogetherStrong.Engine.Components;
using System.Linq;

namespace SlimeTogetherStrong.Engine;

public class GameObject
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public Vector2 Scale { get; set; } = Vector2.One;

    public bool Active { get; set; } = true;
    public string Tag { get; set; }

    private readonly List<Component> _components = [];

    public T AddComponent<T>() where T : Component, new()
    {
        T component = new();
        _components.Add(component);

        component.GameObject = this;
        component.Initialize();
        
        return component;
    }

    public T GetComponent<T>() where T : Component
    {
        return _components.OfType<T>().FirstOrDefault();
    }

    public void RemoveComponent<T>() where T : Component
    {
        T component = GetComponent<T>();
        if (component != null)
        {
            _components.Remove(component);
        }
    }

    public virtual void Initialize()
    {
        foreach (var component in _components)
        {
            component.Initialize();
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        if (!Active) return;
        foreach (var component in _components)
        {
            if (component.Enabled)
            {
                component.Update(gameTime);
            }   
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!Active) return;
        foreach (var component in _components)
        {
            if (component.Enabled)
            {
                component.Draw(spriteBatch);
            }
        }
    }
}