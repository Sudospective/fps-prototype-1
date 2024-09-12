using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    float timeScaleOrig;
    int enemyCount;

    //Wave Management
    [SerializeField] int currentWave;
    [SerializeField] int totalWaves;
    [SerializeField] float waveInterval;//if we want to have waves come out on a timer,
                                        //rather than after the entire prior wave is defeated
    public Spawner enemySpawner;

    //Menu
    [SerializeField] bool isPaused;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    //Player
    public GameObject player;      //changed from serialized field to public to grant access to the enemy ai agent -Demetreus
    public playerController playerScript;
    [SerializeField] int playerHealth;
    [SerializeField] public Image playerHPBarFill;
    [SerializeField] public TMP_Text enemyCounter;
    [SerializeField] public TMP_Text waveCounter;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        currentWave = 0;
        nextWave();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void updateGameGoal(int amount)
    {
        Debug.Log("UPDATE GAME GOAL CALLED");

        // increment enemy count by amount
        enemyCount += amount;
        enemyCounter.text = enemyCount.ToString("F0");

        //current wave is over
        if (enemyCount <= 0)
        {
            Debug.Log("ENEMIES ALL ELIMINATED");

            //checks if that was last wave
            if (currentWave == totalWaves)
            {
                //player wins
                statePause();
                isPaused = true;
                menuActive = menuWin;
                menuActive.SetActive(isPaused);
            }
            else
            {
                nextWave();
            }
        }

    }

    public void nextWave()
    {
        currentWave += 1;
        waveCounter.text = currentWave.ToString("F0");
        Debug.Log("Starting wave: " + currentWave);

        //Spawn enemies based on currentWave
        enemySpawner.StartWave(currentWave);

    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }


    //Getters and Setters
    public int CurrentWave => currentWave;
    public int TotalWaves => totalWaves;
    public int PlayerHealth => playerHealth;

    public void setHP(int hp)
    {
        playerHealth = hp;
    }

}
