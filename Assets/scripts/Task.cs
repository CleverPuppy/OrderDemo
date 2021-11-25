using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour
{
    public enum TaskState
    {
        INIT, // 刚生成，还未被接单
        PICKING, // 去商家取货
        DILIVERING, // 运送途中
        DONE,    // 完成
    };

    public int nSupposeTime; // 预计时间
    public int nTimelapsed;  // 实际花费时间
    public TaskState state;  // 订单状态
    public int nReward; // 订单奖励

    public Vector2Int picking_place;
    public Vector2Int dilivery_place;

    PlayerController player;

    public GameObject TaskUI;
    public Text submitBtnText;
    public Text taskText;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        if(!player)
        {
            Debug.LogError("PlayerController is NULL");
        }

        // 设置为新的位置
        player.map.SetRenderPosition(TaskUI, dilivery_place.x, dilivery_place.y);
        submitBtnText.text = "接订单";
        taskText.text = "新订单！";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool TakeOrder()
    {
        state = TaskState.PICKING;
        nTimelapsed = 0;
        player.TakeTask(this);
        player.map.SetRenderPosition(TaskUI, picking_place.x, picking_place.y);
        submitBtnText.text = "这里是取货点";
        taskText.text = "取货";
        return true;
    }

    public bool PickingGoods()
    {
        if (player.isNearPosition(picking_place.x, picking_place.y))
        {
            state = TaskState.DILIVERING;
            player.map.SetRenderPosition(TaskUI, dilivery_place.x, dilivery_place.y);
            submitBtnText.text = "送货地点";
            taskText.text = "送货捏";
            Debug.Log("取货完成，开始送货");
            return true;
        }
        return false;
    }

    public bool Delievery()
    {
        if(player.isNearPosition(dilivery_place.x, dilivery_place.y))
        {
            state = TaskState.DONE;
            Debug.Log("送货完成,获得奖励");
            player.DoneTask(this);
            GameObject.DestroyImmediate(gameObject);
            return true;
        }
        return false;
    }

    public void NextRound()
    {
        ++nTimelapsed;
        if(nTimelapsed == nSupposeTime)
        {
            // TODO makeToast;
        }
    }

    public void OnSubmitBtnClicked()
    {
        switch (state)
        {
            case TaskState.INIT:
                TakeOrder();
                break;
            case TaskState.PICKING:
                PickingGoods();
                break;
            case TaskState.DILIVERING:
                Delievery();
                break;
            default:
                Debug.LogError("FATAL ERROR , SHOULDN'T HAPPEN");
                break;
        }
    }


    public void Init(Vector2Int picking_place_, Vector2Int dilivery_place_)
    {
        state = TaskState.INIT;
        nTimelapsed = 1;

        nSupposeTime = 2; // 目前设置为固定值，感觉可以根据距离来设置
        nReward = Random.Range(10, 20);

        picking_place = picking_place_;
        dilivery_place = dilivery_place_;
    }
    
}
