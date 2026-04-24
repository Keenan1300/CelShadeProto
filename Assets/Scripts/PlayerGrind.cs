using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    [Header("Grind Data")]
    public bool onRail;
    public float GrindSpeed = 10f;
    public float HeightOffset = 1.0f;
    public float LerpSpeed = 10f;

    private float elapsedTime;
    private float totalRailDuration; // How long it takes to finish the rail at GrindSpeed

    [Header("Scripts")]
    Rigidbody PlayerRB;
    RailScript CurrentRailScript;
    CapsuleCollider Colliding;
    public GameObject PlayerMesh;

    void Start()
    {
        PlayerRB = GetComponent<Rigidbody>();
        Colliding = GetComponent<CapsuleCollider>();
    }

    public void InterpretJump(InputAction.CallbackContext context)
    {
        if (context.started && onRail)
        {
            JumpOffRail();
        }
    }

    private void FixedUpdate()
    {
        if (onRail)
        {
            GrindPlayerAlongRail();
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
            JumpOffRail();
            return;
        }

        // 3. Evaluate Spline Position and Rotation
        // We use the spline's local evaluation and convert to world space via the RailScript
        SplineUtility.Evaluate(CurrentRailScript.RailSp.Spline, progress, out float3 localPos, out float3 localForward, out float3 localUp);

        Vector3 worldPos = CurrentRailScript.ConvertLocaltoWorld(localPos);
        Vector3 worldUp = CurrentRailScript.ConvertLocaltoWorldDirection(localUp);
        Vector3 worldForward = CurrentRailScript.ConvertLocaltoWorldDirection(localForward);

        // 4. Update Position
        // Set position directly to the rail point + offset
        transform.position = Vector3.Lerp(transform.position, worldPos + (worldUp * HeightOffset), Time.deltaTime * LerpSpeed);

        // 5. Update Rotation
        // Face the direction of travel (ForwardOrient accounts for the 2-way logic)
        Vector3 moveDir = CurrentRailScript.ForwardOrient ? worldForward : -worldForward;
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, worldUp);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * LerpSpeed);

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
            CurrentRailScript = hit.gameObject.GetComponent<RailScript>();
            if (CurrentRailScript == null) return;

            EnterRail();
        }
    }

    void EnterRail()
    {
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

        CurrentRailScript.CalcDirection(worldForward, PlayerMesh.transform.forward);

    }

    void JumpOffRail()
    {
        // Add an upward burst for the jump
        ThrowOffRail();
       
        PlayerRB.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    public void ThrowOffRail()
    {
        transform.position += Vector3.forward * 10f;
        onRail = false;
       

        // 1. Clear any 'spinning' forces built up during the grind
        PlayerRB.angularVelocity = Vector3.zero;

        // 2. Straighten the player out 
        // This keeps the Y rotation (direction) but resets X and Z (tilting)
        Vector3 currentEuler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, currentEuler.y, 0);

        // 3. Maintain momentum
        PlayerRB.linearVelocity = transform.forward * GrindSpeed;

        PlayerRB.isKinematic = false;

        //prevent stuck bug
        GetComponent<Collider>().enabled = true;
        PlayerRB.AddForce(Vector3.forward * 100f, ForceMode.Force);


        CurrentRailScript = null;
    }
}
