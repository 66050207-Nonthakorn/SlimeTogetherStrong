using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Engine.UI;

public class XPBar : UIElement
{
    private XPBarRenderer _barRenderer;
    private XPManager _xpManager;

    public XPBar(Vector2 position, Vector2 size, XPManager xpManager)
    {
        Position = position;
        _xpManager = xpManager;

        // Add bar renderer component to this UIElement
        _barRenderer = AddComponent<XPBarRenderer>();
        _barRenderer.Size = size;
        _barRenderer.XPManager = xpManager;
    }
}

// Custom Component for rendering the XP bar background and fill
public class XPBarRenderer : Component
{
    public Vector2 Size { get; set; }
    public XPManager XPManager { get; set; }
    private Texture2D _pixelTexture;
    private Color _fillColor = new Color(50, 120, 200, 180); // Blue with transparency
    private Color _backgroundColor = new Color(80, 80, 80, 120); // Gray with transparency

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        Vector2 pos = GameObject.Position;

        // Draw background (gray unfilled portion)
        spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(
                (int)pos.X,
                (int)pos.Y,
                (int)Size.X,
                (int)Size.Y
            ),
            _backgroundColor
        );

        // Draw filled portion (blue)
        float fillPercentage = XPManager?.GetXPPercentage() ?? 0f;
        int fillWidth = (int)(Size.X * fillPercentage);

        if (fillWidth > 0)
        {
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle(
                    (int)pos.X,
                    (int)pos.Y,
                    fillWidth,
                    (int)Size.Y
                ),
                _fillColor
            );
        }
    }
}
