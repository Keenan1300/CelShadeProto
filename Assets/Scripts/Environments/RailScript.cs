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
