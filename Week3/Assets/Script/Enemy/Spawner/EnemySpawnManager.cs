using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    // References to the individual, self-contained spawners.
    [Header("Spawner References")]
    public GhostEnemySpawnerNew ghostSpawner;
    public BurstEnemySpawnerNew burstSpawner;
    public BombRainSpawner bombRainController;
    public LaserSpawner laserController;
    
    public enum SpawnType { Ghost, Burst, BombRain, Laser }
    
    /// <summary>
    /// Starts a specific spawner.
    /// </summary>
    public void StartSpawning(SpawnType type)
    {
        Debug.Log($"Starting spawner for type: {type}");
        switch (type)
        {
            case SpawnType.Ghost:
                if (ghostSpawner != null) ghostSpawner.BeginSpawning();
                break;
            case SpawnType.Burst:
                if (burstSpawner != null) burstSpawner.BeginSpawning();
                break;
            case SpawnType.BombRain:
                if (bombRainController != null) bombRainController.BeginSpawning();
                break;
            case SpawnType.Laser:
                if (laserController != null) laserController.BeginSpawning();
                break;
        }
    }

    /// <summary>
    /// Stops a specific spawner.
    /// </summary>
    public void StopSpawning(SpawnType type)
    {
        Debug.Log($"Stopping spawner for type: {type}");
        switch (type)
        {
            case SpawnType.Ghost:
                if (ghostSpawner != null) ghostSpawner.StopSpawning();
                break;
            case SpawnType.Burst:
                if (burstSpawner != null) burstSpawner.StopSpawning();
                break;
            case SpawnType.BombRain:
                if (bombRainController != null) bombRainController.StopSpawning();
                break;
            case SpawnType.Laser:
                if (laserController != null) laserController.StopSpawning();
                break;
        }
    }

    /// <summary>
    /// Stops ALL spawners and clears all spawned enemies.
    /// </summary>
    public void StopAndClearAll()
    {
        Debug.Log("Stopping all enemy spawners and clearing all enemies.");
        // Tell all spawners to stop their individual routines.
        if (ghostSpawner != null) ghostSpawner.StopSpawning();
        if (burstSpawner != null) burstSpawner.StopSpawning();
        if (bombRainController != null) bombRainController.StopSpawning();
        if (laserController != null) laserController.StopSpawning();

        // Destroy all objects tagged as "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}