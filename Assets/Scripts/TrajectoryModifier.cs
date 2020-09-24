using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]

public class TrajectoryModifier : MonoBehaviour
{
    public Vector3[] points;
    public LineRenderer lR;
    private int RESOLUTION = 50;
    private int layerOrder = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(!lR)
        {
            lR = GetComponent<LineRenderer>();
        }
        lR.sortingLayerID = layerOrder;
    }

    // Update is called once per frame
    void Update()
    {
        //DrawCurve();
    }
    
    //Helper function to visualize the curve
    void DrawCurve()
    {
        for(int i = 0; i < RESOLUTION; i++)
        {
            float timestep = i/(float)RESOLUTION;
            Vector3 linePoint = CalculateBezierPoint(timestep, points[0], points[1], points[2]);
        }
    }
    
    Vector3 CalculateBezierPoint(float t, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1-t;
        float uu = u*u;
        float tt = t*t;
        
        Vector3 point = (uu*p1)+(2*u*t*p2)+(tt*p3);
        
        return point;
    }
}
