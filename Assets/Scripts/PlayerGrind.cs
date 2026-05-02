using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    [Header("Grind Data")]
    public UnityEvent EnterGrindingEvent;
    public UnityEvent ResetFreelookCam;
    public UnityEvent ExitGrindingEvent;
    public bool onRail;
    public float GrindSpeed = 10f;
    public float HeightOffset = 1.0f;
    public float LerpSpeed = 10f;

    private float elapsedTime;
    private float totalRailDuration; // How long it takes to finish the rail at GrindSpeed

    [Header("Scripts")]
    Rigidbody PlayerRB;
    RailScript CurrentRailScript;
    PlayerController PlayerControl;
    CapsuleCollider Colliding;

    //try not to touch this :)
    public GameObject PlayerMesh;


    public GameObject PlayerRotAxis;
    public bool ForwardOrient;


    public float EjectForce;
    public float JumpoffHeight;

    private Quaternion targetRotation;

    private Vector3 moveDir;

    void Start()
    {
        PlayerRB = GetComponent<Rigidbody>();
        Colliding = GetComponent<CapsuleCollider>();
        PlayerControl = GetComponent<PlayerController>();
    }

    public void InterpretJump(InputAction.CallbackContext context)
    {
        if (context.started && onRail)
        {
            JumpOffRail(transform.forward);
        }
    }

    private void FixedUpdate()
    {
        float HorizontalInput = Input.GetAxisRaw("Horizontal");

        if (onRail)
        {
            GrindPlayerAlongRail();

            //Jump logic
            if (HorizontalInput != 0 && Input.GetKeyDown(KeyCode.Space))
            {
                    PlayerControl.GrindAir = true;
                    PlayerControl.AirTime = PlayerControl.AirTimeGrind;
                    transform.position += transform.up * 10f;
                    JumpOffRail(PlayerRotAxis.transform.right * HorizontalInput);
                    return;
                

            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerControl.GrindAir = true;
                    PlayerControl.AirTime = PlayerControl.AirTimeGrind;
                    transform.position += transform.up * 10f;
                    JumpOffRail(PlayerRotAxis.transform.forward);
                    return;
                }
            }
            
        }
    }

    void GrindPlayerAlongRail()
    {
        

        if (CurrentRailScript == null || !onRail) return;

        // 1. Calculate Progress (0 to 1)
        float progress = elapsedTime / totalRailDuration;

        // 2. Check if we've reached the end or beginning of the rail
        if (progress < 0f || progress > 1f)
        {
            JumpOffRail(PlayerRotAxis.transform.forward);
            return;
        }


    

        // 3. Evaluate Spline Position and Rotation
        // We use the spline's local evaluation and convert to world space via the RailScript
        SplineUtility.Evaluate(CurrentRailScript.RailSp.Spline, progress, out float3 localPos, out float3 localForward, out float3 localUp);

        Vector3 worldPos = CurrentRailScript.ConvertLocaltoWorld(localPos);
        Vector3 worldUp = CurrentRailScript.ConvertLocaltoWorldDirection(localUp);
        Vector3 worldForward = CurrentRailScript.ConvertLocaltoWorldDirection(localForward);

        Vector3 targetPos = worldPos + (worldUp * HeightOffset);

        PlayerRB.MovePosition(Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * LerpSpeed));



        // 4. Update Position
        // Set position directly to the rail point + offset
        transform.position = Vector3.Lerp(transform.position, worldPos + (worldUp * HeightOffset), Time.deltaTime * LerpSpeed);

        // 5. Update Rotation
        // Face the direction of travel (ForwardOrient accounts for the 2-way logic)
        Vector3 moveDir = CurrentRailScript.ForwardOrient ? worldForward : -worldForward;
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, worldUp);
        
        
        //PlayerRotAxis.transform.rotation = targetRotation;
         PlayerRotAxis.transform.rotation = Quaternion.Slerp(PlayerRotAxis.transform.rotation, targetRotation, Time.fixedDeltaTime * LerpSpeed);

        




        //Remember:
        //Rotation should be applied to player mesh
        //Position should be on the script agent.


        // 6. Increment/Decrement Time based on direction
        if (CurrentRailScript.ForwardOrient)
            elapsedTime += Time.deltaTime;
        else
            elapsedTime -= Time.deltaTime;


       
    }

    private void OnCollisionEnter(Collision hit)
    {
       
        
            if (hit.gameObject.CompareTag("Rail") && !onRail)
            {
                CurrentRailScript = hit.transform.root.gameObject.GetComponent<RailScript>();
                if (CurrentRailScript == null) return;

                EnterRail();
              
            }

            //Boot Player off if end is reached
            if (hit.gameObject.CompareTag("RailExit") && onRail)
            {
                Debug.Log("Hit end of the line here!");
                CurrentRailScript = hit.transform.root.gameObject.GetComponent<RailScript>();
                //hit.gameObject.SetActive(false);
                //disable collider, not game object
                hit.gameObject.GetComponent<Collider>().enabled = false;
                if (CurrentRailScript == null) return;
                JumpOffRail(targetRotation.eulerAngles);

            }

        
    }

    void EnterRail()
    {
        EnterGrindingEvent.Invoke();
        GetComponent<Collider>().enabled = false;
      
        onRail = true;
        PlayerRB.isKinematic = true;


        // Calculate how long the grind lasts based on speed
        totalRailDuration = CurrentRailScript.RailLength / GrindSpeed;

        // Find where on the rail we are starting
        Vector3 splinePoint;
        float normalizedTime = CurrentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);

        //Put player aligned to rail
        Vector3 Startpos = transform.position;
        Startpos.x = splinePoint.x;
        Startpos.y = splinePoint.y + HeightOffset;
        transform.position = Startpos;

      // Set our elapsed time relative to the total duration
      elapsedTime = totalRailDuration * normalizedTime;

        // Determine if we are facing with or against the spline direction
        SplineUtility.Evaluate(CurrentRailScript.RailSp.Spline, normalizedTime, out _, out float3 localForward, out _);
        Vector3 worldForward = CurrentRailScript.ConvertLocaltoWorldDirection(localForward);

        CurrentRailScript.CalcDirection(worldForward, PlayerControl.PlayerRotAxis.transform.forward);
        PlayerRotAxis.transform.rotation = Quaternion.LookRotation(worldForward);

    }

    void JumpOffRail(Vector3 JumpDirection)
    {
        // Add an upward burst for the jump
        ExitGrindingEvent.Invoke();
        ResetFreelookCam.Invoke();

        //PlayerRB.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        ThrowOffRail(JumpDirection);

    }

    public void ThrowOffRail(Vector3 LeftRight)
    {
        onRail = false;
        PlayerRB.isKinematic = false;
        GetComponent<Collider>().enabled = true;

        transform.position += transform.up * 5f;
        //transform.position += Vector3.forward * 10f;

       

        // 1. Clear any 'spinning' forces built up during the grind
        //PlayerRB.angularVelocity = Vector3.zero;

        // 2. Straighten the player out 
        // This keeps the Y rotation (direction) but resets X and Z (tilting)
        Vector3 currentEuler = PlayerRotAxis.transform.rotation.eulerAngles;
        PlayerRotAxis.transform.rotation = Quaternion.Euler(0, currentEuler.y, 0);

        // 3.  momentum calc
        Vector3 exitDirection = CurrentRailScript.ForwardOrient ? transform.forward : -transform.forward;
        PlayerRB.linearVelocity = exitDirection * GrindSpeed;

        Vector3 ejectVector = (Vector3.up * JumpoffHeight) + (LeftRight * EjectForce);
        PlayerRB.AddForce(ejectVector, ForceMode.Impulse);

        //prevent stuck bug

        //PlayerRB.AddForce(transform.up * 40f + PlayerRotAxis.transform.forward * EjectForce, ForceMode.Impulse);
                    //Jump Vertical Calculation    Horizontal calculation\\
        //PlayerRB.AddForce(transform.up * 40f + transform.forward * EjectForce * PlayerControl.InputNum, ForceMode.Impulse);


       

        

        CurrentRailScript = null;
    }
}
