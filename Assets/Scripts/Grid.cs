using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    [Range(5, 25)]
    public int height = 10;         // The height of the map
    [Range(5, 25)]
    public int width = 10;          // The width of the map

    [Range(1, 5)]
    public int entranceNum = 1;     // The number of entrances

    [Range(1, 5)]
    public int shopNum = 2;         // The number of shops

    [Range(1, 5)]
    public int houseNum = 3;        // The number of houses

    [Range(0, 100)]
    public float fillPercent;       // The percentage of walls


    public string seed;
    private float increment;

    
    private List<List<Node>> grid = new List<List<Node>>();         // The grid for the map
    private List<List<Node>> tempGrid = new List<List<Node>>();

    private List<Node> walls = new List<Node>();                    // The walls surrounding the map
    private List<Node> freeSpaces = new List<Node>();               // The free spaces of the map

    public Node node;   // The prefab to spawn

    void Start()
    {
        SetUp();        

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetUp();
        }
    }

    /// <summary>
    /// Runs when you start the program and when you press the Spacebar
    /// </summary>
    public void SetUp()
    {
        //seed = UnityEngine.Random.Range(0f, 1000f);
        seed = UnityEngine.Random.Range(0, 1000).ToString();
    

        System.Random rand = new System.Random(seed.GetHashCode());
        walls.Clear();
        freeSpaces.Clear();
        foreach(List<Node> node in grid)
        {
            foreach (Node node2 in node)
            {
                Destroy(node2.gameObject);
            }
        }
        foreach (List<Node> node in tempGrid)
        {
            foreach (Node node2 in node)
            {
                Destroy(node2.gameObject);
            }
        }
        grid.Clear();

        tempGrid.Clear();

        GenerateGrid(rand);
        tempGrid = grid;

        for (int i = 0; i < 3; i++)
        {
            SmoothMap();
            grid = tempGrid;
        }

        GenerateBuildings();
    }

    #region Map Generation

    /// <summary>
    /// Generates the grid using Perlin Noise
    /// </summary>
    public void GenerateGrid(System.Random seed)
    {
        float xCoord = 0;
        increment = (float)seed.Next(0, 100) / 1000;


        for (int i = 0; i < width; i++)
        {
            float yCoord = 0;
            xCoord += increment;

            List<Node> temp = new List<Node>();
            for (int j = 0; j < height; j++)
            {
                yCoord += increment;

                Node n = Instantiate(node, new Vector3((i/2f) - width / 4, (j/2f) - height / 4, 0), Quaternion.identity);

                float noise = Mathf.PerlinNoise(xCoord, yCoord);
                n.val = noise;
                n.active = 0;
                if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                {
                    n.active = 1;
                }
                else if (noise < (fillPercent / 100f))
                {
                    n.active = 1;
                }
                
                n.SetMat();
                temp.Add(n);
            }
            grid.Add(temp);
        }
    }

    /// <summary>
    /// Removes small sections of the map and adds onto the bigger sections
    /// </summary>
    private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetNearbyWalls(x, y);

                if (neighbourWallTiles > 4)
                {
                    grid[x][y].active = 1;
                }
                else if (neighbourWallTiles < 4)
                {
                    grid[x][y].active = 0;
                }
                grid[x][y].SetMat();


            }
        }

    }

    /// <summary>
    /// Gets all of the walls around a node and returns the amount
    /// </summary>
    /// <param name="x">The x location of the node we're checking</param>
    /// <param name="y">The y location of the node we're checking</param>
    /// <returns></returns>
    private int GetNearbyWalls(int x, int y)
    {
        int wallCount = 0;
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    // If its not the current square
                    if (neighbourX != x || neighbourY != y)
                    {
                        if (tempGrid[neighbourX][neighbourY].active >= (fillPercent / 100f))
                        {
                            wallCount++;
                        }

                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    #endregion

    #region Building Generation
    /// <summary>
    /// Generates the buildings
    /// </summary>
    public void GenerateBuildings()
    {
        CheckIfWall();
        GenerateEntrance();
        GenerateShops();
        GenerateHouses();

    }

    /// <summary>
    /// Generates an entrance from the walls surrounding the map
    /// </summary>
    public void GenerateEntrance()
    {
        int i = 0;
        while (i < entranceNum)
        {
            if (walls.Count <= 0)
            {
                i = entranceNum;
            }
            Node entrance = walls[UnityEngine.Random.Range(0, walls.Count - 1)];
            if (entrance.buildingType != BuildingType.Entrance)
            {
                entrance.buildingType = BuildingType.Entrance;
                entrance.SetMat(Color.green);
                i++;
            }
            
        }
    }

    /// <summary>
    /// Looks at the free spaces and places a shop
    /// </summary>
    public void GenerateShops()
    {
        int i = 0;
        while (i < shopNum)
        {
            if (freeSpaces.Count <= 0)
            {
                i = shopNum;
            }
            Node shop = freeSpaces[UnityEngine.Random.Range(0, freeSpaces.Count - 1)];
            if (shop.buildingType != BuildingType.Shop)
            {
                shop.buildingType = BuildingType.Shop;
                shop.SetMat(Color.blue);
                freeSpaces.Remove(shop);

                i++;
            }
            
        }
    }

    /// <summary>
    /// Looks at the free spaces and places a shop
    /// </summary>
    public void GenerateHouses()
    {
        int i = 0;
        while (i < houseNum)
        {
            if (freeSpaces.Count <= 0)
            {
                i = houseNum;
            }
            Node house = freeSpaces[UnityEngine.Random.Range(0, freeSpaces.Count - 1)];
            if (house.buildingType != BuildingType.House && house.buildingType != BuildingType.Shop)
            {
                house.buildingType = BuildingType.House;
                house.SetMat(Color.yellow);
                freeSpaces.Remove(house);

                i++;
            }

        }
    }

    /// <summary>
    /// Helper method for seperating walls and free spaces 
    /// </summary>
    public void CheckIfWall()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetNearbyWalls(x, y);

                if ((neighbourWallTiles > 0 && neighbourWallTiles != 8) && grid[x][y].active == 1)
                {
                    grid[x][y].buildingType = BuildingType.Wall;
                    grid[x][y].SetMat(Color.red);

                    walls.Add(grid[x][y]);
                }
                else if(neighbourWallTiles != 8 && grid[x][y].active == 0)
                {
                    freeSpaces.Add(grid[x][y]);
                }

            }
        }
    }

    #endregion

}
