using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;
using static StaticObjectHolder;

[System.Serializable]
public class EnemyInfo
{
    [Tooltip("The enemy GameObject to spawn")]
    public GameObject enemy;
    [Tooltip("The value of the enemy. An enemy with a value of 5 is worth 5 enemies with an enemy value of 1")]
    public int enemyValue;
    [Tooltip("The spawn density of enemies over time. This is compared to the spawn density of other enemies.")]
    public AnimationCurve spawnWeightAgainstEnemySpawnVolume;
}

public class Spawner : MonoBehaviour
{
    [Header("-- Core Spawning --")]
    [SerializeField, Tooltip("The core gameObject that will be spawned in.")]
    private GameObject Core;
    [SerializeField, Tooltip("The maximum distance from the center of the map that the core can spawn")]
    private float coreSpawnRadius = 650f;

    [Header("-- Ore Spawning --")]
    [SerializeField, Tooltip("The Ore GameObject to spawn")]
    private GameObject ore;
    [SerializeField, Tooltip("How far below the surface is the ore spawned")]
    private float oreDepthBelowSurface = 9f;
    [SerializeField, Tooltip("How far apart are the cells for spawning ores")]
    private float oreSpawnGridSpacing = 75f;
    [SerializeField, Tooltip("The maximum distance from the center of the map to spawn the ore")]
    private float oreSpawnRadius = 700f;

    [Header("-- Enemy Spawning --")]
    [SerializeField, Tooltip("The list of enemies to spawn.")]
    private List<EnemyInfo> enemies;
    [SerializeField, Tooltip("How far away from the center of the map to spawn the waves of enemies")]
    private float enemySpawnRadius = 950f;
    [SerializeField, Tooltip("The time between waves")]
    private float enemySpawnPeriod;
    [SerializeField, Tooltip("How many enemies spawn to start")]
    private float startSpawnRate = 5f;
    [SerializeField, Tooltip("how many enemies to add to the startSpawnRate in the start")]
    private float baseSpawnRate;
    [SerializeField, Tooltip("relates to how quickly enemy difficulty and amount increases with each wave")]
    private float difficultyCurveExponent = 0.075f;

    //the enemies spawned per wave
    private int NumberOfEnemiesToSpawn
    {
        get
        {
            float exponent = difficultyCurveExponent * Time.time / 60f;
            return (int)(startSpawnRate + baseSpawnRate * Mathf.Pow(2, exponent));
        }
        set { }
    }

    [SerializeField, Tooltip("purely used for visualizing radii with gizmos turned on")]
    private float lookRadius = 875f;
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    private void Awake()
    {
        //spawn all the things that need to be spawned to start and start the waveSpawner() coroutine
        SpawnCore();
        SpawnOre();
        StartCoroutine(WaveSpawner());
    }

    /// <summary>
    /// returns a list of enemies to spawn given the current time, and the information in the enemy infos in enemies
    /// </summary>
    private List<GameObject> EnemiesToSpawn()
    {
        List<GameObject> returnList = new List<GameObject>();
        int currentNumberOfEnemiesToSpawn = NumberOfEnemiesToSpawn;
        List<float> enemyWeights = new List<float>();
        //add the weight of each enemy into the enemyWeights list
        foreach (EnemyInfo enemyInfo in enemies)
        {
            enemyWeights.Add(enemyInfo.spawnWeightAgainstEnemySpawnVolume.Evaluate(currentNumberOfEnemiesToSpawn));
        }
        float totalEnemyWeight = 0;
        //add all the weights in enemyWeights to get the total enemyWeight
        foreach (float enemyWeight in enemyWeights)
        {
            totalEnemyWeight += enemyWeight;
        }
        //the value of each enemy is multiplied by the current number of enemies to spawn divided by the total enemy weight
        //this ensures that the weights of all the enemies add up to the total enemy Weight
        float enemyValueMultiplier = currentNumberOfEnemiesToSpawn / totalEnemyWeight;
        //calculate the number of each enemy to spawn, then add them that many times into the return list.
        List<int> numberOfEachEnemyToSpawn = new List<int>();
        for (int i = 0; i < enemies.Count; i++)
        {
            numberOfEachEnemyToSpawn.Add(Mathf.RoundToInt(Mathf.CeilToInt(enemyWeights[i] * enemyValueMultiplier) / enemies[i].enemyValue));
        }
        for (int i = 0; i < numberOfEachEnemyToSpawn.Count; i++)
        {
            for (int j = 0; j < numberOfEachEnemyToSpawn[i]; j++)
            {
                returnList.Add(enemies[i].enemy);
            }
        }
        return returnList;
    }

