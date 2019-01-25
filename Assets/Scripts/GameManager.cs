using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f; //how many seconds before the level starts
    public BoardManager boardScript;  //so you can drag in the board manager script
    public float turnDelay=.1f;  //delay between turns
    public int level = 1;
    public static GameManager instance = null;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;  //text used to display level info in between levels
    private GameObject levelImage;  //goes over normal game UI, used to display levelText
    private bool doingSetup;  //if the gameManager is setting up the next level
    private List<Enemy> enemies;  //holds enemies for the current level
    private bool enemiesMoving;  //if enemies are moving to then next tile

    

    private void Awake()
    {
        if (instance==null) //if there is no active game manager
        {
            instance = this;
        } else if (instance!=this){
            Destroy(gameObject);  //destroy this if redundant
        }
        DontDestroyOnLoad(gameObject);  //persists between levels
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();  
       

    }

    

    void initGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("hideLevelImage", levelStartDelay);  //call hideLevelImage after levelStartDelay seconds

        enemies.Clear();  //clear enemies so that the new level has new enemies
        
        boardScript.setupScene(level); //build the level
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += onLevelFinishedLoading;
    }

    void onLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        
        initGame();
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onLevelFinishedLoading;
    }

    private void hideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    

    public void gameOver()
    {
        levelText.text = "After " + level + " days, you starved";
        levelImage.SetActive(true);
        enabled = false;  //disable the game manager as the game is over
    }
    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving  || doingSetup)  //if it's the player turn, just listen for input (no need to update the frame), otherwise input should not be taken
        {
            return;
        }

        StartCoroutine(moveEnemies());
    }

    public void addEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator moveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count==0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i=0; i<enemies.Count;i++)
        {
            enemies[i].moveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}
