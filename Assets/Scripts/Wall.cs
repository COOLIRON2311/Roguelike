using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite; // sprite to display once player hits the wall
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    /// <summary>
    /// Deal damage to wall
    /// </summary>
    /// <param name="loss">amount of damage</param>
    public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite; // visual feedback
        hp -= loss;
        if (hp <= 0) // if wall has no hp left then destroy it!
            gameObject.SetActive(false);
    }
}
