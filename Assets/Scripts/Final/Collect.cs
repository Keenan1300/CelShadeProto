using UnityEngine;
using UnityEngine.Events;

public class Collect : MonoBehaviour
{
    public float timer;
    public float loopTime = 3f;
   

    public GameObject Player;
    public bool PlayerInRange;
    private Collider EnterRange;
    public Collider playerbody;
    public Collision player;
    public UnityEvent Popup;
    public UnityEvent Popupclose;

    public UnityEvent DrawGraffiti;

  

    public Vector3 startPos;

    public PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Collider Playr = Player.GetComponent<Collider>();
        Collider EnterRange = GetComponent<Collider>();
        Vector3 Rot = transform.eulerAngles;
        startPos = transform.position;
        PlayerInRange = false;
       
    }

    public void OnTriggerEnter(Collider player) // Use OnTriggerEnter for 3D
    {
        PlayerInRange = true;
       // Destroy(gameObject);
        // Check if the object entering the trigger is the Player
        if (player.CompareTag("Player"))
        {
            // Add code here to increase player score (optional)
            Debug.Log("enter");
            playerController = player.GetComponent<PlayerController>();
            playerController.GraffitLoc = transform.position;
            Popup.Invoke();

            // Destroy the coin object
            //Destroy(gameObject);
        }
     

    }

    public void OnTriggerExit(Collider player)
    {
        PlayerInRange = false;
        // Check if the object entering the trigger is the Player
        if (player.CompareTag("Player"))
        {
            // Add code here to increase player score (optional)
            Debug.Log("leavingradius");
            playerController = player.GetComponent<PlayerController>();
            Popupclose.Invoke();

            // Destroy the coin object
            //Destroy(gameObject);
        }
        
    }

    


    // Update is called once per frame
    void Update()
       {

        if (PlayerInRange && Input.GetKeyDown(KeyCode.E))
        {


            Popupclose.Invoke();
            DrawGraffiti.Invoke();
            Destroy(gameObject);
        }

        Vector3 Rot = transform.rotation.eulerAngles;
        Rot.y += 3;
        transform.eulerAngles = Rot;



        // Increment timer
        timer += Time.deltaTime;

        // Modulo operator resets the timer to prevent floating point inaccuracies
        // at high numbers, keeping the calculation stable over long play sessions.
        if (timer > loopTime)
        {
            timer %= loopTime;
        }

        // Calculate vertical offset using Sine wave
        float newY = startPos.y + (Mathf.Sin(timer * 2f) * 3);

        // Apply new position
        transform.position = new Vector3(startPos.x, newY, startPos.z);


    }
}
