using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Components.Physics;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;

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
    private WaveManager _waveManager;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    public static Castle Castle;

    // XP System
    private XPManager _xpManager;
    
    // UI Management
    private GameSceneUI _gameUI;

    // Static reference to get player (for skill system)
    private static Player _staticPlayerRef;
    public static Player GetPlayer() => _staticPlayerRef;
    
    // Game over flag
    private bool _gameOver = false;



    public GameScene()
    {
        // Initialize XP Manager for this game session
        _xpManager = new XPManager();
        
        // Subscribe to level up event
        _xpManager.OnLevelUp += OnLevelUp;
        
        _mapManager = new MapManager();
        _waveManager = new WaveManager(); // _mapManager, this
        _waveManager.SetScene(this);

        CreateCastle();
        CreatePlayer();
        
        _waveManager.StartWave();  // Start the first wave
        
        // Initialize UI
        _gameUI = new GameSceneUI(this, _xpManager, _player);
        _gameUI.Initialize();
    }

    private void CreateAlly(LaneData lane)
    {
        if (lane != null && lane.CanAddAlly())
        {
            Ally ally = new Ally();
            ally.SetScene(this);
            lane.AddAlly(ally);
            _objectsToAdd.Add(ally);
        }    
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
        Castle = new Castle();
        AddGameObject(Castle);
    }

    private void CreatePlayer()
    {
        _player = new Player();
        _player.SetScene(this);
        _staticPlayerRef = _player; // Set static reference
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
        // Don't update if game is over
        if (_gameOver)
            return;
            
        // Check for ESC key to open pause menu
        if (InputManager.Instance.IsKeyPressed(Keys.Escape))
        {
            SceneManager.Instance.PushOverlay(new PauseScene());
            return; // Don't process rest of update when opening pause menu
        }

        // Debug: Press X to add XP
        if (InputManager.Instance.IsKeyPressed(Keys.X))
        {
            _xpManager.AddXP(10);
        }
        
        // Debug: Press F1 to trigger win screen
        if (InputManager.Instance.IsKeyPressed(Keys.F1))
        {
            ShowGameOver(true);
            return;
        }
        
        // Debug: Press F2 to trigger lose screen
        if (InputManager.Instance.IsKeyPressed(Keys.F2))
        {
            ShowGameOver(false);
            return;
        }

        // Debug: Press L to test level up overlay
        if (InputManager.Instance.IsKeyPressed(Keys.L))
        {
            ShowLevelUpOverlay();
        }
        
        // Check lose condition: Castle HP <= 0
        if (Castle != null)
        {
            var castleHealth = Castle.GetComponent<HealthComponent>();
            if (castleHealth != null && castleHealth.CurrentHP <= 0)
            {
                ShowGameOver(false);
                return;
            }
        }

        // Update skill manager
        SkillManager.Instance.Update(gameTime);

        // Update wave manager
        _waveManager.Update(gameTime);
        
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
        // if (keyboard.IsKeyDown(Keys.Q) && _prevKeyboard.IsKeyUp(Keys.Q))
        // {
        //     _isPlacingAlly = true;
        // }

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

            // if (lane != null)
            //     System.Diagnostics.Debug.WriteLine($"HOVER LANE {lane.Index}");
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

            // Check collision against all enemies, not just _targets
            foreach (var target in GameObjects)
            {
                if (target.Tag != "Enemy" || !target.Active) continue;

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

    private void ShowLevelUpOverlay()
    {
        // Create sample upgrade options
        var upgradeOptions = new List<UpgradeOption>
        {
            new UpgradeOption
            {
                EntityTexture = ResourceManager.Instance.GetTexture("P_idle_0"),
                StatsIconTexture = ResourceManager.Instance.GetTexture("Health_Icon"),
                EntityName = "Player",
                StatsName = "Health",
                StatsIncrease = 50
            },
            new UpgradeOption
            {
                EntityTexture = ResourceManager.Instance.GetTexture("castle"),
                StatsIconTexture = ResourceManager.Instance.GetTexture("Health_Icon"),
                EntityName = "Castle",
                StatsName = "Defense",
                StatsIncrease = 25
            },
            new UpgradeOption
            {
                EntityTexture = ResourceManager.Instance.GetTexture("P_idle_0"),
                StatsIconTexture = ResourceManager.Instance.GetTexture("Mana_Icon"),
                EntityName = "Player",
                StatsName = "Mana",
                StatsIncrease = 30
            }
        };

        // Show overlay with callback
        var levelUpScene = new LevelUpOverlayScene(upgradeOptions, OnUpgradeSelected);
        SceneManager.Instance.PushOverlay(levelUpScene);
    }

    private void OnLevelUp(int newLevel)
    {
        System.Console.WriteLine($"Level up! Now level {newLevel}");
        ShowLevelUpOverlay();
    }

    private void OnUpgradeSelected(int cardIndex)
    {
        System.Console.WriteLine($"Selected upgrade card {cardIndex}");
        // Handle the upgrade logic here based on cardIndex
        // For example:
        // if (cardIndex == 0) { /* Upgrade player health */ }
        // if (cardIndex == 1) { /* Upgrade castle defense */ }
        // if (cardIndex == 2) { /* Upgrade player mana */ }
    }
    
    private void ShowGameOver(bool isWin)
    {
        if (_gameOver)
            return;
            
        _gameOver = true;
        
        // Get play time from timer UI
        string playTime = "00:00";
        if (_gameUI != null && _gameUI.TimerUI != null)
        {
            playTime = _gameUI.TimerUI.GetFormattedTime();
        }
        
        // Show game over overlay (similar to level up overlay)
        var gameOverScene = new GameOverScene(isWin, playTime);
        SceneManager.Instance.PushOverlay(gameOverScene);
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