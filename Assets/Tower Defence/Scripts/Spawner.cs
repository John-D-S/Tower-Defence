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
    private float coreSpawnRadius;
    [SerializeField]
    private GameObject Core;
    [SerializeField, Tooltip("The time between waves")]
    private float enemySpawnPeriod;
    [SerializeField]
    private float baseSpawnRate;
    [SerializeField, Tooltip("relates to how quickly enemy difficulty and amount increases with each wave")]
    private float difficultyCurveExponent = 0.075f;

    [SerializeField]
    HealthBar healthBar;

    //the enemies spawned per wave
    private int numberOfEnemiesToSpawn
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

    // Update is called once per frame
    private void Start()
    {
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
        int enemiesToSpawnNow = numberOfEnemiesToSpawn;
        float enemySpawnAngle = Random.Range(0, 360);
        for (int i = 0; i < enemiesToSpawnNow; i++)
        {
            Vector2 SpawnPosition = enemySpawnRadius * Angle2Direction(enemySpawnAngle + Random.Range(-2.5f, 2.5f));
            float spawnHeight = TargetHeight(SpawnPosition);
            Instantiate(Enemy, ConvertToVector3(SpawnPosition) + Vector3.up * spawnHeight, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
