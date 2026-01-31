using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;

namespace SlimeTogetherStrong.Engine.UI;

public class RoundedRectangleRenderer : Component
{
    // Cached pixel texture for drawing (shared across all instances)
    private static Texture2D _pixelTexture;

    public Vector2 Size { get; set; } = new Vector2(200, 200);
    public Color FillColor { get; set; } = Color.Gray;
    public Color OutlineColor { get; set; } = Color.White;
    public int OutlineThickness { get; set; } = 2;
    public int CornerRadius { get; set; } = 10;

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Create pixel texture once and cache it
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        int x = (int)GameObject.Position.X;
        int y = (int)GameObject.Position.Y;
        int w = (int)Size.X;
        int h = (int)Size.Y;
        int r = CornerRadius;

        // Draw filled rounded rectangle (simplified - just rectangles, no actual rounded corners for now)
        // Main body
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, w, h), FillColor);

        // Draw outline
        if (OutlineThickness > 0)
        {
            // Top
            spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, w, OutlineThickness), OutlineColor);
            // Bottom
            spriteBatch.Draw(_pixelTexture, new Rectangle(x, y + h - OutlineThickness, w, OutlineThickness), OutlineColor);
            // Left
            spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, OutlineThickness, h), OutlineColor);
            // Right
            spriteBatch.Draw(_pixelTexture, new Rectangle(x + w - OutlineThickness, y, OutlineThickness, h), OutlineColor);
        }
    }
}
