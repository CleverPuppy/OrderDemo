using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public MapGenerator map;
    public HudManager hudManager;
    public GameManager gameManager;


    public Camera main_camera;

    float scale = 1.0f;
    
    public int x = 0;
    public int y = 0;

    public Vector3 MOVE_UP = new Vector3(0, 1);
    public Vector3 MOVE_DOWN = new Vector3(0, -1);
    public Vector3 MOVE_LEFT = new Vector3(-1, 0);
    public Vector3 MOVE_RIGHT = new Vector3(1, 0);

    ArrayList arrTaskTaken;

    public int score = 0;
    public int nSafeStepLeft = 0;
    public int rounds = 0;

    void Start()
    {
        scale = map.scale;
        arrTaskTaken = new ArrayList();

        // 无法保证Map先初始化
        if (!map.validLocation(x, y) || !map.validType(x, y, MapGenerator.TILE_TYPE.ROAD)) 
        {
            Debug.Log("随机初始点");
            var random_init = map.getRandomRoadPoint();
            x = random_init.x;
            y = random_init.y;
        }

        gameObject.transform.position = new Vector3(x, y, -1) * scale;
        nSafeStepLeft = gameManager.nSafeStepsEveryRound;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown( KeyCode.S ))
        {
            MoveDown();
        }
        if(Input.GetKeyDown( KeyCode.W ))
        {
            MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
        if(Input.GetKeyDown(KeyCode.Equals))
        {
            ChangeCameraSize(1);
        }
        if(Input.GetKeyDown(KeyCode.Minus))
        {
            ChangeCameraSize(-1);
        }
    }


    void MoveUp()
    {
        if (Moveable(x, y + 1))
        {
            y = y + 1;
            transform.position += MOVE_UP * scale;
            OnStep();
        }
    }

    void MoveDown()
    {
        if (Moveable(x, y - 1))
        {
            y = y - 1;
            transform.position += MOVE_DOWN * scale;
            OnStep();
        }
    }
    void MoveLeft()
    {
        if (Moveable(x - 1, y))
        {
            x = x - 1;
            transform.position += MOVE_LEFT * scale;
            OnStep();
        }
    }
    void MoveRight()
    {
        if (Moveable(x + 1, y))
        {
            x = x + 1;
            transform.position += MOVE_RIGHT * scale; 
            OnStep();
        }
    }

    void OnStep()
    {
        nSafeStepLeft--;

        hudManager.SetLeftSafeStep(nSafeStepLeft);
    }

    void ChangeCameraSize(int delta)
    {
        main_camera.orthographicSize += delta;
    }

    bool Moveable(int x, int y)
    {
        return map.validLocation(x, y) && map.validType(x, y, MapGenerator.TILE_TYPE.ROAD);
    }

    public void NextRound()
    {
        foreach(var task in arrTaskTaken)
        {
            ((Task)task).GetComponent<Task>().NextRound();
        }
        nSafeStepLeft = gameManager.nSafeStepsEveryRound;
        hudManager.SetDangerStep(0);
        hudManager.SetLeftSafeStep(nSafeStepLeft);
        UpdateRound();
        gameManager.OnNextRound();
    }

    void UpdateRound()
    {
        ++rounds;
        hudManager.SetRound(rounds);
    }

    // 接受任务
    public void TakeTask( Task task)
    {
        if(task == null)
        {
            Debug.LogError("错误");
        }
        gameManager.allUntakeTasks.Remove(task.gameObject);
        arrTaskTaken.Add(task);
    }

    public bool isNearPosition(int x_, int y_)
    {
        if(x == x_ && (y == y_ - 1 || y == y_ + 1))
            return true;
        if (y == y_ && (x == x_ - 1 || x == x_ + 1))
            return true;

        return false;
    }

    public void DoneTask(Task task)
    {
        arrTaskTaken.Remove(task);
        score += task.nReward;
        hudManager.ScoreText.text = score.ToString();
    }
};