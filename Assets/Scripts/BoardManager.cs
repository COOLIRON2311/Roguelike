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
    public int columns = 8; // dims of
    public int rows = 8; // game board
    public Count wallCount = new(5, 9); // min/max walls per lvl
    public Count foodCount = new(1, 5); // min/max food per lvl
    public GameObject exit; // single exit
    public GameObject[] floorTiles; // array of floor prefabs
    public GameObject[] wallTiles; // array of wall prefabs
    public GameObject[] foodTiles; // array of food prefabs
    public GameObject[] enemyTiles; // array of enemy prefabs
    public GameObject[] outerWallTiles; // array of outer walls prefabs

    private Transform boardHolder; // keep hierarchy clean
    private readonly List<Vector3> gridPositions = new(); // all possible positions on game board

    /// <summary>
    /// Initialize list of all possible positions on game board
    /// </summary>
    void InitializeList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++) // along x axis
        {
            for (int y = 1; y < rows - 1; y++) // along y axis
            {
                gridPositions.Add(new(x, y, 0f)); // add pos to list
            }
        }
    }
    /// <summary>
    /// Setup floor and outer wall of game board
    /// </summary>
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++) // along x axis
        {
            for (int y = -1; y < rows + 1; y++) // along y axis
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; // floor tile at random
                if (x == -1 || x == columns || y == -1 || y == rows) // outer wall
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)]; // outer wall tile at random

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity); // instantiate chosen tile at (x, y)
                instance.transform.SetParent(boardHolder); // set parent to board holder
            }
        }
    }
    /// <summary>
    /// Generate random position on grid
    /// </summary>
    /// <returns>a random position</returns>
    Vector3 RandomPosition()
    {
        int randIndex = Random.Range(0, gridPositions.Count);
        Vector3 randPosition = gridPositions[randIndex];
        gridPositions.RemoveAt(randIndex); // make sure no duplicates
        return randPosition;
    }
    /// <summary>
    /// Spawn random number of tiles at random positions
    /// </summary>
    /// <param name="tileArray">array of objects to place</param>
    /// <param name="minimum">minimum number of tiles to place</param>
    /// <param name="maximum">maximum number of tiles to place</param>
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objCount; i++)
        {
            Vector3 randPos = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)]; // random tile variant
            Instantiate(tileChoice, randPos, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Math.Log(level, 2f);  // log scale
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity); // always at upper right of level
    }
}
