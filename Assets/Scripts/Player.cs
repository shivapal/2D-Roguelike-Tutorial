using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public Text foodText;  //display amount of food points
    public int wallDamage = 1;
    public int pointsPerFood =10;
    public int pointsPerSoda =20;
    public float restartLevelDelay = 1;

    private Animator animator;
    private int food;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;

        foodText.text = "Food " + food;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn)  //if enemies still moving
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        if (horizontal != 0)  //this away avoid diagonal movement
        {
            vertical = 0;
        }

        if (horizontal!=0 || vertical != 0)  //if some input received
        {
            attemptedMove<Wall>(horizontal, vertical);
        }
    }

    protected override void attemptedMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;
        base.attemptedMove<T>(xDir, yDir);
        RaycastHit2D hit;
        if (move(xDir,yDir,out hit))  //for audio implementation
        {

        }
        checkIfGameOver();
        GameManager.instance.playersTurn = false;  //no run enemy code
    }

    protected override void onCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;  //if T is a wall
        hitWall.damageWall(wallDamage); //damage the wall
        animator.SetTrigger("PlayerChop");
    }

    private void OnTriggerEnter2D(Collider2D other)  //for something with a box collider
    {
        if (other.tag == "Exit")
        {
            Invoke("restart", restartLevelDelay);
            enabled = false;  //disable the player so you don't have two
        } else if (other.tag == "Food")
        {
            food = food + pointsPerFood;
            other.gameObject.SetActive(false);
            foodText.text = "+" + pointsPerFood + " Food: " +food;
        } else if (other.tag == "Soda")
        {
            food = food + pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            other.gameObject.SetActive(false);
        }
    }

    private void restart()
    {
        GameManager.instance.level++;
        SceneManager.LoadScene(0);
        
    }
    public void loseFood(int loss)
    {
        animator.SetTrigger("PlayerHit");
        food = food - loss;
        foodText.text = "-" + loss + " Food: " + food;
        checkIfGameOver();
    }
    private void checkIfGameOver()
    {
        if (food <= 0)
        {
            GameManager.instance.gameOver();
        }
    }
}
