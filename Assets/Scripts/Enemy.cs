using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    private Animator animator;  //hold the animator component
    private Transform target;  
    private bool skipMove;
    // Start is called before the first frame update
    protected override void Start()
    {
        GameManager.instance.addEnemyToList(this); //add enemy to list so that they can be moved individually
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    

    protected override void attemptedMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.attemptedMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void moveEnemy()
    {
        int xDir=0;
        int yDir=0;
        if (Mathf.Abs(target.position.x - transform.position.x)<float.Epsilon)  //if in the same row
        {
            if (target.position.y > transform.position.y)  //if the player is above the enemy, move up
            {
                yDir = 1;
            }
            else
            {
                yDir = -1;  //if the player is below the enemy, move down
            }
        } else if (target.position.x > transform.position.x)  // if the player is more to the right, move right
        {
            xDir = 1;
        }
        else  //otherwise move left
        {
            xDir = -1;
        }
        attemptedMove<Player>(xDir, yDir);  
    }

    protected override void onCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;  //get the player
        animator.SetTrigger("EnemyAttack");  //attack the player
        hitPlayer.loseFood(playerDamage);  //do damage
        
    }
}
