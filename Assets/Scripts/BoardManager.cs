using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5,9);
    public Count foodCount = new Count(1,5);
    //make these public so you can drag and drop sprites
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    private Transform boardHolder;  //made to be a parent for board elements for organization purposes
    private List<Vector3> gridPositions = new List<Vector3>();  //list holds positions where interactable objects can go

    
    void initializeList()
    {
        gridPositions.Clear();
        for (int x =1; x<columns-1; x++)
        {
            for (int y = 1; y<rows-1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void boardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        //go through all positions, making either floor or outer walls
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, outerWallTiles.Length)]; //select a random floor prefab
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)]; //select a random floor prefab
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //make a gameObject using the prefab
                instance.transform.SetParent(boardHolder);
            }
        }

    }

    Vector3 randomPosition()
    {
        int randomIndex = Random.Range(0,gridPositions.Count-1);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);  //something is here so don't use it again
        return randomPosition;
    }

    void layoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum);
        for (int i = 0; i <objectCount; i++) //for all the objects of a type you want to place
        {
            Vector3 randPosition = randomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];  //pick a prefab of the type
            Instantiate(tileChoice, randPosition, Quaternion.identity);  //make a game object using the prefab
        }
    }

    public void setupScene(int level)
    {
        boardSetup();
        initializeList();
        layoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        layoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        layoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