    /// <summary>
    /// spawn waves periodically every enemyspawnPeriodSeconds
    /// </summary>
    private IEnumerator WaveSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemySpawnPeriod);
            StartCoroutine(SpawnWave());
        }
    }

    /// <summary>
    /// spawns the appropriate amount of enemies spread out around an area on the border of the crater
    /// </summary>
    private IEnumerator SpawnWave()
    {
        //a list of enemies to spawn
        List<GameObject> enemiesToSpawn = EnemiesToSpawn();
        //choose a spot to spawn them around
        float enemySpawnAngle = Random.Range(0, 360);
        //spawn them all around the chosen spot, with .1 of a second between each one being spawned.
        foreach (GameObject enemy in enemiesToSpawn)
        {
            Vector2 SpawnPosition = enemySpawnRadius * Angle2Direction(enemySpawnAngle + Random.Range(-2.5f, 2.5f));
            float spawnHeight = TargetHeight(SpawnPosition);
            Instantiate(enemy, ConvertToVector3(SpawnPosition) + Vector3.up * spawnHeight, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    /// <summary>
    /// spawns the core at a random spot within coreSpawnRadius from the center of the map and gives it a random y rotation
    /// </summary>
    private void SpawnCore()
    {
        Vector2 spawnPos = Random.insideUnitCircle * coreSpawnRadius;
        float spawnHeight = TargetHeight(spawnPos);
        Vector3 coreSpawnPosition = ConvertToVector3(spawnPos) + Vector3.up * spawnHeight;
        theCore = Instantiate(Core, coreSpawnPosition, Quaternion.identity).GetComponent<Structures.Core>();
        theCore.transform.Rotate(Vector3.up * Random.Range(0, 360));
    }

    /// <summary>
    /// distributes the ore across the map such that there will be on average 1 ore every oreGridSpacing units within the oreSpawnRadius
    /// </summary>
    private void SpawnOre()
    {
        int oreSpawnGridSideCount = Mathf.FloorToInt(oreSpawnRadius * 2 / oreSpawnGridSpacing);
        float maxRandomOffset = oreSpawnGridSpacing * 0.5f;
        for (int x = 0; x < oreSpawnGridSideCount; x++)
        {
            for (int z = 0; z < oreSpawnGridSideCount; z++)
            {
                float xPos = (x * oreSpawnGridSpacing) - (0.5f * oreSpawnGridSideCount * oreSpawnGridSpacing);
                float zPos = (z * oreSpawnGridSpacing) - (0.5f * oreSpawnGridSideCount * oreSpawnGridSpacing);
                if (Vector2.Distance(new Vector2(xPos, zPos), Vector2.zero) <= oreSpawnRadius)
                {
                    float xPosRandomVariation = Random.Range(-maxRandomOffset, maxRandomOffset);
                    float zPosRandomVariation = Random.Range(-maxRandomOffset, maxRandomOffset);
                    xPos += xPosRandomVariation;
                    zPos += zPosRandomVariation;
                    if (Vector2.Distance(new Vector2(xPos, zPos), ConvertToVector2(theCore.transform.position)) > 25)
                    {
                        float yPos = TargetHeight(new Vector2(xPos, zPos)) - oreDepthBelowSurface;
                        Vector3 orePosition = new Vector3(xPos, yPos, zPos);
                        Quaternion oreRotation = Random.rotation;
                        Instantiate(ore, orePosition, oreRotation);
                    }
                }
            }
        }
    }
}