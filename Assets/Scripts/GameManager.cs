using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public float turnDelay = 0.1f;
    public static GameManager instance = null; // singleton instance holder
    public BoardManager boardScript;
    public int playerFoodPoints = 100; // player's food
    [HideInInspector] public bool playersTurn = true; // turn tracker
    private int level = 8; // test level 3
    private List<Enemy> enemies;
    private bool enemiesMoving;

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

    void InitGame()
    {
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    /// <summary>
    /// Disable GameManager
    /// </summary>
    public void GameOver()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving)
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
