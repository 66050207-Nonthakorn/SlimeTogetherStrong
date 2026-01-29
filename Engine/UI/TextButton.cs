using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;

namespace SlimeTogetherStrong.Engine.UI;

public class TextButton : UIElement
{
    private Button _button;
    private GameObject _textObject;
    private Text _text;

    public TextButton(string text, Vector2 position, Vector2 size, Action onClick, SpriteFont font)
    {
        // Position is top-left
        Position = position;

        // Add button component to this UIElement
        _button = AddComponent<Button>();
        _button.Size = size;
        _button.OnClick = onClick;
        _button.IsShowFill = true;
        _button.FillColor = new Color(50, 50, 50, 200);
        _button.IsShowOutline = true;
        _button.OutlineColor = Color.White;
        _button.OutlineThickness = 3;

        // Create text as a child GameObject centered on button
        _textObject = new GameObject
        {
            Position = new Vector2(position.X + size.X / 2, position.Y + size.Y / 2)
        };

        _text = _textObject.AddComponent<Text>();
        _text.Font = font;
        _text.Content = text;
        _text.Color = Color.White;
        var textSize = _text.MeasureText();
        _text.Origin = textSize / 2;

        AddChild(_textObject);
    }

    // Property accessors for easy customization
    public Color FillColor
    {
        get => _button.FillColor;
        set => _button.FillColor = value;
    }

    public Color OutlineColor
    {
        get => _button.OutlineColor;
        set => _button.OutlineColor = value;
    }

    public int OutlineThickness
    {
        get => _button.OutlineThickness;
        set => _button.OutlineThickness = value;
    }

    public Color TextColor
    {
        get => _text.Color;
        set => _text.Color = value;
    }

    public string Text
    {
        get => _text.Content;
        set
        {
            _text.Content = value;
            // Re-center text when content changes
            var textSize = _text.MeasureText();
            _text.Origin = textSize / 2;
        }
    }
}
