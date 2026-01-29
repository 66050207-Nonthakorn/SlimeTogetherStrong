using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Engine.UI;

public class Button : Component
{
    public Vector2 Size { get; set; } = new Vector2(200, 50);
    public Action OnClick { get; set; }

    public Color FillColor { get; set; } = Color.DarkGray;
    public Color OutlineColor { get; set; } = Color.Red;
    public int OutlineThickness { get; set; } = 1;
    public bool IsShowOutline { get; set; } = false;
    public bool IsShowFill { get; set; } = true;

    public override void Update(GameTime gameTime)
    {
        var mousePosition = InputManager.Instance.GetMousePosition();
        // Button uses top-left position
        var buttonRectangle = new Rectangle(
            (int)base.GameObject.Position.X,
            (int)base.GameObject.Position.Y,
            (int)Size.X,
            (int)Size.Y
        );

        if (buttonRectangle.Contains(mousePosition))
        {
            // Use IsMouseButtonPressed instead of IsMouseButtonDown to prevent multiple triggers
            if (InputManager.Instance.IsMouseButtonPressed(0))
            {            
                OnClick?.Invoke();
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D dummyTexture = new(spriteBatch.GraphicsDevice, 1, 1);
        dummyTexture.SetData([Color.White]);

        // Button uses top-left position
        int buttonX = (int)GameObject.Position.X;
        int buttonY = (int)GameObject.Position.Y;

        // Draw fill
        if (IsShowFill)
        {
            spriteBatch.Draw(
                dummyTexture,
                new Rectangle(buttonX, buttonY, (int)Size.X, (int)Size.Y),
                FillColor
            );
        }

        // Draw outline
        if (IsShowOutline)
        {
            // Top outline
            spriteBatch.Draw(
                dummyTexture,
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
                dummyTexture,
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
                dummyTexture,
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
                dummyTexture,
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