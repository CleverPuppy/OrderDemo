using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController playerController;
    public MapGenerator map;
    public GameObject UICanvas;
    public GameObject TaskObject;

    public int generate_task_num_per_round = 5;

    public int nSafeStepsEveryRound = 20;
    public float dangerDrivePenaltyDeath = 0.01f;

    public ArrayList allUntakeTasks;

    int[] store_indexes;
    int[] house_indexes;

    void Start()
    {
        map = FindObjectOfType<MapGenerator>();


        allUntakeTasks = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameStart()
    {
        Debug.Log("游戏开始");
    }

    public void OnNextRound()
    {
        Debug.Log("进行下一个回合");
        CleanUntakenTasks();
        GenerateTask();
    }

    void GenerateTask()
    {
        if(store_indexes == null)
        {
            store_indexes = new int[map.arrStores.Count];
            for(int i = 0; i < store_indexes.Length; ++i)
            {
                store_indexes[i] = i;
            }
        }

        if (house_indexes == null)
        {
            house_indexes = new int[map.arrHouses.Count];
            for (int i = 0; i < house_indexes.Length; ++i)
            {
                house_indexes[i] = i;
            }
        }


        Debug.Log("生成订单");
        shuffle_index(house_indexes, generate_task_num_per_round);
        shuffle_index(store_indexes, generate_task_num_per_round);

        for( int i = 0; i < generate_task_num_per_round; ++i )
        {
            GameObject task = GameObject.Instantiate(TaskObject, UICanvas.transform);
            GameObject house = (GameObject)map.arrHouses[house_indexes[i]];
            GameObject store = (GameObject)map.arrStores[store_indexes[i]];
            task.GetComponent<Task>().Init(store.GetComponent<Store>().pos, house.GetComponent<House>().pos);
            task.SetActive(true);
            allUntakeTasks.Add(task);
        }
    }

    void CleanUntakenTasks()
    {
        for(int i = 0; i < allUntakeTasks.Count; ++i)
        {
            GameObject.Destroy(
                (GameObject)allUntakeTasks[i]
            );
        }
        allUntakeTasks.Clear();
    }

    // 前nTime个为随机数
    void shuffle_index(int[] arr, int nTime)
    {
        nTime = arr.Length < nTime ? arr.Length : nTime;
        for(int i = 0; i < nTime; ++i)
        {
            int random_index = Random.Range(i , arr.Length);
            int tmp = arr[i];
            arr[i] = arr[random_index];
            arr[random_index] = tmp;
        }
    }
}   
