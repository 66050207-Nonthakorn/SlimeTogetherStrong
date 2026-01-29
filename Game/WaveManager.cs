using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SlimeTogetherStrong.Game;

public class WaveManager
{
    private Enemy[] EnemyTypes = new Enemy[]
    {
        new Ninja(),
        new Spearsman(),
        new Warrior(),
        new Tank()
    };

    public int CurrentWave = 1;
    public int TotalWaves = 10;
    public float SpawnTimer;
    public float SpawnInterval = 2f;
    private int enemiesSpawned;
    private int enemiesPerWave;
    private bool waveActive = false;
    private List<Enemy> currentWaveEnemies = new List<Enemy>();
    
    // Reference to scene
    private GameScene _scene;
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

    public void StartWave()
    {
        if (CurrentWave > TotalWaves)
            return;

        waveActive = true;
        enemiesSpawned = 0;
        SpawnTimer = 0f;
        currentWaveEnemies.Clear();
        
        // Increase enemies per wave as waves progress
        enemiesPerWave = 3 + CurrentWave;
        
        System.Diagnostics.Debug.WriteLine($"Wave {CurrentWave} started! Enemies to spawn: {enemiesPerWave}");
    }

    public void SpawnEnemy()
    {
        if (!waveActive || enemiesSpawned >= enemiesPerWave)
            return;

        // Pick a random lane to spawn in
        int laneIndex = _random.Next(0, MapManager.Instance.Lanes.Length);
        LaneData lane = MapManager.Instance.Lanes[laneIndex];

        int index = lane.Enemies.Count;

        Enemy enemyClass = EnemyTypes[_random.Next(0, EnemyTypes.Length)];

        Enemy enemy = enemyClass.GetType()
            .GetConstructor(Array.Empty<Type>())
            .Invoke(null) as Enemy;
        enemy.SetScene(_scene);
        enemy.SetParentLane(lane, index);
        lane.AddEnemy(enemy);
        
        // Position enemy at the spawn point of the lane
        enemy.Position =
            lane.StartPoint
            + lane.Direction * index * 40f; // Spacing out enemies
        
        _scene.AddGameObject(enemy);
        currentWaveEnemies.Add(enemy);
        enemiesSpawned++;
        
        System.Diagnostics.Debug.WriteLine($"Enemy spawned in lane {laneIndex}. Total: {enemiesSpawned}/{enemiesPerWave}");
    }

    public bool IsWaveComplete()
    {
        if (!waveActive)
            return false;

        // Wave is complete when all enemies have been spawned AND all are defeated
        if (enemiesSpawned < enemiesPerWave)
            return false;

        // Check if all spawned enemies are defeated
        foreach (var enemy in currentWaveEnemies)
        {
            if (enemy.Active)
                return false;  // At least one enemy is still alive
        }

        return true;
    }

    public bool AllWavesComplete()
    {
        return CurrentWave > TotalWaves;
    }

    public void Update(GameTime gameTime)
    {
        if (!waveActive && CurrentWave <= TotalWaves)
        {
            // Start the next wave if not already active
            StartWave();
            return;
        }

        if (!waveActive)
            return;

        SpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (SpawnTimer >= SpawnInterval && enemiesSpawned < enemiesPerWave)
        {
            SpawnEnemy();
            SpawnTimer = 0f;
        }

        // Check if wave is complete and advance
        if (IsWaveComplete())
        {
            waveActive = false;
            CurrentWave++;
            System.Diagnostics.Debug.WriteLine($"Wave {CurrentWave - 1} complete!");
        }
    }
}