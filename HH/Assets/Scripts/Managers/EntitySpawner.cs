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

    [Header("Make sure the weight adds up to 100")]
    [Tooltip("Make sure the weight values are sorted from low to high")]
    public EntityInformation[] entities;
}


public class EntitySpawner : MonoBehaviour
{
    [Header("These are the variables connected to different timestamps")]
    public TimeDependendSpawnVariables[] timeStampVariables;


    public TimeDependendSpawnVariables currentTimeVariables;

    public Transform[] spawnTransforms;

    public void Start()
    {

        Invoke(nameof(SpawnRandomEntity), 2f);
    }
    public void SpawnRandomEntity()
    {
        if (TryGetRandomEnemy(currentTimeVariables, out EntityInformation stats))
        {
            Ray ray = new Ray(GetRandomSpawnPoint(), Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Instantiate(stats.entity, hit.point, Quaternion.identity);
                StartCoroutine(SpawnCooldown(currentTimeVariables));
            }
        }
        else
        {
            print("Couldnt spawn an enemy");
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
