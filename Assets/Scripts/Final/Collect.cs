using UnityEngine;
using UnityEngine.Events;

public class Collect : MonoBehaviour
{


    public GameObject Player;
    private Collider EnterRange;
    public Collider playerbody;
    public Collision player;
    public UnityEvent Popup;

    public PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Collider Playr = Player.GetComponent<Collider>();
        Collider EnterRange = GetComponent<Collider>();
        Vector3 Rot = transform.eulerAngles;
    }

    private void OnTriggerEnter(Collider player) // Use OnTriggerEnter for 3D
    {
        Destroy(gameObject);
        // Check if the object entering the trigger is the Player
        if (player.CompareTag("Player"))
        {
            // Add code here to increase player score (optional)

            playerController = player.GetComponent<PlayerController>(); 
            Popup.Invoke();

            // Destroy the coin object
            //Destroy(gameObject);
        }

    }
        // Update is called once per frame
        void Update()
       {

        Vector3 Rot = transform.rotation.eulerAngles;
        Rot.y += 5;
        transform.eulerAngles = Rot;
        }
}
