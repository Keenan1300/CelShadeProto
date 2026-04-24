using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    [Header("Grind Data")]
    bool onRail;
    public float GrindSpeed;
    public float HeightOffset;
    public float TimeForFullSpline;
    public float elapsedTime;
    public float LerpSpeed = 0.5f;

    [Header("JumpLogic")]
    public float JumpForce = 5f; // Added for jump off

    [Header("Inputs")]
    public bool jump;
    Vector3 Input;

    [Header("Scripts")]
    Rigidbody PlayerRB;
    CharacterController CharCont;
    Collider CharCollision;
    RailScript CurrentRailScript;

    void Start()
    {
        PlayerRB = GetComponent<Rigidbody>();
        CharCollision = GetComponent<CapsuleCollider>();
    }

    public void InterpretJump(InputAction.CallbackContext context)
    {
        // Space button check
        jump = Convert.ToBoolean(context.ReadValue<float>());

        // If we press space while on the rail, jump off
        if (context.started && onRail)
        {
            JumpOffRail();
        }
    }

    public void InterpretMovement(InputAction.CallbackContext context)
    {
        Vector2 Rawinput = context.ReadValue<Vector2>();
        Input.x = Rawinput.x;
    }

    private void FixedUpdate()
    {
        if (onRail)
        {
            GrindplayerAlongRail();
        }
    }

    void GrindplayerAlongRail()
    {
        if (CurrentRailScript != null && onRail)
        {
            float progress = elapsedTime / TimeForFullSpline;
            bool ForwardOrient = CurrentRailScript.ForwardOrient;

            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }

            float nextTimeNormalized;
            if (ForwardOrient)
            {
                nextTimeNormalized = (elapsedTime + Time.deltaTime) / TimeForFullSpline;
            }
            else
            {
               
                nextTimeNormalized = (elapsedTime - Time.deltaTime) / TimeForFullSpline;
            }

            float3 pos, tangent, up;
            float3 nextposfloat, nextTan, nextUp;

            SplineUtility.Evaluate(CurrentRailScript.RailSp.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(CurrentRailScript.RailSp.Spline, nextTimeNormalized, out nextposfloat, out nextTan, out nextUp);

            Vector3 worldPos = CurrentRailScript.ConvertLocaltoWorld(pos);
            Vector3 nextPos = CurrentRailScript.ConvertLocaltoWorld(nextposfloat);

            //update position
            transform.position = worldPos + (transform.up * HeightOffset);

            //update rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos - worldPos), LerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.up, up) * transform.rotation, LerpSpeed);

            if (ForwardOrient)
            {
                elapsedTime += Time.deltaTime;
            }
            else
            {
                elapsedTime -= Time.deltaTime;
            }

            if (elapsedTime > TimeForFullSpline || elapsedTime < TimeForFullSpline * -1f)
            {
                JumpOffRail();

            }
          
        }
    }

    private void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.tag == "Rail")
        {
            onRail = true;
            CurrentRailScript = hit.gameObject.GetComponent<RailScript>();

            
            //PlayerRB.isKinematic = true;

            SetRailPositionFromEntry();
        }
    }

    void SetRailPositionFromEntry()
    {
        TimeForFullSpline = CurrentRailScript.RailLength / GrindSpeed;

        Vector3 SplinePoint;
        float normalizedTime = CurrentRailScript.CalculateTargetRailPoint(transform.position, out SplinePoint);
        elapsedTime = TimeForFullSpline * normalizedTime;

        float3 pos, forward, up;
        SplineUtility.Evaluate(CurrentRailScript.RailSp.Spline, normalizedTime, out pos, out forward, out up);

        CurrentRailScript.CalcDirection(forward, transform.forward);

        
        transform.position = SplinePoint + (transform.up * HeightOffset);
    }

    void JumpOffRail()
    {
        ThrowOffRail();
    }

    public void ThrowOffRail()
    {
        PlayerRB.AddForce(transform.forward * GrindSpeed * 10f, ForceMode.Force);
        onRail = false;
        CurrentRailScript = null;

        //PlayerRB.isKinematic = false;

        transform.position += transform.forward * 10;
    }
}