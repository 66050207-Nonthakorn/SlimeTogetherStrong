using Microsoft.Xna.Framework;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Game.Entities;
using SlimeTogetherStrong.Game.Entities.Enemies;
using SlimeTogetherStrong.Game.Scenes;
using System;
using System.Collections.Generic;

namespace SlimeTogetherStrong.Game.Systems;

public class WaveManager
{
    private Enemy[] EnemyTypes = new Enemy[]
    {
        new Ninja(),
        new Spearsman(),
        new Warrior(),
        new Tank()
    };

    private int[][] SpawnWeights = new int[][]
    {
        new int[] { 40, 15, 40, 5 },
        new int[] { 30, 25, 30, 15 },
        new int[] { 20, 30, 25, 25 }
    };

    public int CurrentWave = 1;
    public int TotalWaves = 10;
    public float SpawnTimer;
    public float SpawnInterval = 2f;
    private int enemiesSpawned;
    private int enemiesPerWave;
    private bool waveActive = false;
    private List<Enemy> currentWaveEnemies = new List<Enemy>();

    private bool isWavePaused = false;
    private float wavePauseTimer = 0f;
    private const float WAVE_PAUSE_DURATION = 2.5f;

    private GameScene _scene;
    private XPManager _xpManager;
    private Random _random = new Random();
    public static WaveManager Instance { get; private set; }

    public WaveManager()
    {
        Instance = this;
    }

    public void SetScene(GameScene scene)
    {
        _scene = scene;
    }

    public void SetXPManager(XPManager xpManager)
    {
        _xpManager = xpManager;
    }

    private float GetSpawnInterval()
    {
        if (CurrentWave <= 3) return 2.0f;
        if (CurrentWave <= 6) return 1.5f;
        if (CurrentWave <= 9) return 1.2f;
        return 1.0f;
    }

    private int[] GetCurrentSpawnWeights()
    {
        if (CurrentWave <= 3) return SpawnWeights[0];
        if (CurrentWave <= 6) return SpawnWeights[1];
        return SpawnWeights[2];
    }

    private Enemy PickWeightedEnemy()
    {
        int[] weights = GetCurrentSpawnWeights();
        int totalWeight = 0;
        foreach (int w in weights) totalWeight += w;

        int roll = _random.Next(0, totalWeight);
        int cumulative = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll < cumulative)
            {
                return EnemyTypes[i];
            }
        }

        return EnemyTypes[0];
    }

    public void StartWave()
    {
        if (CurrentWave > TotalWaves)
            return;

        waveActive = true;
        enemiesSpawned = 0;
        SpawnTimer = 0f;
        SpawnInterval = GetSpawnInterval();
        currentWaveEnemies.Clear();

        enemiesPerWave = 3 + CurrentWave;
    }

    public void SpawnEnemy()
    {
        if (!waveActive || enemiesSpawned >= enemiesPerWave)
            return;

        int laneIndex = _random.Next(0, MapManager.Instance.Lanes.Length);
        LaneData lane = MapManager.Instance.Lanes[laneIndex];

        int index = lane.Enemies.Count;

        Enemy enemyClass = PickWeightedEnemy();

        Enemy enemy = enemyClass.GetType()
            .GetConstructor(Array.Empty<Type>())
            .Invoke(null) as Enemy;
        enemy.SetScene(_scene);
        enemy.SetParentLane(lane, index);
        enemy.SetXPManager(_xpManager);
        lane.AddEnemy(enemy);

        enemy.Position =
            lane.StartPoint
            + lane.Direction * index * 40f;

        _scene.AddGameObject(enemy);
        currentWaveEnemies.Add(enemy);
        enemiesSpawned++;
    }

    public bool IsWaveComplete()
    {
        if (!waveActive)
            return false;

        if (enemiesSpawned < enemiesPerWave)
            return false;

        foreach (var enemy in currentWaveEnemies)
        {
            if (enemy.Active)
                return false;
        }

        return true;
    }

    public bool AllWavesComplete()
    {
        return CurrentWave > TotalWaves;
    }

    public bool IsWavePaused => isWavePaused;

    public float GetRemainingPauseTime() => Math.Max(0f, WAVE_PAUSE_DURATION - wavePauseTimer);

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (isWavePaused)
        {
            wavePauseTimer += deltaTime;
            if (wavePauseTimer >= WAVE_PAUSE_DURATION)
            {
                isWavePaused = false;
                wavePauseTimer = 0f;
                StartWave();
            }
            return;
        }

        if (!waveActive && CurrentWave <= TotalWaves)
        {
            if (CurrentWave == 1)
            {
                StartWave();
            }
            return;
        }

        if (!waveActive)
            return;

        SpawnTimer += deltaTime;

        if (SpawnTimer >= SpawnInterval && enemiesSpawned < enemiesPerWave)
        {
            SpawnEnemy();
            SpawnTimer = 0f;
        }

        if (IsWaveComplete())
        {
            waveActive = false;
            CurrentWave++;

            if (CurrentWave <= TotalWaves)
            {
                isWavePaused = true;
                wavePauseTimer = 0f;
            }
        }
    }
}
