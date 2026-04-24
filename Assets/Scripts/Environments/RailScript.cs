using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.ParticleSystem;

public class RailScript : MonoBehaviour
{

    //RailStructure
    public Transform PointA;
    public Transform PointB;


    //Rail data
    public bool ForwardOrient;
    public SplineContainer RailSp;
    public float RailLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        RailSp = GetComponent<SplineContainer>();
        RailLength = RailSp.CalculateLength();
        



    }

    //Take local coordinates and convert it to world position
    public Vector3 ConvertLocaltoWorld(float3 localPoint)
    {
        Vector3 WorldPos = transform.TransformPoint(localPoint);
        return WorldPos;
    }


    //Take a spot in the world and convert it to local coordinates
    public float3 ConvertWorldtoLocal(Vector3 worldPoint)
    {
        float3 localPos = transform.TransformPoint(worldPoint);
        return localPos;

    }


    public float CalculateTargetRailPoint(Vector3 playerPos, out Vector3 worldPosOnSpline)
    {
        float3 nearestpoint;
        float time;
        SplineUtility.GetNearestPoint(RailSp.Spline, ConvertWorldtoLocal(playerPos), out nearestpoint, out time);
        worldPosOnSpline = ConvertLocaltoWorld(nearestpoint);
        return time;
    }

    
    public void CalcDirection(float3 railforward, Vector3 playerforward)
    {
        float angle = Vector3.Angle(railforward, playerforward.normalized);
        if (angle > 90f)
        {
            ForwardOrient = false;
        }
        else
        {
            ForwardOrient = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RailSp == null || PointA == null || PointB == null) return;
       
        Spline Spline = RailSp.Spline;

        //Set Position A
        BezierKnot StartPos = Spline[0];
        StartPos.Position = RailSp.transform.InverseTransformPoint(PointA.position);
        Spline[0] = StartPos;

        //Set position B
        BezierKnot EndPos = Spline[Spline.Count - 1];
        EndPos.Position = RailSp.transform.InverseTransformPoint(PointB.position);
        Spline[Spline.Count - 1] = StartPos;
    }
}
