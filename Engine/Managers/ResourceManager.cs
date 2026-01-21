using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace SlimeTogetherStrong.Engine.Managers;

public class ResourceManager
{
    public static ResourceManager Instance { get; private set; } = new ResourceManager();

    private readonly Dictionary<string, Texture2D> _textures = [];
    private readonly Dictionary<string, SpriteFont> _fonts = [];

    private ResourceManager() { }

    public void LoadTexture(string name, Texture2D texture)
    {
        _textures[name] = texture;
    }
    
    public Texture2D GetTexture(string name)
    {
        return _textures.TryGetValue(name, out var texture) ? texture : null;
    }

    public void LoadFont(string name, SpriteFont font)
    {
        _fonts[name] = font;
    }

    public SpriteFont GetFont(string name)
    {
        return _fonts.TryGetValue(name, out var font) ? font : null;
    }
}