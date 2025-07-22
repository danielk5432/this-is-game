using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    // References to the individual, self-contained spawners.
    [Header("Spawner References")]
    public GhostEnemySpawnerNew ghostSpawner;
    public BurstEnemySpawnerNew burstSpawner;
    public BombRainController bombRainController;
    public LaserController laserController;
    
    public enum SpawnType { Ghost, Burst, BombRain, Laser }
    
    /// <summary>
    /// Starts a specific spawner.
    /// </summary>
    public void StartSpawning(SpawnType type)
    {
        switch (type)
        {
            case SpawnType.Ghost:
                if (ghostSpawner != null) ghostSpawner.BeginSpawning();
                break;
            case SpawnType.Burst:
                if (burstSpawner != null) burstSpawner.BeginSpawning();
                break;
            // Add other enemy types here...
        }
    }

    /// <summary>
    /// Stops a specific spawner.
    /// </summary>
    public void StopSpawning(SpawnType type)
    {
        switch (type)
        {
            case SpawnType.Ghost:
                if (ghostSpawner != null) ghostSpawner.StopSpawning();
                break;
            case SpawnType.Burst:
                if (burstSpawner != null) burstSpawner.StopSpawning();
                break;
            // Add other enemy types here...
        }
    }

    /// <summary>
    /// Stops ALL spawners and clears all spawned enemies.
    /// </summary>
    public void StopAndClearAll()
    {
        // Tell all spawners to stop their individual routines.
        if (ghostSpawner != null) ghostSpawner.StopSpawning();
        if (burstSpawner != null) burstSpawner.StopSpawning();
        // ... stop other spawners ...

        // Destroy all objects tagged as "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}