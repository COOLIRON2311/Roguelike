using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; // singleton instance holder
    public BoardManager boardScript;
    public int playerFoodPoints = 100; // player's food
    [HideInInspector] public bool playersTurn = true; // turn tracker
    private int level = 3; // test level 3

    void Awake()
    {
        if (instance == null) // if there is no instance...
            instance = this; // assign self to it
        else if (instance != this) // if self is not instance...
            Destroy(gameObject); // destroy it!

        DontDestroyOnLoad(gameObject); // to keep data between scenes
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
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

    }
}
