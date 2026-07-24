using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    public Rigidbody RB;
    public Animator Anim;

    //visual graphic char
    public GameObject PlayerMesh;

    //Rotation logic gameobject
    public GameObject PlayerRotAxis;

    //Spray Data
    public GameObject GraffitiSprayAnim;
    public Vector3 GraffitLoc;

    public GameObject JumpDust;
    bool Touchpoint;

    PlayerGrind PlayerGrind;

    [Header("Inputs")]
    //inputs
    float horizontalinput;
    float VerticalInput;

    //Char Data
    public float movespeed;

    //1 or 0, is the player moving with WASD?
    public int InputNum;


    [Header("JumpData")]

    //Jump
    public float Jumpforce;
    public float JumpForwardforce;
    public float JumpCooldown;
    public bool JumpCooled;
    public bool Grounded;

    public float airmultiplier;

    //how long is player affected by gravity?
    public float gravitytimer;
    //how strong is this gravity?
    public float gravityMultiplier;

    public float VertFallClamp;
    public float AirTime;
    public float AirTimeDefault;
    public float AirTimeGrind;


    //Grind Jump special air movement\\

    //How much can the player move while in air from a grind?
    public float GrindAirManeuverability;
    public float specialAirGrindHeight;
    public bool GrindAir;


    //Ground Check
    public LayerMask Ground;
    public float playerhieght;

    //Grind Check
    public bool RailGrind;
    public bool WallGrindR;
    public bool WallGrindL;

    //Dance
    public bool Dancing;
    public float DanceDuration;
    public GameObject Dancer;

    //Direction Calc Y
    Vector3 MoveDirection;
    public Transform Orientation;

    public bool OnRail;
    public bool SprayScene;


    public bool GraffitiRange;


    //Direction Calc X
    public float XCast;
    public float YCast;
    public float XLimitCast;
    public float YLimitCast;
    private Vector3 CPoint;
    private Vector3 BPoint;

    //angular math
    public float angle;
    public float angleADeg;
    private float angularmomentum;

    public AnimationCurve AnimCurve;
    public float Timer;
    private RaycastHit slopeHit;

    //UI
    public GameObject SprayPrompt;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerGrind = GetComponent<PlayerGrind>();
        JumpCooled = true;
        GrindAir = false;
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        Anim = PlayerMesh.GetComponent<Animator>();
        SprayPrompt.SetActive(false);

        Dancing = false;
        GraffitiRange = false;
        SprayScene = false;
    }

    // Update is called once per frame
    void Update()
    {
        



            OnRail = PlayerGrind.onRail;

        //Anim.SetBool("Grinding", OnRail);

        //Dancing!
        if (Input.GetKeyDown(KeyCode.Q) && Grounded && !OnRail)
        {
            RB.isKinematic = true;
            Dancing = true;
            PlayerMesh.SetActive(false);

            Invoke(nameof(Dance), DanceDuration);
        }

        input();



        //ground check
        Grounded = Physics.SphereCast(PlayerRotAxis.transform.position, 2f, Vector3.down, out RaycastHit hit2, playerhieght * 0.5f + 0.2f, Ground);
        Debug.DrawRay(transform.position, Vector3.down * (playerhieght * 0.5f + 0.2f), Color.red);

        //Anim updates
        Anim.SetBool("Grounded", Grounded);
        Anim.SetBool("Grinding", RailGrind);
        Anim.SetBool("WallGrindR", WallGrindR);
        Anim.SetBool("WallGrindL", WallGrindL);


        //Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && Grounded && JumpCooled)
        {
            JumpCooled = false;
            PlayerGrind.JumpCooldown = false;

            Anim.SetBool("Jump", true);
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
            gravitytimer += Time.deltaTime * AirTime;
            gravitytimer = Mathf.Clamp(gravitytimer, 0f, VertFallClamp);


            RB.AddForce(Vector3.down * gravitytimer * 2f, ForceMode.Impulse);
            Anim.SetBool("Falling", true);

            Touchpoint = true;
        }
        else if (Grounded)
        {
            //Test
            //PlayerRotAxis
            //if (angleADeg < 45f)
            //{
            //    transform.rotation = Quaternion.Euler(angleADeg, PlayerRotAxis.transform.rotation.y, PlayerRotAxis.transform.rotation.z);
            //}

            SurfaceAlign();
            GrindAir = false;
            Anim.SetBool("GrindAir", false);
            Anim.SetBool("Falling", false);

            //aesthetics
            if (Touchpoint == true)
            {
                Createdustcloud();
            }

        }

        //Spray


        if (SprayScene)
        {
            SprayPrompt.SetActive(false);
        }

        if (GraffitiRange && Grounded && Input.GetKeyDown(KeyCode.E))
        {


            Vector3 Graflookdir = GraffitLoc - PlayerRotAxis.transform.position;
            Graflookdir.x = 0f;
            Quaternion lookthere = Quaternion.LookRotation(Graflookdir);

            SprayScene = true;
            Instantiate(GraffitiSprayAnim, PlayerMesh.transform.position, PlayerRotAxis.transform.rotation);

            //make player invisible
            gameObject.SetActive(false);
        }

    }


    public void SurfaceAlign()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit info = new RaycastHit();
        Quaternion rf = Quaternion.Euler(0, 0, 0);

        if (Physics.Raycast(ray, out info, Ground))
        {

            //  rf = Quaternion.Lerp(transform.rotation , Quaternion.FromToRotation(Vector3.up, info.normal), aniCurve.Evaluate(Timer));
            //  transform.rotation = Quaternion.Euler(rf.eulerAngles.x, transform.eulerAngles.y,rf.eulerAngles.z);

            rf = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, info.normal), AnimCurve.Evaluate(Timer));
            transform.rotation = Quaternion.Euler(rf.eulerAngles.x, transform.eulerAngles.y, rf.eulerAngles.z);
        }

        if (YCast > 5f && YCast < 10f)
        {
            transform.position -= Vector3.down;
        }

        //Ground 'move gap' Fix on slopes
        RaycastHit hit;

        if (Physics.Raycast(CPoint, Vector3.down, out hit, YLimitCast))
        {
            // Extract the distance as a float
            YCast = hit.distance;
        }

        if (Physics.Raycast(transform.position, PlayerMesh.transform.forward, out hit, XLimitCast))
        {
            // Extract the distance as a float
            XCast = hit.distance;
        }

        //YCast to floor Calc
        CPoint = (transform.position) + PlayerRotAxis.transform.forward * XCast;
        BPoint = new Vector3(CPoint.x, CPoint.y - YCast, CPoint.z);
        //X rotation calculation
        float angleARadians = Mathf.Atan2(YCast, XCast);
        angleADeg = angleARadians * Mathf.Rad2Deg;

        DrawDebugTriangle();
    }


    public void DrawDebugTriangle()
    {

        Debug.DrawLine(transform.position, BPoint);
        Debug.DrawLine(CPoint, transform.position);
        Debug.DrawLine(BPoint, CPoint);
    }


    public void Dance()
    {
        Instantiate(Dancer, PlayerMesh.transform.position, Quaternion.identity);
        //Dancing = false;
    }

    private void ResetGravity()
    {
        gravityMultiplier = 150f;
    }

    private void FixedUpdate()
    {

        if (GrindAir)
        {
            RailGrind = false;
            WallGrindR = false;
            WallGrindL = false;

            Anim.SetBool("GrindAir", true);
            //AirTime = 0.99f;
            gravityMultiplier = specialAirGrindHeight;
            RB.AddForce(MoveDirection.normalized * movespeed * 10f / GrindAirManeuverability, ForceMode.Force);

            //overtime decrease back to gravity
            RB.AddForce((Vector3.down * gravityMultiplier) * (gravitytimer * 3f), ForceMode.Acceleration);

        }
            else if (!GrindAir)
            {

              Invoke(nameof(ResetGravity), 0.5f);
            }

        if (!Dancing)
        {
            MovePlayer();
        }

    }

    private void input()
    {
        if (!OnRail && !SprayScene)
        {
            //Calc Player input
            horizontalinput = Input.GetAxisRaw("Horizontal");
            VerticalInput = Input.GetAxisRaw("Vertical");
            InputNum = (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0) ? 1 : 0;

            //if (Dancing && horizontalinput != 0 || VerticalInput != 0)
            //{
            //  PlayerMesh.SetActive(true);
            //}

        }

    }

    private void MovePlayer()
    {


        if (OnRail)
        {
            Debug.Log("Hortiz" + Input.GetAxisRaw("Horizontal"));
            Anim.SetBool("GrindAir", false);
            //replace with proper grinding anim when the time comes
            //Anim.SetBool("Grinding", true);

        }

        if (!OnRail)
        {

            //find move dir
            MoveDirection = Orientation.forward * VerticalInput + Orientation.right * horizontalinput;

        }

        if (Grounded && !OnRail)
        {
            AirTime = AirTimeDefault;
            gravitytimer = 0;
           

            if (OnSlope())
            {
                Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(MoveDirection, slopeHit.normal).normalized;
                angularmomentum = angle / 100f;
                RB.linearVelocity = slopeMoveDirection * movespeed * angularmomentum * 7;
                // Apply a continuous down-force relative to the slope angle to stick to the ground
                if (RB.linearVelocity.y > 0)
                {
                    RB.AddForce(-slopeHit.normal * movespeed, ForceMode.Force);
                }
            }
            else 
            {
                RB.AddForce(MoveDirection.normalized * movespeed * 10f, ForceMode.Force);
            }

        }
        else if (!Grounded && !OnRail)
        {
            RB.AddForce((Vector3.down * AirTime) * gravitytimer * gravityMultiplier, ForceMode.Acceleration);
           

        }


        if (GrindAir)
        {

            Anim.SetBool("GrindAir", true);
           
           // gravityMultiplier = 70;
            RB.AddForce(MoveDirection.normalized * movespeed * 10f / GrindAirManeuverability, ForceMode.Force);

            //overtime decrease back to gravity
            RB.AddForce((Vector3.down * gravityMultiplier) * (gravitytimer * 3f), ForceMode.Acceleration);

        }
        else if (!GrindAir)
        {
            gravityMultiplier = 150;
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

    private bool OnSlope()
    {
        // Cast a ray from the center of the player straight down
        if (Physics.Raycast(CPoint, Vector3.down, out slopeHit, (playerhieght * 0.5f) + 0.3f, Ground))
        {
            angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            // Returns true if it is an actual slope and not flat ground (0 degrees)
            return angle > 0 && angle < 45f;
        }
        return false;
    }

    private void Jumplogii()
    {
        //Jump dust
        Instantiate(JumpDust, new Vector3(transform.position.x, transform.position.y - 4f, transform.position.z), Quaternion.identity);

        RB.linearVelocity = new Vector3(RB.linearVelocity.x, RB.linearVelocity.y, RB.linearVelocity.z);

        RB.AddForce(transform.up * Jumpforce + (PlayerRotAxis.transform.forward * JumpForwardforce * InputNum), ForceMode.VelocityChange);

    }

    private void resetjump()
    {

        JumpCooled = true;
        PlayerGrind.JumpCooldown = true;

    }

    private void JumpBuffer()
    {
        JumpCooled = false;
        PlayerGrind.JumpCooldown = false;
        Invoke(nameof(resetjump), 0.5f);
    }



    //UI ELEMENTS

    public void popup()
    {
      Debug.Log("thing");
       SprayPrompt.SetActive(true);
        GraffitiRange = true;
    }

    public void popupclose()
    {
        Debug.Log("thing");
        SprayPrompt.SetActive(false);
        GraffitiRange = false;
    }


    //Dust Cloud settings

    public void Createdustcloud()
    {

        Touchpoint = false;
        //land dust
        Instantiate(JumpDust, new Vector3(transform.position.x, transform.position.y - 4f, transform.position.z), Quaternion.identity);
    }





}
