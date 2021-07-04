using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;
using static StaticObjectHolder;

[System.Serializable]
public class EnemyInfo
{
    public GameObject enemy;
    [Tooltip("The value of the enemy. An enemy with a value of 5 is worth 5 enemies with an enemy value of 1")]
    public int enemyValue;
    public AnimationCurve spawnWeightAgainstEnemySpawnVolume;
}

public class Spawner : MonoBehaviour
{
    [Header("-- Core Spawning --")]
    [SerializeField]
    private GameObject Core;
    [SerializeField]
    private float coreSpawnRadius = 650f;

    [Header("-- Ore Spawning --")]
    [SerializeField]
    private GameObject ore;
    [SerializeField]
    private float oreDepthBelowSurface = 9f;
    [SerializeField]
    private float oreSpawnGridSpacing = 75f;
    [SerializeField]
    private float oreSpawnRadius = 700f;

    [Header("-- Enemy Spawning --")]
    [SerializeField]
    private List<EnemyInfo> enemies;
    [SerializeField]
    private float enemySpawnRadius = 950f;
    [SerializeField, Tooltip("The time between waves")]
    private float enemySpawnPeriod;
    [SerializeField]
    private float baseSpawnRate;
    [SerializeField, Tooltip("relates to how quickly enemy difficulty and amount increases with each wave")]
    private float difficultyCurveExponent = 0.075f;

    //the enemies spawned per wave
    private int NumberOfEnemiesToSpawn
    {
        get
        {
            float exponent = difficultyCurveExponent * Time.time / 60f;
            return (int)(baseSpawnRate * Mathf.Pow(2, exponent));
        }
        set { }
    }

    [SerializeField]
    private float lookRadius = 875f;
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    private void Awake()
    {
        SpawnCore();
        SpawnOre();
        StartCoroutine(WaveSpawner());
    }

    private List<GameObject> EnemiesToSpawn()
    {
        List<GameObject> returnList = new List<GameObject>();
        int currentNumberOfEnemiesToSpawn = NumberOfEnemiesToSpawn;
        List<float> enemyWeights = new List<float>();
        foreach (EnemyInfo enemyInfo in enemies)
        {
            enemyWeights.Add(enemyInfo.spawnWeightAgainstEnemySpawnVolume.Evaluate(currentNumberOfEnemiesToSpawn));
        }
        float totalEnemyWeight = 0;
        foreach (float enemyWeight in enemyWeights)
        {
            totalEnemyWeight += enemyWeight;
        }
        float enemyValueMultiplier = currentNumberOfEnemiesToSpawn / totalEnemyWeight;
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

    private IEnumerator WaveSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemySpawnPeriod);
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        List<GameObject> enemiesToSpawn = EnemiesToSpawn();
        float enemySpawnAngle = Random.Range(0, 360);
        foreach (GameObject enemy in enemiesToSpawn)
        {
            Vector2 SpawnPosition = enemySpawnRadius * Angle2Direction(enemySpawnAngle + Random.Range(-2.5f, 2.5f));
            float spawnHeight = TargetHeight(SpawnPosition);
            Instantiate(enemy, ConvertToVector3(SpawnPosition) + Vector3.up * spawnHeight, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void SpawnCore()
    {
        Vector2 spawnPos = Random.insideUnitCircle * coreSpawnRadius;
        float spawnHeight = TargetHeight(spawnPos);
        Vector3 coreSpawnPosition = ConvertToVector3(spawnPos) + Vector3.up * spawnHeight;
        theCore = Instantiate(Core, coreSpawnPosition, Quaternion.identity).GetComponent<Structure.Core>();
    }

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