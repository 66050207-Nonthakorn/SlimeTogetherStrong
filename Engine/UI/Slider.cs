using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Engine.UI;

public class Slider : Component
{
    public float MinValue { get; set; } = 0f;
    public float MaxValue { get; set; } = 1f;
    public float CurrentValue { get; set; } = 0.5f;

    public Vector2 Size { get; set; } = new Vector2(200, 20);
    public Color BaseColor { get; set; } = Color.Gray;
    public Color TrackColor { get; set; } = Color.DarkGray;
    
    public Action<float> OnValueChanged { get; set; }

    private bool _isDragging = false;

    public override void Update(GameTime gameTime)
    {
        // Check if mouse is over track
        Vector2 mousePosition = InputManager.Instance.GetMousePosition();
        Rectangle trackRect = new(
            (int)base.GameObject.Position.X,
            (int)(base.GameObject.Position.Y + Size.Y / 4),
            (int)Size.X,
            (int)(Size.Y / 2)
        );

        if (trackRect.Contains(mousePosition) && InputManager.Instance.IsMouseButtonDown(0))
        {
            _isDragging = true;
        }
        else if (InputManager.Instance.IsMouseButtonReleased(0))
        {
            _isDragging = false;
        }

        if (!_isDragging) return;

        float relativeX = mousePosition.X - base.GameObject.Position.X;
        float newValue = MinValue + relativeX / Size.X * (MaxValue - MinValue);
        newValue = MathHelper.Clamp(newValue, MinValue, MaxValue);

        if (newValue != CurrentValue)
        {
            CurrentValue = newValue;
            OnValueChanged?.Invoke(CurrentValue);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D dummyTexture = new(spriteBatch.GraphicsDevice, 1, 1);
        // Draw base
        dummyTexture.SetData([BaseColor]);
        spriteBatch.Draw(
            dummyTexture,
            new Rectangle(
                (int)GameObject.Position.X,
                (int)GameObject.Position.Y,
                (int)Size.X,
                (int)Size.Y
            ),
            BaseColor
        );

        // Draw track
        dummyTexture.SetData([TrackColor]);
        spriteBatch.Draw(
            dummyTexture,
            new Rectangle(
                (int)GameObject.Position.X,
                (int)GameObject.Position.Y,
                (int)(Size.X * (CurrentValue - MinValue) / (MaxValue - MinValue)),
                (int)Size.Y
            ),
            TrackColor
        );
    }
}