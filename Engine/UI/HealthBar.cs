using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using System.Reflection;

namespace SlimeTogetherStrong.Engine.UI;

public class HealthBar : Component
{
    public Vector2 Size { get; set; } = new Vector2(40, 5);
    public Vector2 Offset { get; set; } = new Vector2(0, -30); // Above the entity
    public Color BackgroundColor { get; set; } = new Color(40, 40, 40);
    public Color FillColor { get; set; } = Color.Red;
    public Color BorderColor { get; set; } = Color.Black;
    public int BorderThickness { get; set; } = 1;

    private HealthComponent _healthComponent;
    private PropertyInfo _currentHPProperty;
    private PropertyInfo _maxHPProperty;

    public override void Initialize()
    {
        _healthComponent = GameObject.GetComponent<HealthComponent>();
        
        // If no HealthComponent, check for custom health properties (like Ally)
        if (_healthComponent == null)
        {
            var type = GameObject.GetType();
            _currentHPProperty = type.GetProperty("CurrentHP");
            _maxHPProperty = type.GetProperty("MaxHP");
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        int currentHP, maxHP;
        
        // Get health from HealthComponent or custom properties
        if (_healthComponent != null)
        {
            if (_healthComponent.IsDead()) return;
            currentHP = _healthComponent.CurrentHP;
            maxHP = _healthComponent.MaxHP;
        }
        else if (_currentHPProperty != null && _maxHPProperty != null)
        {
            currentHP = (int)_currentHPProperty.GetValue(GameObject);
            maxHP = (int)_maxHPProperty.GetValue(GameObject);
            if (currentHP <= 0) return;
        }
        else
        {
            return; // No health data available
        }

        Texture2D pixel = new(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData([Color.White]);

        // Calculate position centered above the entity
        Vector2 barPosition = GameObject.Position + Offset;
        barPosition.X -= Size.X / 2; // Center horizontally

        // Draw border
        if (BorderThickness > 0)
        {
            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)barPosition.X - BorderThickness,
                    (int)barPosition.Y - BorderThickness,
                    (int)Size.X + BorderThickness * 2,
                    (int)Size.Y + BorderThickness * 2
                ),
                BorderColor
            );
        }

        // Draw background
        spriteBatch.Draw(
            pixel,
            new Rectangle(
                (int)barPosition.X,
                (int)barPosition.Y,
                (int)Size.X,
                (int)Size.Y
            ),
            BackgroundColor
        );

        // Draw health fill
        float healthPercentage = (float)currentHP / maxHP;
        int fillWidth = (int)(Size.X * healthPercentage);

        if (fillWidth > 0)
        {
            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)barPosition.X,
                    (int)barPosition.Y,
                    fillWidth,
                    (int)Size.Y
                ),
                FillColor
            );
        }
    }
}
