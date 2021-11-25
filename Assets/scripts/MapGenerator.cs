using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public int width;
    public int height;
    public float scale = 1.0f;

    public GameObject Road;
    public GameObject Store;
    public GameObject House;

    public int[][] map;

    public enum TILE_TYPE
    {
        OBSTACLE,
        ROAD,
        STORE,
        HOUSE
    };

    public int nStoreNum;
    public int nMinStoreClusterSize;
    public int nMaxStoreClusterSize; 
    public int nMaxStoreCluster;

    public int nRealStoreNum = 0;
    public int nRealHouseNum = 0;

    public ArrayList arrStores;
    public ArrayList arrHouses;
    public ArrayList arrRoads;

    // Start is called before the first frame update
    void Start()
    {
        arrStores = new ArrayList();
        arrHouses = new ArrayList();
        arrRoads = new ArrayList();

        map = new int[width][];
        for(int i = 0; i < width; i++)
        {
            map[i] = new int[height];
            for(int j = 0; j < height; j++)
            {
                map[i][j] = (int)TILE_TYPE.OBSTACLE;
            }
        }

        // generate road
        ArrayList i_points = new ArrayList();
        ArrayList j_points = new ArrayList();
        fill_random(1, width - 1, 3, 5, i_points);
        fill_random(1, height - 1, 3, 5, j_points);

        foreach(int i in i_points)
        {
           for(int j = 0; j < height; ++j)
            {
                map[i][j] = (int)TILE_TYPE.ROAD;
            }
        }

        foreach(int j in j_points)
        {
            for(int i = 0; i < width; ++i)
            {
                map[i][j] = (int)TILE_TYPE.ROAD;
            }
        }


        // generate store
        for(int nCluster = 0; nCluster < nMaxStoreCluster; ++nCluster)
        {
            int size = Random.Range(nMinStoreClusterSize, nMaxStoreClusterSize );
            var randomCenter = getRandomRoadPoint();
            int added = fill_area(randomCenter, size, size, TILE_TYPE.OBSTACLE, TILE_TYPE.STORE);
            nRealStoreNum += added;
            if(nRealStoreNum > nStoreNum)
            {
                break;
            }
        }

        // else around road will be house
        for(int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                if (validLocation(i, j) && validType(i, j, TILE_TYPE.OBSTACLE) && NearType(i, j, TILE_TYPE.ROAD))
                {
                    map[i][j] = (int)TILE_TYPE.HOUSE;
                    nRealHouseNum++;
                }
            }
        }

        // insatiate road \ house \ store
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                switch (map[i][j])
                {
                    case (int) TILE_TYPE.ROAD:
                        var road = GameObject.Instantiate(Road);
                        SetRenderPosition(road, i, j);
                        arrRoads.Add(road);
                        break;

                    case (int)TILE_TYPE.HOUSE:
                        var house = GameObject.Instantiate(House);
                        house.GetComponent<House>().pos = new Vector2Int(i, j);
                        SetRenderPosition(house, i, j);
                        arrHouses.Add(house);
                        break;

                    case (int)TILE_TYPE.STORE:
                        var store = GameObject.Instantiate(Store);
                        store.GetComponent<Store>().pos = new Vector2Int(i, j);
                        SetRenderPosition(store, i, j);
                        arrStores.Add(store);
                        break;
                }
            }
        }
    }

    public void SetRenderPosition(GameObject obj, int x, int y)
    {
        float rx = x * scale;
        float ry = y * scale;
        obj.transform.position = new Vector3((float)rx, (float)ry, 0.0f);
    }

    void fill_random(int min, int max, int min_Gap, int max_Gap,  ArrayList ret)
    {
        int value = min;
        while(value < max)
        {
            ret.Add(value);
            value += Random.Range(min_Gap, max_Gap);
        }
    }

    int fill_area(Vector2Int center, int range_x, int range_y, TILE_TYPE from, TILE_TYPE to, TILE_TYPE near = TILE_TYPE.ROAD)
    {
        int count = 0;
        for(int i = center.x - range_x; i <= center.x + range_x; ++i)
        {
            for(int j = center.y -range_y; j <= center.y + range_y; ++j)
            {
               if(validLocation(i, j) && validType(i, j, from) && NearType(i, j, near))
               {
                    map[i][j] = (int)to;
                    ++count;
               }
            }
        }
        return count;
    }

    public bool validLocation(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
        {
            return false;
        }
        return true;
    }

    public bool validLocation(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return false;
        }
        return true;
    }

    public bool validType(Vector2Int pos, TILE_TYPE type)
    {
        return map[pos.x][ pos.y] == (int)type;
    }

    public bool validType(int x, int y, TILE_TYPE type)
    {
        return map[x][y] == (int)type;
    }

    public bool NearType(Vector2Int pos, TILE_TYPE type)
    {
        return (validLocation(pos.x - 1, pos.y) && validType(pos.x - 1, pos.y, type)) ||
            (validLocation(pos.x + 1, pos.y) && validType(pos.x + 1, pos.y, type)) ||
            (validLocation(pos.x, pos.y - 1) && validType(pos.x, pos.y - 1, type)) ||
            (validLocation(pos.x, pos.y + 1) && validType(pos.x, pos.y + 1, type));
    }
    public bool NearType(int x, int y, TILE_TYPE type)
    {
        return (validLocation(x - 1, y) && validType(x - 1, y, type)) ||
            (validLocation(x + 1, y) && validType(x + 1, y, type)) ||
            (validLocation(x, y - 1) && validType(x, y - 1, type)) ||
            (validLocation(x, y + 1) && validType(x, y + 1, type));
    }
    public Vector2Int getRandomRoadPoint()
    {
        int MAX_TRIES = 1000;
        Vector2Int ret = new Vector2Int();
        do
        {
            ret.x = Random.Range(1, width - 1);
            ret.y = Random.Range(1, height - 1);
            MAX_TRIES--;
        }while(map[ret.x][ ret.y] != (int)TILE_TYPE.ROAD && MAX_TRIES > 0);

        if(MAX_TRIES == 0)
        {
            Debug.LogError("FATAL ERROR, MAY CHANGE TO A DECENT ALGORITHM");
        } 

        return ret;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
