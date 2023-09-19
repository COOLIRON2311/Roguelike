using System;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    // Start is called before the first frame update
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove) // act every other move
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    /// <summary>
    /// Used by GameManager to move each enemy in enemy list
    /// </summary>
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Math.Abs(target.position.x - transform.position.x) < float.Epsilon) // enemy and player are in a same column
            yDir = target.position.y > transform.position.y ? 1 : -1; // move up/down
        else
            xDir = target.position.x > transform.position.x ? 1 : -1; // move along horizontal axis

        AttemptMove<Player>(xDir, yDir);
    }

    /// <summary>
    /// Enemy attempts to move into space occupied by player
    /// </summary>
    /// <typeparam name="T">component type</typeparam>
    /// <param name="component">player</param>
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");
        hitPlayer.LoseFood(playerDamage);
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);

    }
}
