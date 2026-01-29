using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;

namespace SlimeTogetherStrong.Engine.UI;

public class UpgradeCard : UIElement
{
    private Button _button;
    private GameObject _entityImageObject;
    private GameObject _statsIconObject;
    private GameObject _entityTextObject;
    private GameObject _statsTextObject;
    
    public string EntityName { get; private set; }
    public string StatsName { get; private set; }
    public int StatsIncrease { get; private set; }

    public UpgradeCard(
        Vector2 position,
        Vector2 size,
        Texture2D entityTexture,
        Texture2D statsIconTexture,
        string entityName,
        string statsName,
        int statsIncrease,
        Action onClick,
        SpriteFont font)
    {
        Position = position;
        EntityName = entityName;
        StatsName = statsName;
        StatsIncrease = statsIncrease;

        // Card button background
        _button = AddComponent<Button>();
        _button.Size = size;
        _button.OnClick = onClick;
        _button.IsShowFill = true;
        _button.FillColor = new Color(60, 60, 60, 220);
        _button.IsShowOutline = true;
        _button.OutlineColor = Color.White;
        _button.OutlineThickness = 3;

        // Calculate layout
        float imageHeight = size.Y * 0.65f; // 65% for image area
        float textHeight = size.Y * 0.35f;  // 35% for text area
        float padding = 10f;

        // Entity image (centered in upper portion)
        _entityImageObject = new GameObject();
        float imageSize = imageHeight - padding * 2;
        _entityImageObject.Position = new Vector2(
            position.X + size.X / 2,
            position.Y + imageHeight / 2
        );
        
        var entityRenderer = _entityImageObject.AddComponent<SpriteRenderer>();
        entityRenderer.Texture = entityTexture;
        
        if (entityTexture != null)
        {
            // Scale to fit
            float scale = Math.Min(imageSize / entityTexture.Width, imageSize / entityTexture.Height);
            _entityImageObject.Scale = new Vector2(scale, scale);
            entityRenderer.Origin = new Vector2(entityTexture.Width / 2f, entityTexture.Height / 2f);
        }
        
        AddChild(_entityImageObject);

        // Stats icon (bottom right of image area)
        if (statsIconTexture != null)
        {
            _statsIconObject = new GameObject();
            float iconSize = 32f;
            _statsIconObject.Position = new Vector2(
                position.X + size.X - iconSize - padding,
                position.Y + imageHeight - iconSize - padding
            );
            
            var iconRenderer = _statsIconObject.AddComponent<SpriteRenderer>();
            iconRenderer.Texture = statsIconTexture;
            
            float iconScale = iconSize / Math.Max(statsIconTexture.Width, statsIconTexture.Height);
            _statsIconObject.Scale = new Vector2(iconScale, iconScale);
            
            AddChild(_statsIconObject);
        }

        // Text: "Upgrade [Entity]'s" (first line)
        _entityTextObject = new GameObject();
        _entityTextObject.Position = new Vector2(
            position.X + size.X / 2,
            position.Y + imageHeight + 20
        );
        
        var entityText = _entityTextObject.AddComponent<Text>();
        entityText.Font = font;
        entityText.Content = $"Upgrade {entityName}'s";
        entityText.Color = Color.White;
        var entityTextSize = entityText.MeasureText();
        entityText.Origin = entityTextSize / 2;
        
        AddChild(_entityTextObject);

        // Text: "[Stats Name]" (second line)
        _statsTextObject = new GameObject();
        _statsTextObject.Position = new Vector2(
            position.X + size.X / 2,
            position.Y + imageHeight + 45
        );
        _statsTextObject.Scale = new Vector2(1.3f, 1.3f);
        
        var statsText = _statsTextObject.AddComponent<Text>();
        statsText.Font = font;
        statsText.Content = statsName;
        statsText.Color = Color.Yellow;
        var statsTextSize = statsText.MeasureText();
        statsText.Origin = statsTextSize / 2;
        
        AddChild(_statsTextObject);
    }

    // Highlight card on hover
    public void SetHighlighted(bool highlighted)
    {
        if (highlighted)
        {
            _button.FillColor = new Color(80, 80, 80, 240);
            _button.OutlineColor = Color.Yellow;
        }
        else
        {
            _button.FillColor = new Color(60, 60, 60, 220);
            _button.OutlineColor = Color.White;
        }
    }
}
