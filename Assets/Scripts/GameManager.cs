using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject Dancer;
    public GameObject WinMesh;
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
            float CleanedUpCountDown = Mathf.Round(CountDown * 10f) / 10f;
            CountText.GetComponent<TextMeshProUGUI>().text = CleanedUpCountDown.ToString();

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
            Invoke(nameof(GameWinFunc),2f);
        }
    }

    
    public void GameWinFunc()
    {

        Vector3 Spawnloc = Player.transform.position;
        Spawnloc.y -= 1f;
        Vector3 Offset = new Vector3(0, -7, 0);

        // LoadSceneByName(WinScene);
        WinText.SetActive(true);

        if (GameObject.FindGameObjectsWithTag("WinMesh") != null)
        {
            Instantiate(WinMesh, (Player.transform.position), Quaternion.identity);
            Instantiate(Dancer, Player.transform.position + Offset, Quaternion.identity);
        }

        
       
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


    //Use only if 
    public void QuitGame()
    {
        
        Debug.Log("Quit application request received.");

    #if UNITY_EDITOR
        // Stops play mode if running inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            // Closes the actual built application (.exe, .app, etc.)
            Application.Quit();
    #endif
    }


}
