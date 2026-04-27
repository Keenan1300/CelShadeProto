using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class PlayerCamController : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playermesh;
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
        }
        else
        {
            GrindCam.Priority = 10;
            Freelook.Priority = 20;
        }


        //normal operations
        if (!isGrinding)
        {
            //Calc direction Orientation
            Vector3 viewDir = player.position - new Vector3(Freelookpos.position.x, player.position.y, Freelookpos.position.z);
            orientation.forward = viewDir;

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
}
