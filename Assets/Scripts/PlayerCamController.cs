using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCamController : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playermesh;
    public Rigidbody rb;

    public float RotSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Calc direction Orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
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

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
