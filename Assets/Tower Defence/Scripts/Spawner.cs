using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClasses.HelperFunctions;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private float enemySpawnRadius = 950f;
    [SerializeField]
    private GameObject Enemy;
    [SerializeField]
    private float coreSpawnRadius = 700f;
    [SerializeField]
    private GameObject Core;
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
        StartCoroutine(WaveSpawner());
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
        int enemiesToSpawnNow = NumberOfEnemiesToSpawn;
        float enemySpawnAngle = Random.Range(0, 360);
        for (int i = 0; i < enemiesToSpawnNow; i++)
        {
            Vector2 SpawnPosition = enemySpawnRadius * Angle2Direction(enemySpawnAngle + Random.Range(-2.5f, 2.5f));
            float spawnHeight = TargetHeight(SpawnPosition);
            Instantiate(Enemy, ConvertToVector3(SpawnPosition) + Vector3.up * spawnHeight, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void SpawnCore()
    {
        Vector2 spawnPos = Random.insideUnitCircle * coreSpawnRadius;
        float spawnHeight = TargetHeight(spawnPos);
        Vector3 coreSpawnPosition = ConvertToVector3(spawnPos) + Vector3.up * spawnHeight;
        Instantiate(Core, coreSpawnPosition, Quaternion.identity);
    }
}