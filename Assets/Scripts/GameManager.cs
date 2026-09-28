using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float CountDown;
    public float CountDownTime;

    //find all tags, if they're all gone, player wins.

    //tag remains while clock is zero, loss condition.+

    public GameObject CountText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    }
    
}
