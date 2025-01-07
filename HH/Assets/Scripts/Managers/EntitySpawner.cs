using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EntityInformation
{
    public string entityName;

    [Range(0, 100)] 
    public int weight;
    public GameObject entity;
}
[System.Serializable]
public struct TimeDependendSpawnVariables 
{
    public int startHour;
    [Header("Spawning Variables")]
    public float minTimeTillNextSpawn;
    public float maxTimeTillNextSpawn;

    [Tooltip("This will multiply the time calculated from the min and max time till next spawn")]
    public float spawnTimeMultiplier;

    public float numberOfSpawns;

    [Header("Make sure the weight adds up to 100")]
    [Tooltip("Make sure the weight values are sorted from low to high")]
    public EntityInformation[] entities;
}


public class EntitySpawner : MonoBehaviour
{
    public TimeDependendSpawnVariables currentTimeVariables;

    public Transform[] spawnTransforms;

    public LayerMask groundMask;

    [SerializeField] int maxTries = 50;

    [SerializeField] int maxEntities;
    public static int currentEntityCount;
    public void Start()
    {
        SpawnRandomEntity();
    }
    public void SpawnRandomEntity()
    {
        for(int i = 0; i < currentTimeVariables.numberOfSpawns; i++)
        {
            if(currentEntityCount >= maxEntities) { return; }
            if (TryGetRandomEnemy(currentTimeVariables, out EntityInformation stats))
            {
                int currentTry = 0;

                while (currentTry < maxTries)
                {
                    // Get a random spawn point and raise it above ground
                    Vector3 spawnPoint = GetRandomSpawnPoint();
                    spawnPoint.y += 10f; // Ensure it's above the ground

                    // Cast the ray downward
                    Ray ray = new Ray(spawnPoint, Vector3.down);

                    // Raycast with a layer mask to detect the ground
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
                    {
                        currentEntityCount++;
                        // Instantiate entity at the hit point
                        Instantiate(stats.entity, hit.point, Quaternion.identity);
                        StartCoroutine(SpawnCooldown(currentTimeVariables));
                        break;  
                    }

                    currentTry++;
                    print($"Attempt {currentTry}: Could not find valid ground.");
                }

                print("Failed to spawn after maximum attempts.");
            }
            else
            {
                print("Could not spawn an enemy - no valid stats found.");
            }
        }
    }


    public Vector3 GetRandomSpawnPoint()
    {
        var randomTransform = spawnTransforms[Random.Range(0, spawnTransforms.Length)].position;
        randomTransform.y += 100;
        return randomTransform;
    }
    public IEnumerator SpawnCooldown(TimeDependendSpawnVariables time)
    {
        var cooldown = Random.Range(time.minTimeTillNextSpawn, time.maxTimeTillNextSpawn) * time.spawnTimeMultiplier;
        yield return new WaitForSeconds(cooldown);
        SpawnRandomEntity();
    }


    public bool TryGetRandomEnemy(TimeDependendSpawnVariables time, out EntityInformation stats)
    {
        int weight = Random.Range(0, 100);
        for (int i = 0; i < time.entities.Length; i++)
        {
            if (weight <= time.entities[i].weight)
            {
                stats = time.entities[i];
                return true;
            }
        }
        //this happens if no valid entity is found 
        stats = new EntityInformation();
        return false;
    }
}
