using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;
    
    public GameObject[] enemies;
    public Transform[] spawnPoints;

    [SerializeField] int enemiesPerWave;
    [SerializeField] float delaySpawn = 1.0f;

    private int spawnCount;

    void Awake()

    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Spawner has been initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartWave(int waveNum)
    {
        spawnCount = waveNum * enemiesPerWave;

        //Spawn
        Debug.Log("Coroutine for Spawn is called!");
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {

        for (int i = 0; i < spawnCount; i++)
        {
            Debug.Log("Spawning now");
            Transform spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemyToSpawn = enemies[Random.Range(0, enemies.Length)];

            Instantiate(enemyToSpawn, spawnPos.position, spawnPos.rotation);

            yield return new WaitForSeconds(delaySpawn);
        }
    }
}