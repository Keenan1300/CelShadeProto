using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RailScript : MonoBehaviour
{
    [Header("Rail Structure")]
    public Transform PointA;
    public Transform PointB;

    [Header("Rail Data")]
    public bool ForwardOrient;
    public SplineContainer RailSp;
    public float RailLength;

    void Start()
    {
        RailSp = GetComponent<SplineContainer>();
        UpdateRailPoints();
    }

    // Correctly convert local spline point to world position
    public Vector3 ConvertLocaltoWorld(float3 localPoint)
    {
        return transform.TransformPoint(localPoint);
    }

    // Correctly convert world position to local spline space
    public float3 ConvertWorldtoLocal(Vector3 worldPoint)
    {
        return transform.InverseTransformPoint(worldPoint);
    }

    // Convert directions (tangents/up vectors) without translation
    public Vector3 ConvertLocaltoWorldDirection(float3 localDir)
    {
        return transform.TransformDirection(localDir);
    }

    public float CalculateTargetRailPoint(Vector3 playerPos, out Vector3 worldPosOnSpline)
    {
        // Must convert player world pos to local rail space for SplineUtility
        float3 localPlayerPos = ConvertWorldtoLocal(playerPos);

        SplineUtility.GetNearestPoint(RailSp.Spline, localPlayerPos, out float3 nearestLocalPoint, out float t);

        worldPosOnSpline = ConvertLocaltoWorld(nearestLocalPoint);
        return t; // Returns 0.0 to 1.0
    }

    public void CalcDirection(Vector3 worldRailForward, Vector3 playerForward)
    {
        // Using Dot product is slightly more performant for "is it facing same way?"
        float dot = Vector3.Dot(worldRailForward.normalized, playerForward.normalized);

        // If dot is positive, we are facing the same way. If negative, opposite.
        ForwardOrient = dot > 0;
    }

    void Update()
    {
        // Only update if points move in the editor/game
        if (PointA != null && PointB != null)
        {
            UpdateRailPoints();
        }
    }

    void UpdateRailPoints()
    {
        //Spline spline = RailSp.Spline;

        // Set Knot 0 to Point A
        //BezierKnot startKnot = spline[0];
        //startKnot.Position = transform.InverseTransformPoint(PointA.position);
        //spline[0] = startKnot;

        // Set Last Knot to Point B
        //BezierKnot endKnot = spline[spline.Count - 1];
        //endKnot.Position = transform.InverseTransformPoint(PointB.position);
        //spline[spline.Count - 1] = endKnot;

        // Recalculate length so PlayerGrind speed stays consistent
        RailLength = RailSp.CalculateLength();
    }
}

