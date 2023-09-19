using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f; // time to wait before starting levels
    public float turnDelay = 0.1f;
    public static GameManager instance = null; // singleton instance holder
    public BoardManager boardScript;
    public int playerFoodPoints = 100; // player's food
    [HideInInspector] public bool playersTurn = true; // turn tracker

    private Text levelText;
    private GameObject levelImage;
    private int level = 1; // test level 3
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    void Awake()
    {
        if (instance == null) // if there is no instance...
            instance = this; // assign self to it
        else if (instance != this) // if self is not instance...
            Destroy(gameObject); // destroy it!

        DontDestroyOnLoad(gameObject); // to keep data between scenes
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    void InitGame()
    {
        doingSetup = true; // turn off player movement
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        enemies.Clear();
        boardScript.SetupScene(level);
        Invoke("HideLevelImage", levelStartDelay);
    }

    /// <summary>
    /// Turn off level image after setup is done
    /// </summary>
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    /// <summary>
    /// Disable GameManager
    /// </summary>
    public void GameOver()
    {
        levelText.text = "After " + level + "days, you starved.";
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;
        StartCoroutine(MoveEnemies());
    }

    /// <summary>
    /// Have enemies register themselves with GameManager so it can issue movement orders
    /// </summary>
    /// <param name="script">an enemy to register</param>
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    /// <summary>
    /// Move each enemy in enemies list
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0) // if no enemies spawned yet
        {
            yield return new WaitForSeconds(turnDelay);
        }
        foreach (var e in enemies)
        {
            e.MoveEnemy();
            yield return new WaitForSeconds(e.moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}
