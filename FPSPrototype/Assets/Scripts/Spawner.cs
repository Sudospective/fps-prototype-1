using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;

    public Transform[] spawnPoints;

    [SerializeField] int enemiesPerWave;
    [SerializeField] float delaySpawn = 1.0f;

    private int enemiesRemaining;
    private int spawnCount;

    // Start is called before the first frame update
    void Start()
    {
        enemiesRemaining = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartWave(int waveNum)
    {
        spawnCount = waveNum * enemiesPerWave;

        //Spawn
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        enemiesRemaining = spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(enemy, spawnPos.position, spawnPos.rotation);

            yield return new WaitForSeconds(delaySpawn);
        }
    }
}