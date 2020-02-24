using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{
    public int width;
    public int height;

    public int wallCount;
    public int spaceCount;

    public int entranceWidth;
    public int entranceDepth;


    public int cARunCount;

    public string seed;

    public bool vonNeumannNeighbourhood;
    public bool mooreNeighbourhood;
    public bool mooreNeighbourhoodExtended;
    public bool randomSeed;

    public bool entranceNorth;
    public bool entranceEast;
    public bool entranceSouth;
    public bool entranceWest;

    [Range(0, 100)]
    public int fillPercent;

    public int[,] map;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();
        SealGaps();

            if (entranceNorth) {
                GenerateEntranceNorth(entranceWidth, entranceDepth);
            }
            if (entranceEast)
            {
                GenerateEntranceEast(entranceWidth, entranceDepth);
            }
            if (entranceSouth)
            {
                GenerateEntranceSouth(entranceWidth, entranceDepth);
            }
            if (entranceWest)
            {
                GenerateEntranceWest(entranceWidth, entranceDepth);
            }


        for (int i = 0; i < cARunCount; i++)
        {
            CellularAutomata();
        }


        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(map, 1);

    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            GenerateMap();
        }
    }

    //Gets seed for generating map
    void RandomFillMap()
    {
        int randomTime = Random.Range(1, 60);

        if (randomSeed)
        {
            seed = System.DateTime.Now.AddSeconds(Time.time + randomTime).ToString();
        }

        System.Random Randoms = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Randoms.Next(0, 100) < fillPercent)
                {
                    map[x, y] = 1;
                }
                else {
                    map[x, y] = 0;
                }
                  
                
            }
        }
    }

    //Turns outer edge of grid to all 1 or in other words a wall
    void SealGaps()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
            }
        }
    }

    //Celluar Automata algorithm, with clauses for diffrent neighbourhood types
    void CellularAutomata()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(vonNeumannNeighbourhood)
                {
                    int adjacentWalls = GetVonNeumannNeighbourhood(x,y) ;

                    if (adjacentWalls > wallCount)
                        map[x, y] = 1;
                    else if (adjacentWalls <= spaceCount)
                        map[x, y] = 0;


                }
                else if (mooreNeighbourhood)
                {
                    int adjacentWalls = GetMooreNeighbourhood(x, y);

                    if (adjacentWalls >= wallCount)
                        map[x, y] = 1;
                    else if (adjacentWalls <= spaceCount)
                        map[x, y] = 0;
                }
                else if(mooreNeighbourhoodExtended)
                {
                    int adjacentWalls 
                        = GetMooreNeighbourhoodextendedExtended(x, y);

                    if (adjacentWalls >= wallCount)
                        map[x, y] = 1;
                    else if (adjacentWalls <= spaceCount)
                        map[x, y] = 0;
                }
                else
                {
                    Debug.Log("No Neighbourhoods types chosen, " +
                        "please select one to enable celluar automata");
                }


            }
        }
    }

    /// <summary>
    /// Checks 4 surrounding cells in the cardinal directions 
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    int GetVonNeumannNeighbourhood(int gridX, int gridY)
    {
        int wallCount = 0;

        for (int neighbourX = gridX - 1; neighbourX <= gridX; neighbourX++)
        {
            if (neighbourX >= 0 && neighbourX < width)
            {
                if (neighbourX != gridX)
                {
                    wallCount += map[neighbourX, gridY];
                }
            }
            else
            {
                wallCount++;
            }
        }

        for (int neighbourY = gridY - 1; neighbourY <= gridY; neighbourY++)
        {
            if (neighbourY >= 0 && neighbourY < height)
            {
                if (neighbourY != gridY)
                {
                    wallCount += map[neighbourY, gridY];
                }
            }
            else
            {
                wallCount++;
            }
        }

        return wallCount;
    }


    /// <summary>
    /// Check the 8 cells, diagonal and the four cardinal directions
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    int GetMooreNeighbourhood(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; 
            neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width 
                    && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
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
    /// <summary>
    /// Morres neighbourhood r =2 checks 24 cells connected to the selected cell
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
    /// <returns></returns>
    int GetMooreNeighbourhoodextendedExtended(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 2; 
            neighbourX <= gridX + 2; neighbourX++)
        {
            for (int neighbourY = gridY - 2; 
                neighbourY <= gridY + 2; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width 
                    && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
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

    /// <summary>
    /// Sets a defined areas cells to a space at the north most part of the map
    /// </summary>
    /// <param name="eWidth"></param>
    /// <param name="eDepth"></param>
    void GenerateEntranceNorth(int eWidth, int eDepth){
        if (eWidth >= width || eDepth >= height)
        {
            Debug.Log(name + " entrance exceeds permitted size, please select lower values");
        }
        else
        {
            int xPos = width / 2;
            for (int neighbourX = xPos - eWidth; neighbourX <= xPos + eWidth; neighbourX++)
            {
                for (int neighbourY = height - eDepth; neighbourY < height; neighbourY++)
                {
                    map[neighbourX, neighbourY] = 0;
                }
            }
        }
    }
    /// <summary>
    /// Sets a defined areas cells to a space at the north most part of the map
    /// </summary>
    /// <param name="eWidth"></param>
    /// <param name="eDepth"></param>
    void GenerateEntranceEast(int eWidth, int eDepth)
    {
        if (eWidth >= height || eDepth >= height)
        {
            Debug.Log(name + " entrance exceeds permitted size, please select lower values");
        }
        else
        {
            int yPos = height / 2;
            for (int neighbourY = yPos - eWidth; neighbourY <= yPos + eWidth; neighbourY++)
            {
                for (int neighbourX = width - eDepth; neighbourX < width; neighbourX++)
                {
                    map[neighbourX, neighbourY] = 0;
                }
            }
        }
    }
    /// <summary>
    /// Sets a defined areas cells to a space at the north most part of the map
    /// </summary>
    /// <param name="eWidth"></param>
    /// <param name="eDepth"></param>
    void GenerateEntranceSouth(int eWidth, int eDepth)
    {
        if (eWidth >= width || eDepth >= height)
        {
            Debug.Log(name + " entrance exceeds permitted size, please select lower values");
        }
        else
        {
            int xPos = width / 2;
            for (int neighbourX = xPos - eWidth; neighbourX <= xPos + eWidth; neighbourX++)
            {
                for (int neighbourY = 0; neighbourY <= eDepth; neighbourY++)
                {
                    map[neighbourX, neighbourY] = 0;
                }
            }
        }

    }
    /// <summary>
    /// Sets a defined areas cells to a space at the north most part of the map
    /// </summary>
    /// <param name="eWidth"></param>
    /// <param name="eDepth"></param>
    void GenerateEntranceWest(int eWidth, int eDepth)
    {
        if (eWidth >= height || eDepth >= width)
        {
            Debug.Log(name + " entrance exceeds permitted size, please select lower values");
        }
        else
        {
            int yPos = height / 2;
            for (int neighbourY = yPos - eWidth; neighbourY <= yPos + eWidth; neighbourY++)
            {
                for (int neighbourX = 0; neighbourX <= eDepth; neighbourX++)
                {
                    map[neighbourX, neighbourY] = 0;
                }
            }
        }

    }

}
