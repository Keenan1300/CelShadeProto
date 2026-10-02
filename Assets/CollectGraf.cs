using UnityEngine;
using UnityEngine.Events;

public class CollectGraf : MonoBehaviour
{
    public float timer;
    public float loopTime = 3f;

    public AudioSource MusicPlayer;
    public GameObject Player;
    public bool PlayerInRange;
    private Collider EnterRange;
    public Collider playerbody;
    public Collision player;


    public AudioClip SprayCollect;
    public UnityEvent DrawGraffiti;



    public Vector3 startPos;

    public PlayerController playerController;
    public GrindCanCollect GrindCollect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;  
    }


    public void OnTriggerEnter(Collider player)
    {
        PlayerInRange = false;
        // Check if the object entering the trigger is the Player
        if (player.CompareTag("Player"))
        {
            // Add code here to increase player score (optional)
            Debug.Log("leavingradius");

            playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.AddCan(1);
            }
            else
            {
                GrindCollect = player.GetComponent<GrindCanCollect>();
                GrindCollect.CollectGraffitiCan.Invoke();
            }




            // Destroy the coin object
            //Destroy(gameObject);
        }

        MusicPlayer.PlayOneShot(SprayCollect);
        Destroy(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        //hover
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
