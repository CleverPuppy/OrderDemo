using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour
{
    public enum TaskState
    {
        INIT, // �����ɣ���δ���ӵ�
        PICKING, // ȥ�̼�ȡ��
        DILIVERING, // ����;��
        DONE,    // ���
    };

    public int nSupposeTime; // Ԥ��ʱ��
    public int nTimelapsed;  // ʵ�ʻ���ʱ��
    public TaskState state;  // ����״̬
    public int nReward; // ��������

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

        // ����Ϊ�µ�λ��
        player.map.SetRenderPosition(TaskUI, dilivery_place.x, dilivery_place.y);
        submitBtnText.text = "�Ӷ���";
        taskText.text = "�¶�����";
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
        submitBtnText.text = "������ȡ����";
        taskText.text = "ȡ��";
        return true;
    }

    public bool PickingGoods()
    {
        if (player.isNearPosition(picking_place.x, picking_place.y))
        {
            state = TaskState.DILIVERING;
            player.map.SetRenderPosition(TaskUI, dilivery_place.x, dilivery_place.y);
            submitBtnText.text = "�ͻ��ص�";
            taskText.text = "�ͻ���";
            Debug.Log("ȡ����ɣ���ʼ�ͻ�");
            return true;
        }
        return false;
    }

    public bool Delievery()
    {
        if(player.isNearPosition(dilivery_place.x, dilivery_place.y))
        {
            state = TaskState.DONE;
            Debug.Log("�ͻ����,��ý���");
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

        nSupposeTime = 2; // Ŀǰ����Ϊ�̶�ֵ���о����Ը��ݾ���������
        nReward = Random.Range(10, 20);

        picking_place = picking_place_;
        dilivery_place = dilivery_place_;
    }
    
}
