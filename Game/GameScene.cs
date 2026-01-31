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
    private List<GameObject> _targets = new List<GameObject>();

    private MapManager _mapManager;
    private WaveManager _waveManager;
    private MouseState _prevMouse;

    public static Castle Castle;

    private XPManager _xpManager;

    private UpgradeManager _upgradeManager;
    private List<UpgradeData> _currentUpgradeOptions;

    private GameSceneUI _gameUI;

    private static Player _staticPlayerRef;
    public static Player GetPlayer() => _staticPlayerRef;

    private bool _gameOver = false;

    public GameScene()
    {
        _xpManager = new XPManager();

        _upgradeManager = new UpgradeManager();

        _xpManager.OnLevelUp += OnLevelUp;

        _mapManager = new MapManager();
        _waveManager = new WaveManager();
        _waveManager.SetScene(this);
        _waveManager.SetXPManager(_xpManager);

        CreateCastle();
        CreatePlayer();

        _waveManager.StartWave();

        _gameUI = new GameSceneUI(this, _xpManager, _player);
        _gameUI.Initialize();

        SkillManager.Instance.OnAllyPlaceRequested += OnAllyPlaceRequested;
    }

    private void OnAllyPlaceRequested()
    {
    }

    private void CreateAlly(LaneData lane)
    {
        if (lane != null && lane.CanAddAlly())
        {
            Ally ally = new Ally();
            ally.SetScene(this);
            ally.ApplyBonusStats(
                UpgradeManager.Instance?.BonusAllyMaxHP ?? 0,
                UpgradeManager.Instance?.BonusAllyDamage ?? 0
            );
            lane.AddAlly(ally);
            _objectsToAdd.Add(ally);
        }
    }

    private LaneData GetLaneFromMouse(Vector2 mousePos)
    {
        LaneData closestLane = null;
        float minDistance = 80f;

        foreach (LaneData lane in _mapManager.Lanes)
        {
            Vector2 laneVec = lane.StartPoint - lane.EndPoint;
            float laneLength = laneVec.Length();
            Vector2 laneDir = lane.Direction;

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
        _staticPlayerRef = _player;
        AddGameObject(_player);
    }

    public void SpawnProjectile(Vector2 position, Vector2 direction)
    {
        var projectile = new Projectile(position, direction);
        projectile.Damage += UpgradeManager.Instance?.BonusPlayerDamage ?? 0;
        _objectsToAdd.Add(projectile);
    }

    public override void Update(GameTime gameTime)
    {
        if (_gameOver)
            return;
                    if (InputManager.Instance.IsKeyPressed(Keys.Escape))
        {
            SceneManager.Instance.PushOverlay(new PauseScene());
            return;
        }

        if (InputManager.Instance.IsKeyPressed(Keys.X))
        {
            _xpManager.AddXP(10);
        }

        if (InputManager.Instance.IsKeyPressed(Keys.F1))
        {
            ShowGameOver(true);
            return;
        }

        if (InputManager.Instance.IsKeyPressed(Keys.F2))
        {
            ShowGameOver(false);
            return;
        }

        if (InputManager.Instance.IsKeyPressed(Keys.L))
        {
            ShowLevelUpOverlay(_xpManager.CurrentLevel + 1);
        }

        if (Castle != null)
        {
            var castleHealth = Castle.GetComponent<HealthComponent>();
            if (castleHealth != null && castleHealth.CurrentHP <= 0)
            {
                ShowGameOver(false);
                return;
            }
        }

        if (_waveManager.AllWavesComplete())
        {
            ShowGameOver(true);
            return;
        }

        SkillManager.Instance.Update(gameTime);

        _waveManager.Update(gameTime);


        base.Update(gameTime);

        foreach (var obj in _objectsToAdd)
        {
            AddGameObject(obj);
        }
        _objectsToAdd.Clear();

        CheckProjectileCollisions();

        CheckPlaceAlly();

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
            _targets.Remove(obj);
        }
    }

    private void CheckPlaceAlly()
    {
        var mouse = Mouse.GetState();

        if (SkillManager.Instance.IsInPlacementMode &&
            mouse.LeftButton == ButtonState.Pressed &&
            _prevMouse.LeftButton == ButtonState.Released)
        {
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
            LaneData lane = GetLaneFromMouse(mousePos);

            if (lane != null && lane.CanAddAlly())
            {
                CreateAlly(lane);
                SkillManager.Instance.OnAllyPlacedSuccessfully();
            }
        }

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

            foreach (var target in GameObjects)
            {
                if (target.Tag != "Enemy" || !target.Active) continue;

                var targetCollider = target.GetComponent<CircleCollider>();
                if (targetCollider == null) continue;

                if (projectileCollider.IsIntersect(targetCollider))
                {
                    var health = target.GetComponent<HealthComponent>();
                    health?.TakeDamage(projectile.Damage);

                    var explosion = new Explosion(projectile.Position);
                    _objectsToAdd.Add(explosion);

                    projectile.Active = false;
                    break;
                }
            }
        }
    }

    private void ShowLevelUpOverlay(int newLevel)
    {
        _currentUpgradeOptions = _upgradeManager.GetRandomUpgrades(3);

        var upgradeOptions = new List<UpgradeOption>();
        foreach (var upgrade in _currentUpgradeOptions)
        {
            upgradeOptions.Add(new UpgradeOption
            {
                EntityTexture = ResourceManager.Instance.GetTexture(upgrade.EntityTextureKey),
                StatsIconTexture = ResourceManager.Instance.GetTexture(upgrade.StatsIconKey),
                EntityName = upgrade.EntityName,
                StatsName = upgrade.StatsName,
                StatsIncrease = upgrade.Value
            });
        }

        int clearedWave = Math.Max(1, _waveManager.CurrentWave - 1);

        var levelUpScene = new LevelUpOverlayScene(upgradeOptions, OnUpgradeSelected, clearedWave, newLevel);
        SceneManager.Instance.PushOverlay(levelUpScene);
    }

    private void OnLevelUp(int newLevel)
    {
        ShowLevelUpOverlay(newLevel);
    }

    private void OnUpgradeSelected(int cardIndex)
    {
        if (_currentUpgradeOptions == null || cardIndex >= _currentUpgradeOptions.Count)
            return;

        var selectedUpgrade = _currentUpgradeOptions[cardIndex];

        _upgradeManager.ApplyUpgrade(selectedUpgrade);

        ApplyUpgradeEffect(selectedUpgrade);
    }

    private void ApplyUpgradeEffect(UpgradeData upgrade)
    {
        switch (upgrade.Type)
        {
            case UpgradeType.CastleMaxHP:
                var castleHealth = Castle?.GetComponent<HealthComponent>();
                if (castleHealth != null)
                {
                    castleHealth.MaxHP += upgrade.Value;
                    castleHealth.Heal(upgrade.Value);
                }
                break;

            case UpgradeType.CastleHeal:
                var castleHeal = Castle?.GetComponent<HealthComponent>();
                castleHeal?.Heal(upgrade.Value);
                break;

            case UpgradeType.PlayerMaxMana:
                if (_player?.ManaComponent != null)
                {
                    _player.ManaComponent.MaxMana += upgrade.Value;
                    _player.ManaComponent.RegenerateMana(upgrade.Value);
                }
                break;

            case UpgradeType.PlayerManaRegen:
                if (_player?.ManaComponent != null)
                {
                    _player.ManaComponent.ManaRegenRate += upgrade.Value;
                }
                break;

            case UpgradeType.PlayerDamage:
                break;

            case UpgradeType.PlayerAttackSpeed:
                if (_player != null)
                {
                    _player.ShootCooldown *= (1f - upgrade.Value / 100f);
                }
                break;

            case UpgradeType.AllyMaxHP:
            case UpgradeType.AllyDamage:
                break;
        }
    }

    private void ShowGameOver(bool isWin)
    {
        if (_gameOver)
            return;

        _gameOver = true;

        string playTime = "00:00";
        if (_gameUI != null && _gameUI.TimerUI != null)
        {
            playTime = _gameUI.TimerUI.GetFormattedTime();
        }

        var gameOverScene = new GameOverScene(isWin, playTime);
        SceneManager.Instance.PushOverlay(gameOverScene);
    }

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

    public void DrawPlacementHighlights(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!SkillManager.Instance.IsInPlacementMode)
            return;

        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        foreach (var lane in _mapManager.Lanes)
        {
            if (!lane.CanAddAlly())
                continue;

            int nextSlotIndex = lane.Allies.Count;
            float forwardOffset = GameConstants.BLUE_RADIUS + nextSlotIndex * GameConstants.FORWARD_SPACING;
            float sideOffset = (nextSlotIndex % 2 == 0 ? -1 : 1) * GameConstants.SIDE_SPACING;

            Vector2 highlightPos = lane.EndPoint
                + lane.Direction * forwardOffset
                + lane.Perpendicular * sideOffset;

            DrawFilledCircle(spriteBatch, highlightPos, 25f, Color.LimeGreen * 0.6f);
            DrawCircle(spriteBatch, highlightPos, 25f, Color.Green, 2);
        }
    }

    private void DrawFilledCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color)
    {
        for (int y = -(int)radius; y <= (int)radius; y++)
        {
            float halfWidth = MathF.Sqrt(radius * radius - y * y);
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle((int)(center.X - halfWidth), (int)(center.Y + y), (int)(halfWidth * 2), 1),
                color
            );
        }
    }
}
