using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class PlayerCamController : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playermesh;
    public Transform CameraReset;
    public Rigidbody rb;

    public float RotSpeed;

    public PlayerGrind Grinding;

    public CinemachineCamera Freelook;
    public Transform Freelookpos;

    public CinemachineCamera GrindCam;
    public Transform GrindCampos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrinding = Grinding.onRail;


        if (isGrinding)
        {
            GrindCam.Priority = 20;    // High priority makes this cam active
            Freelook.Priority = 10;

            Freelook.transform.position = new Vector3(GrindCam.transform.position.x, player.transform.position.y, GrindCam.transform.position.z);

            Freelookpos.position = new Vector3 (GrindCam.transform.position.x, player.transform.position.y, GrindCam.transform.position.z);

            Vector3 viewDir = player.position - Freelook.transform.position;
            Freelook.transform.rotation = Quaternion.LookRotation(viewDir);

        }
        else
        {
            //Cam switch 

            GrindCam.Priority = 10;
            Freelook.Priority = 20;
            
            
            //normal operations
           
            
                //Calc direction Orientation
                Vector3 viewDir = playermesh.position - new Vector3(Freelook.transform.position.x, player.position.y, Freelook.transform.position.z);
                orientation.forward = viewDir.normalized;

                //Calc Player object Direction
                float horizontalinput = Input.GetAxisRaw("Horizontal");
                float VerticalInput = Input.GetAxisRaw("Vertical");

          


            Vector3 InputDir = orientation.forward * VerticalInput + orientation.right * horizontalinput;

                if (InputDir != Vector3.zero)
                {
                    if (horizontalinput != 0 || VerticalInput != 0)
                    {
                        playermesh.forward = Vector3.Slerp(playermesh.forward, InputDir.normalized, Time.deltaTime * RotSpeed);
                    }
                }


                // Debug Tweak
                if (Input.GetKey(KeyCode.Escape))
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            
        }


    


    }

    public void resetCampos()

    {

        // 1. Get the player's current Y-axis rotation
        float playerYaw = playermesh.eulerAngles.y;

        // 2. Access the components
        var panTilt = Freelook.GetComponent<CinemachinePanTilt>();
        var orbital = Freelook.GetComponent<CinemachineOrbitalFollow>();

        if (panTilt != null)
        {
            // Force the value to match the player's rotation
            panTilt.PanAxis.Value = playerYaw;
            panTilt.TiltAxis.Value = 0; // Level the camera
        }

        // 3. THE "NO DRIFT" SECRET:
        // If you are using Orbital Follow, it has its own internal offset.
        if (orbital != null)
        {
            // Reset the orbital offset so it's directly behind
            orbital.HorizontalAxis.Value = 0;
        }

        // 4. Force the Cinemachine Brain to forget the "Ease"
        // We 'Warp' the camera so it doesn't try to slide from its previous position
        Freelook.OnTargetObjectWarped(player, Vector3.zero);

        // This internal call clears the 'previous state' which causes the drift
        Freelook.ForceCameraPosition(Freelook.transform.position, Freelook.transform.rotation);




        UpdateOrientation();

        //Freelook.transform.rotation = Quaternion.LookRotation(viewDir);
        //orientation.transform.rotation = Quaternion.LookRotation(viewDir);
        //Debug.Log("resetcam");
    }

    void UpdateOrientation()
    {
        // Use the camera's current forward but flatten the Y so you don't move into the ground
        Vector3 viewDir = player.position - new Vector3(Freelook.transform.position.x, player.position.y, Freelook.transform.position.z);
        orientation.forward = viewDir.normalized;
    }
}

