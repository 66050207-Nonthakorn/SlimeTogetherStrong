using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Engine.UI;

public class VerticalBarWithIcon : UIElement
{
    private GameObject _iconObject;
    private GameObject _barBackgroundObject;

    private Texture2D _iconTexture;

    private Vector2 _basePosition;
    private Vector2 _iconSize;
    private Vector2 _barSize;
    private Color _fillColor;
    private Color _backgroundColor;
    private Color _outlineColor;
    private int _outlineThickness;

    private Func<float> _valueGetter;
    private Func<string> _textGetter; // For displaying text like "500/1000"

    public VerticalBarWithIcon(
        Vector2 position,
        Vector2 iconSize,
        Vector2 barSize,
        Texture2D iconTexture,
        Color fillColor,
        Color backgroundColor,
        Color outlineColor,
        int outlineThickness,
        Func<float> valueGetter,
        Func<string> textGetter = null)
    {
        _basePosition = position;
        _iconSize = iconSize;
        _barSize = barSize;
        _iconTexture = iconTexture;
        _fillColor = fillColor;
        _backgroundColor = backgroundColor;
        _outlineColor = outlineColor;
        _outlineThickness = outlineThickness;
        _valueGetter = valueGetter;
        _textGetter = textGetter;

        Position = position;
    }

    public override void Initialize()
    {
        base.Initialize();
        
        // Create icon at top
        _iconObject = new GameObject();
        _iconObject.Position = _basePosition;
        
        var iconRenderer = _iconObject.AddComponent<SpriteRenderer>();
        iconRenderer.Texture = _iconTexture;
        
        if (_iconTexture != null)
        {
            float scaleX = _iconSize.X / _iconTexture.Width;
            float scaleY = _iconSize.Y / _iconTexture.Height;
            _iconObject.Scale = new Vector2(scaleX, scaleY);
        }
        
        AddChild(_iconObject);
        
        // Calculate bar position (below icon with small gap, centered under icon)
        float gap = 5f;
        float barCenterOffset = (_iconSize.X - _barSize.X) / 2; // Center bar under icon
        Vector2 barTopLeft = new Vector2(
            _basePosition.X + barCenterOffset,
            _basePosition.Y + _iconSize.Y + gap
        );
        
        // Create bar with custom renderer
        _barBackgroundObject = new GameObject();
        _barBackgroundObject.Position = barTopLeft;
        var barRenderer = _barBackgroundObject.AddComponent<VerticalBarRenderer>();
        barRenderer.Size = _barSize;
        barRenderer.FillColor = _fillColor;
        barRenderer.BackgroundColor = _backgroundColor;
        barRenderer.OutlineColor = _outlineColor;
        barRenderer.OutlineThickness = _outlineThickness;
        barRenderer.ValueGetter = _valueGetter;
        barRenderer.TextGetter = _textGetter;

        AddChild(_barBackgroundObject);
    }
}

// Custom Component for rendering vertical bar with outline
public class VerticalBarRenderer : Component
{
    public Vector2 Size { get; set; }
    public Color FillColor { get; set; }
    public Color BackgroundColor { get; set; }
    public Color OutlineColor { get; set; }
    public int OutlineThickness { get; set; }
    public Func<float> ValueGetter { get; set; }
    public Func<string> TextGetter { get; set; }
    
    private Texture2D _pixelTexture;

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        Vector2 pos = GameObject.Position;
        float percentage = Math.Clamp(ValueGetter?.Invoke() ?? 0f, 0f, 1f);
        float fillHeight = Size.Y * percentage;
        
        // Draw background
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                (int)pos.X,
                (int)pos.Y,
                (int)Size.X,
                (int)Size.Y
            ),
            BackgroundColor
        );
        
        // Draw fill (from bottom to top)
        if (fillHeight > 0)
        {
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle(
                    (int)pos.X,
                    (int)(pos.Y + Size.Y - fillHeight),
                    (int)Size.X,
                    (int)fillHeight
                ),
                FillColor
            );
        }
        
        // Draw outline (4 sides)
        // Top
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                (int)(pos.X - OutlineThickness),
                (int)(pos.Y - OutlineThickness),
                (int)(Size.X + OutlineThickness * 2),
                OutlineThickness
            ),
            OutlineColor
        );
        
        // Bottom
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                (int)(pos.X - OutlineThickness),
                (int)(pos.Y + Size.Y),
                (int)(Size.X + OutlineThickness * 2),
                OutlineThickness
            ),
            OutlineColor
        );
        
        // Left
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                (int)(pos.X - OutlineThickness),
                (int)pos.Y,
                OutlineThickness,
                (int)Size.Y
            ),
            OutlineColor
        );
        
        // Right
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                (int)(pos.X + Size.X),
                (int)pos.Y,
                OutlineThickness,
                (int)Size.Y
            ),
            OutlineColor
        );

        // Draw text centered inside the bar
        if (TextGetter != null)
        {
            var font = ResourceManager.Instance.GetFont("DefaultFont");
            if (font != null)
            {
                string text = TextGetter();
                Vector2 textSize = font.MeasureString(text);
                float textX = pos.X + (Size.X - textSize.X) / 2; // Center horizontally
                float textY = pos.Y + (Size.Y - textSize.Y) / 2; // Center vertically in bar
                spriteBatch.DrawString(font, text, new Vector2(textX, textY), Color.White);
            }
        }
    }
}
