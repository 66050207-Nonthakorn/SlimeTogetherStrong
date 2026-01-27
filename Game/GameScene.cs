using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Components.Physics;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Game;

public class GameScene : Scene
{
    private Texture2D _pixelTexture;
    private Player _player;
    private List<GameObject> _objectsToRemove = new List<GameObject>();
    private List<GameObject> _objectsToAdd = new List<GameObject>();
    private List<GameObject> _targets = new List<GameObject>();  // สำหรับ test collision

    // test ally 
    private bool _isPlacingAlly = false;
    private MapManager _mapManager;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;



    public GameScene()
    {
        _mapManager = new MapManager();

        CreateCastle();
        CreatePlayer();
        CreateTestTargets();  // สร้าง dummy targets สำหรับทดสอบ ถ้ามี enermy แล้วจะเอามาแทนตรงนี้
    }

    private Ally CreateAlly(LaneData lane)
    {
        if (lane == null || !lane.CanAddAlly())
            return null;

        Ally ally = new Ally();
        ally.ParentLane = lane;
        ally.Tag = "Ally";

        var renderer = ally.AddComponent<SpriteRenderer>();
        renderer.Texture = ResourceManager.Instance.GetTexture("castle");
        if (renderer.Texture != null)
        {
            renderer.Origin =
                new Vector2(renderer.Texture.Width / 2f, renderer.Texture.Height / 2f);
        }

        ally.Scale = new Vector2(0.04f, 0.04f);

        var collider = ally.AddComponent<CircleCollider>();
        collider.Radius = 20f;

        lane.AddAlly(ally);
        AddGameObject(ally);

        return ally;
    }

    private LaneData GetLaneFromMouse(Vector2 mousePos)
    {
        LaneData closestLane = null;
        float minDistance = 35f; // ความกว้างถนน / tolerance

        foreach (LaneData lane in _mapManager.Lanes)
        {
            Vector2 laneVec = lane.StartPoint - lane.EndPoint;
            float laneLength = laneVec.Length();
            Vector2 laneDir = lane.Direction; // ต้องชี้จาก End → Start

            Vector2 toMouse = mousePos - lane.EndPoint;
            float proj = Vector2.Dot(toMouse, laneDir);

            if (proj < 0)
                continue;

            Vector2 closestPoint =
                lane.EndPoint + laneDir * proj;

            float distance =
                Vector2.Distance(mousePos, closestPoint);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestLane = lane;
            }
        }

        return closestLane;
    }

    private void CreateCastle()
    {
        Castle castle = new Castle(GameConstants.CENTER, 100);
        castle.Scale = new Vector2(0.1f, 0.1f);
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

    private void CreateTestTargets()
    {
        // สร้าง 3 targets บน GREEN_RADIUS สำหรับทดสอบ
        float[] angles = { 0f, MathHelper.PiOver2, MathHelper.Pi };  // 0°, 90°, 180°

        foreach (float angle in angles)
        {
            var target = new GameObject();
            target.Tag = "Target";

            // Position บน GREEN_RADIUS
            float x = GameConstants.CENTER.X + MathF.Cos(angle) * GameConstants.GREEN_RADIUS;
            float y = GameConstants.CENTER.Y + MathF.Sin(angle) * GameConstants.GREEN_RADIUS;
            target.Position = new Vector2(x, y);
            target.Scale = new Vector2(0.08f, 0.08f);

            // ใช้ castle texture เป็น placeholder
            var renderer = target.AddComponent<SpriteRenderer>();
            renderer.Texture = ResourceManager.Instance.GetTexture("castle");
            renderer.Tint = Color.Red;  // สีแดงให้เห็นชัด
            if (renderer.Texture != null)
            {
                renderer.Origin = new Vector2(renderer.Texture.Width / 2f, renderer.Texture.Height / 2f);
            }

            // เพิ่ม CircleCollider
            var collider = target.AddComponent<CircleCollider>();
            collider.Radius = 25f;  // รัศมี collision ของ target

            // เพิ่ม HealthComponent
            var health = target.AddComponent<HealthComponent>();
            health.MaxHP = 30;  // HP 30 = ยิง 3 ครั้งตาย (Projectile damage = 10)
            health.Initialize();

            // Event เมื่อโดนยิง
            health.OnDamage += (dmg) =>
            {
                System.Diagnostics.Debug.WriteLine($"HP: {health.CurrentHP}/{health.MaxHP}");
            };

            // Event เมื่อตาย
            health.OnDeath += () =>
            {
                System.Diagnostics.Debug.WriteLine(" DESTROYED!");
                target.Active = false;
            };

            AddGameObject(target);
            _targets.Add(target);
        }
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
        // _mapManager.Update(gameTime);

        base.Update(gameTime);

        // เพิ่ม objects ใหม่
        foreach (var obj in _objectsToAdd)
        {
            AddGameObject(obj);
        }
        _objectsToAdd.Clear();

        //  Projectile vs Target
        CheckProjectileCollisions();

        // Place Ally
        CheckPlaceAlly();

        // ลบ objects ที่ไม่ active
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
            _targets.Remove(obj);  // ลบออกจาก targets list ด้วย
        }
    }

    private void CheckPlaceAlly()
    {

        var keyboard = Keyboard.GetState();
        var mouse = Mouse.GetState();

        // กด Q ครั้งเดียว → เข้าโหมดวาง Ally
        if (keyboard.IsKeyDown(Keys.Q) && _prevKeyboard.IsKeyUp(Keys.Q))
        {
            _isPlacingAlly = true;
        }

        // คลิกซ้ายครั้งเดียว → วาง Ally
        if (_isPlacingAlly &&
            mouse.LeftButton == ButtonState.Pressed &&
            _prevMouse.LeftButton == ButtonState.Released)
        {
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
            LaneData lane = GetLaneFromMouse(mousePos);

            if (lane != null)
            {
                CreateAlly(lane);
                _isPlacingAlly = false;
            }
        }

        if (_isPlacingAlly)
        {
            var lane = GetLaneFromMouse(
                new Vector2(Mouse.GetState().X, Mouse.GetState().Y)
            );

            if (lane != null)
                System.Diagnostics.Debug.WriteLine($"HOVER LANE {lane.Index}");
        }

        // เก็บ state ก่อนหน้า
        _prevKeyboard = keyboard;
        _prevMouse = mouse;
    }

    private void CheckProjectileCollisions()
    {
        foreach (var obj in GameObjects)
        {
            if (obj.Tag != "Projectile" || !obj.Active) continue;

            var projectile = obj as Projectile;
            if (projectile == null) continue;

            var projectileCollider = projectile.GetComponent<CircleCollider>();
            if (projectileCollider == null) continue;

            foreach (var target in _targets)
            {
                if (!target.Active) continue;

                var targetCollider = target.GetComponent<CircleCollider>();
                if (targetCollider == null) continue;

                // ใช้ CircleCollider.IsIntersect แทน manual distance check
                if (projectileCollider.IsIntersect(targetCollider))
                {
                    // ชนแล้ว! Deal damage
                    var health = target.GetComponent<HealthComponent>();
                    health?.TakeDamage(projectile.Damage);

                    // สร้าง explosion effect ที่ตำแหน่งที่ชน
                    var explosion = new Explosion(projectile.Position);
                    _objectsToAdd.Add(explosion);

                    // ทำลาย projectile
                    projectile.Active = false;
                    break;  // projectile ชนได้แค่ 1 target
                }
            }
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