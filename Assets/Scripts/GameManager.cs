using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float CountDown;
    public float CountDownTime;
    public float PlayerHP;
    public int CanCount;

    public GameObject HealthBar;

    public GameObject WinText;
    public GameObject LoseText;

    //find all tags, if they're all gone, player wins.

    //tag remains while clock is zero, loss condition.+

    //Graffiti text
    public GameObject GrafCanCount;


    public GameObject[] TagPoints;
    //Timer Text
    public GameObject CountText;
    public int TotalTagPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TagPoints = GameObject.FindGameObjectsWithTag("TagPoint");

        foreach (GameObject obj in TagPoints)
        {
            // Do something with each object
            TotalTagPoints++;
        }

        CountDown = CountDownTime;
        //Convert Time to on screen signifier
        CountText.GetComponent<TextMeshProUGUI>().text = CountDown.ToString();
    }

    public void startTimer(float CountDownTime)
    {
        //CountText.GetComponent<Timer+>
    }

    // Update is called once per frame
    void Update()
    {
        if (CountDown > 0)
        {

            CountDown -= Time.deltaTime;
            CountText.GetComponent<TextMeshProUGUI>().text = CountDown.ToString();

        }
        else 
        {
            if (TotalTagPoints > 0)
            {
                GameLostFunc();
            }
        }

        if (TotalTagPoints < 1)
        {
            GameWinFunc();
        }
    }

    
    public void GameWinFunc()
    {
        // LoadSceneByName(WinScene);
        WinText.SetActive(true);
    }

    public void GameLostFunc()
    {
        //LoadSceneByName(LoseScene);
        LoseText.SetActive(true);
    }

    public void RemoveGrafPoint()
    {
        TotalTagPoints--;
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    public void UpdateStatsData()
    {
        GrafCanCount.GetComponent<TextMeshProUGUI>().text = CanCount.ToString();
        HealthBar.GetComponent<HealthBar>().SetHealth(PlayerHP);
    }

}
