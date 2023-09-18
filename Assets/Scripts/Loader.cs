using UnityEngine;
/// <summary>
/// Load GameManager when the game starts
/// </summary>
public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    void Awake()
    {
        if (GameManager.instance == null) // check if GameManager has been instantiated
            Instantiate(gameManager); // if not then do it
    }
}
