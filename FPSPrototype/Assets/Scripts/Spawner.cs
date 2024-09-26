using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;
    public Transform[] spawnPoints;
    
    //Variables for old system
    //public GameObject[] enemies;
    
    [SerializeField] int enemiesPerWave;
    [SerializeField] float delaySpawn = 1.0f;

    private int spawnCount;
    private int currentWave = 1;

    [SerializeField] GameObject athena;
    [SerializeField] GameObject ares;
    [SerializeField] GameObject apollo;
    [SerializeField] GameObject zeus;


    void Awake()

    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        StartCoroutine(SpawnEnemies(waveNum));
    }

    private IEnumerator SpawnEnemies(int waveNum)
    {
        List<GameObject> enemiesToSpawn = new List<GameObject>();

        if(waveNum >= 1)
        {
            enemiesToSpawn.Add(athena);
        }
        if(waveNum >= 2)
        {

            enemiesToSpawn.Add(ares);
        }
        if(waveNum >= 3)
        {
           
            enemiesToSpawn.Add(apollo);
        }
        if(waveNum >= 4)
        {
            enemiesToSpawn.Add(zeus);

        }
        
        
        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemyToSpawn = enemiesToSpawn[Random.Range(0, enemiesToSpawn.Count)];

            Instantiate(enemyToSpawn, spawnPos.position, spawnPos.rotation);

            yield return new WaitForSeconds(delaySpawn);
        }

        currentWave++;
    }



   /* private IEnumerator SpawnZeus()
    {
        Debug.Log("Spawning Zeus");
        Transform spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(zeus, spawnPos.position, spawnPos.rotation);
        yield return null;
    }
   */
}