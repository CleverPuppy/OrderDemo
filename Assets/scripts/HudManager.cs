using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text LogText;
    float logUnitHeight = 10.0f;
    public Scrollbar verticleScrollBar;

    public ArrayList logs = new ArrayList();
    
    public GameObject LogContent;
    public GameManager gameManager;

    public Text ScoreText;
    public Text RoundText;
    public Text leftSafeStep;
    public Text dangerStep;

    void Start()
    {
        logUnitHeight = LogText.flexibleHeight;

        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewContent(string content)
    {
        logs.Add(content);
        int last = logs.Count - 1;
        int beg = last - 5 > 0 ? last - 5 : 0;
        string s = "";
        for(int i = beg; i <= last; ++i)
        {
            s += "\n" + logs[i];
        }
        LogText.text = s;
    }

    public void SetScore(int nScore)
    {
        ScoreText.text= nScore.ToString();
    }    

    public void SetRound(int nRound)
    {
        RoundText.text = nRound.ToString();
    }

    public void SetLeftSafeStep(int nStep)
    {
        if (nStep >= 0)
        {
            leftSafeStep.text = nStep.ToString();
        }
        else
        {
            leftSafeStep.text = 0.ToString();
            SetDangerStep(-nStep);
        }
    }

    public void SetDangerStep(int nStep)
    {
        dangerStep.text = (nStep * gameManager.dangerDrivePenaltyDeath).ToString()  + "%";
    }

}
