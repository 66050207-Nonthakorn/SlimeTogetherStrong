using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Engine.UI;

public class Button : Component
{
    public string Text { get; set; }
    public SpriteFont Font { get; set; }
    public Color BackgroundColor { get; set; } = Color.Green;
    public Color HoverColor { get; set; } = Color.DarkGreen;
    public Color TextColor { get; set; } = Color.White;
    public Vector2 Size { get; set; } = new Vector2(200, 50);
    public Action OnClick { get; set; }

    private Color _currentColor = Color.Green; 

    public override void Update(GameTime gameTime)
    {
        var mousePosition = InputManager.Instance.GetMousePosition();
        var buttonRectangle = new Rectangle((int)base.GameObject.Position.X, (int)base.GameObject.Position.Y, (int)Size.X, (int)Size.Y);

        if (buttonRectangle.Contains(mousePosition))
        {
            _currentColor = HoverColor;

            if (InputManager.Instance.IsMouseButtonDown(0))
            {            
                OnClick?.Invoke();
            }
        }
        else
        {
            _currentColor = BackgroundColor;
        }
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw button background
        Texture2D rectTexture = new(spriteBatch.GraphicsDevice, 1, 1);
        rectTexture.SetData([_currentColor]);

        Rectangle rectangle = new(
            (int)base.GameObject.Position.X,
            (int)base.GameObject.Position.Y,
            (int)Size.X,
            (int)Size.Y
        );
        spriteBatch.Draw(rectTexture, rectangle, _currentColor);

        // Draw button text
        if (Font != null)
        {
            Vector2 textSize = Font.MeasureString(Text);
            Vector2 textPosition = new(
                base.GameObject.Position.X + (Size.X - textSize.X) / 2,
                base.GameObject.Position.Y + (Size.Y - textSize.Y) / 2
            );

            spriteBatch.DrawString(Font, Text, textPosition, TextColor);
        }
    }
}