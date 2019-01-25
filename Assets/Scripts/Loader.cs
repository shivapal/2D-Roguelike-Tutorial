using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{

    public GameObject gameManager;  //so you can drag in a game manager script
    //use awake when a game starts
    void Awake()
    {
        if (GameManager.instance == null) //if the game manager isn't running, start it
        {
            Instantiate(gameManager);
        }   
    }
}
