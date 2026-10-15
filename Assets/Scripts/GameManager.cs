using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public float CountDown;
    public float CountDownTime;
    public float PlayerHP;
    public int CanCount;

    public GameObject HealthBar;

    public GameObject Player;
    public GameObject WinText;
    public GameObject LoseText;
    public GameObject LoseMesh;



    private bool GameStateActive = true;
    //find all tags, if they're all gone, player wins.

    //tag remains while clock is zero, loss condition.+

    //Graffiti text
    public GameObject GrafCanCount;
    public GameObject GrafText;

    public GameObject RetryButton;
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

    public void LoadScene(string  sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
            if (TotalTagPoints > 0 && GameStateActive)
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
        Vector3 Spawnloc = Player.transform.position;
        Spawnloc.y -= 1f;
        Vector3 Offset = new Vector3(0, -8, 0);
        Instantiate(LoseMesh, (Player.transform.position + Offset), Quaternion.identity);
        Player.SetActive(false);
        GameStateActive = false;

        //Switch off Game layer UI
        CountText.SetActive(false);
        HealthBar.SetActive(false);
        GrafCanCount.SetActive(false);
        GrafText.SetActive(false);
        //Retry Button
        RetryButton.SetActive(true);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;


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
