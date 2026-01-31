using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Engine.UI;

public class Button : Component
{
    private static Texture2D _pixelTexture;

    public static bool WasClickedThisFrame { get; private set; } = false;

    public Vector2 Size { get; set; } = new Vector2(200, 50);
    public Action OnClick { get; set; }

    public Color FillColor { get; set; } = Color.DarkGray;
    public Color OutlineColor { get; set; } = Color.Red;
    public int OutlineThickness { get; set; } = 1;
    public bool IsShowOutline { get; set; } = false;
    public bool IsShowFill { get; set; } = true;

    public static void ResetClickFlag()
    {
        WasClickedThisFrame = false;
    }

    public override void Update(GameTime gameTime)
    {
        var mousePosition = InputManager.Instance.GetMousePosition();
        var buttonRectangle = new Rectangle(
            (int)base.GameObject.Position.X,
            (int)base.GameObject.Position.Y,
            (int)Size.X,
            (int)Size.Y
        );

        if (buttonRectangle.Contains(mousePosition))
        {
            if (InputManager.Instance.IsMouseButtonPressed(0))
            {
                WasClickedThisFrame = true;
                OnClick?.Invoke();
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Create pixel texture once and cache it
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        // Button uses top-left position
        int buttonX = (int)GameObject.Position.X;
        int buttonY = (int)GameObject.Position.Y;

        // Draw fill
        if (IsShowFill)
        {
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle(buttonX, buttonY, (int)Size.X, (int)Size.Y),
                FillColor
            );
        }

        // Draw outline
        if (IsShowOutline)
        {
            // Top outline
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle(
                    buttonX - OutlineThickness,
                    buttonY - OutlineThickness,
                    (int)Size.X + OutlineThickness * 2,
                    OutlineThickness
                ),
                OutlineColor
            );

            // Bottom outline
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle(
                    buttonX - OutlineThickness,
                    buttonY + (int)Size.Y,
                    (int)Size.X + OutlineThickness * 2,
                    OutlineThickness
                ),
                OutlineColor
            );

            // Left outline
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle(
                    buttonX - OutlineThickness,
                    buttonY - OutlineThickness,
                    OutlineThickness,
                    (int)Size.Y + OutlineThickness * 2
                ),
                OutlineColor
            );

            // Right outline
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle(
                    buttonX + (int)Size.X,
                    buttonY - OutlineThickness,
                    OutlineThickness,
                    (int)Size.Y + OutlineThickness * 2
                ),
                OutlineColor
            );
        }
    }
}