using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Game;

public class GameScene : Scene
{
    private Texture2D _pixelTexture;
    private Player _player;
    private List<GameObject> _objectsToRemove = new List<GameObject>();
    private List<GameObject> _objectsToAdd = new List<GameObject>();  // เพิ่มบรรทัดนี้                                                                                                                 


    public GameScene()
    {
        CreateCastle();
        CreatePlayer();
    }

    private void CreateCastle()
    {
        var castle = new GameObject();
        castle.Scale = new Vector2(0.1f, 0.1f);
        castle.Position = GameConstants.CENTER;
        castle.Tag = "Castle";

        var renderer = castle.AddComponent<SpriteRenderer>();
        renderer.Texture = ResourceManager.Instance.GetTexture("castle");

        if (renderer.Texture != null)
        {
            renderer.Origin = new Vector2(renderer.Texture.Width / 2f, renderer.Texture.Height / 2f);
        }

        AddGameObject(castle);
    }

    private void CreatePlayer()
    {
        _player = new Player();
        _player.SetScene(this);
        AddGameObject(_player);
    }

    // Player spawn bullet
    public void SpawnProjectile(Vector2 position, Vector2 direction)
    {
        var projectile = new Projectile(position, direction);
        _objectsToAdd.Add(projectile);                                                                                                                                  

    }

    // Override Update เพื่อ cleanup inactive objects                                                                                                                                                   

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        foreach (var obj in _objectsToAdd)
        {
            AddGameObject(obj);
        }
        _objectsToAdd.Clear();

        _objectsToRemove.Clear();
        foreach (var obj in GameObjects)
        {
            if (!obj.Active)
            {
                _objectsToRemove.Add(obj);
            }
        }

        foreach (var obj in _objectsToRemove)
        {
            RemoveGameObject(obj);
        }
    }

    // debug rings
    public void DrawDebugRings(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        DrawCircle(spriteBatch, GameConstants.CENTER, GameConstants.ORANGE_RADIUS, Color.Orange * 0.5f, 2);
        DrawCircle(spriteBatch, GameConstants.CENTER, GameConstants.BLUE_RADIUS, Color.Blue * 0.5f, 2);
        DrawCircle(spriteBatch, GameConstants.CENTER, GameConstants.GREEN_RADIUS, Color.Green * 0.5f, 2);
    }

    private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, int thickness)
    {
        int segments = 64;
        float angleStep = MathHelper.TwoPi / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep;
            float angle2 = (i + 1) * angleStep;

            Vector2 p1 = center + new Vector2(MathF.Cos(angle1), MathF.Sin(angle1)) * radius;
            Vector2 p2 = center + new Vector2(MathF.Cos(angle2), MathF.Sin(angle2)) * radius;

            DrawLine(spriteBatch, p1, p2, color, thickness);
        }
    }

    private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
    {
        Vector2 edge = end - start;
        float angle = MathF.Atan2(edge.Y, edge.X);
        float length = edge.Length();

        spriteBatch.Draw(_pixelTexture, start, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
    }
}