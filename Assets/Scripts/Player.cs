using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1; // damage player deals to walls
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food; // store player's score during level

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food; // store score in GameManager when level changes
    }

    /// <summary>
    /// Check if game is over
    /// </summary>
    private void CheckIfGameOver()
    {
        if (food <= 0) // if no food is left
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver(); // call GameOver
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (isMoving)
            return;
        food--;
        foodText.text = "Food: " + food;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        Move(xDir, yDir, out hit);
        if (hit.transform == null)
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Exit":
                Invoke("Restart", restartLevelDelay);
                enabled = false;
                break;
            case "Food":
                food += pointsPerFood;
                foodText.text = $"+{pointsPerFood} Food: {food}";
                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
                other.gameObject.SetActive(false);
                break;
            case "Soda":
                food += pointsPerSoda;
                foodText.text = $"+{pointsPerSoda} Food: {food}";
                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
                other.gameObject.SetActive(false);
                break;
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall; // cast component to Wall
        hitWall.DamageWall(wallDamage); // deal damage to the wall
        animator.SetTrigger("playerChop"); // activate trigger
    }

    /// <summary>
    /// Reload the level
    /// </summary>
    private void Restart()
    {
        // Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Handle player being hit
    /// </summary>
    /// <param name="loss">amount of score to lose</param>
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = $"-{loss} Food: {food}";
        CheckIfGameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn)
            return;

        int horizontal = (int)Input.GetAxisRaw("Horizontal");  // x moving dir
        int vertical = (int)Input.GetAxisRaw("Vertical"); // y moving dir

        if (horizontal != 0)
            vertical = 0; // prevent player from moving diagonally

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical); // wall interaction expected
    }
}
