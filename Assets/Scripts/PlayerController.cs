using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    public Rigidbody RB;
    private Animator Anim;
    public GameObject PlayerMesh;

    public GameObject JumpDust;

    PlayerGrind PlayerGrind;

    [Header("Inputs")]
    //inputs
    float horizontalinput;
    float VerticalInput;

    //Char Data
    public float movespeed;

    //1 or 0, is the player moving with WASD?
    int InputNum;


    [Header("JumpData")]
    //Jump
    public float Jumpforce;
    public float JumpForwardforce;
    public float JumpCooldown;
    private bool JumpCooled;
    public bool Grounded;
    public float airmultiplier;
    public float gravityMultiplier;
    public float VertFallClamp;


    //Ground Check
    public LayerMask Ground;
    public float playerhieght;

    //Direction Calc
    Vector3 MoveDirection;
    public Transform Orientation;

    public bool OnRail;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerGrind = GetComponent<PlayerGrind>();
        JumpCooled = true;
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        Anim = PlayerMesh.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        OnRail = PlayerGrind.onRail;
        Debug.Log("On Rail is " + OnRail);


        input();

        //ground check
        Grounded = Physics.Raycast(transform.position, Vector3.down, playerhieght * 0.5f + 0.2f, Ground);
        Debug.DrawRay(transform.position, Vector3.down * (playerhieght * 0.5f + 0.2f), Color.red);
        Anim.SetBool("Grounded", Grounded);

        //Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && Grounded && JumpCooled)
        {
            JumpCooled = false;
            Jumplogii();
            Invoke(nameof(resetjump), JumpCooldown);
            
            Anim.SetTrigger("Jump");
            Debug.Log("Jump!");
        }
        else
        {
            Anim.SetBool("Jump", false);
        }

        if (!Grounded) // If the player is falling
        {
            //exponential grav increase as term velo is reached
            gravityMultiplier += Time.deltaTime;
            gravityMultiplier = Mathf.Clamp(gravityMultiplier, 0f, VertFallClamp);


            RB.AddForce(Vector3.down * gravityMultiplier * 2f, ForceMode.Impulse);
            Anim.SetBool("Falling",true);
        }
        else if (Grounded)
        {
            Anim.SetBool("Falling", false);
           
        }

    }

    private void FixedUpdate()
    {
      
  
            MovePlayer();
        
    }

    private void input()
    {
        if (!OnRail)
        {
            //Calc Player input
            horizontalinput = Input.GetAxisRaw("Horizontal");
            VerticalInput = Input.GetAxisRaw("Vertical");
            InputNum = (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0) ? 1 : 0;
        }

    }

    private void MovePlayer()
    {

        if (OnRail)
        {
            //replace with proper grinding anim when the time comes
            Anim.SetBool("Falling", true);
        }

        if (!OnRail)
        {
            //find move dir
            MoveDirection = Orientation.forward * VerticalInput + Orientation.right * horizontalinput;
        }
        
        if (Grounded && !OnRail)
        {
            gravityMultiplier = 0;
            RB.AddForce(MoveDirection.normalized * movespeed * 10f, ForceMode.Force);

        }
        else if (!Grounded && !OnRail)
        {
            RB.AddForce((Vector3.down * 0.5f) * gravityMultiplier, ForceMode.Acceleration);
            //RB.AddForce(MoveDirection.normalized * movespeed * 10f * airmultiplier, ForceMode.Force);

        }

        //Animation
        //Standard Run
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && Grounded && !OnRail)
        {
            Anim.SetBool("IsMoving", true);
        }
        else
        {
            Anim.SetBool("IsMoving", false);
        }
    }

    private void Jumplogii()
    {

       Instantiate(JumpDust,new Vector3(transform.position.x, transform.position.y -4f, transform.position.z), Quaternion.identity);

        RB.linearVelocity = new Vector3(RB.linearVelocity.x, RB.linearVelocity.y, RB.linearVelocity.z);

        RB.AddForce(transform.up * Jumpforce + (PlayerMesh.transform.forward * JumpForwardforce * InputNum), ForceMode.VelocityChange);

    }

    private void resetjump()
    {
        Anim.ResetTrigger("Jump");
        JumpCooled = true;
    }
}
