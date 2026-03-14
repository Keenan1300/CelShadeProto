using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Rigidbody RB;
    private Animator Anim;
    public GameObject PlayerMesh;
    float horizontalinput;
    float VerticalInput;
    public float movespeed;

    Vector3 MoveDirection;

    public Transform Orientation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        Anim = PlayerMesh.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        input();
        //RB.AddForce();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void input()
    {
        //Calc Player input
         horizontalinput = Input.GetAxisRaw("Horizontal");
         VerticalInput = Input.GetAxisRaw("Vertical");


    }

    private void MovePlayer()
    {
        //find move dir
        MoveDirection = Orientation.forward * VerticalInput + Orientation.right * horizontalinput;
        RB.AddForce(MoveDirection.normalized*movespeed*10f,ForceMode.Force);

        //Animation
        if (Input.GetAxisRaw("Horizontal") != 0 ||  Input.GetAxisRaw("Vertical") != 0)
        {
            Anim.SetBool("IsMoving", true);
        }
        else
        {
            Anim.SetBool("IsMoving", false);
        }

    }


}
