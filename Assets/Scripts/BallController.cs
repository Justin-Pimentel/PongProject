using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float modAngle;
    public float initialSpeed;
    public float maxVectorSpeed;
    public float rayDist;
    public float duration;
    public GameObject test;
    private GameObject lP;
    private GameObject rP;
    private GameObject parentWell;
    private Dictionary<string, string> wellRotations;
    private Vector2 newVelocity;
    private bool inWell = false;
    private int RESOLUTION = 25;
    private Vector3[] curvePoints;
    private Vector2[] points = new Vector2[3]; 

    // Start is called before the first frame update
    void Start()
    {
        lP = GameObject.Find("LeftPaddle");
        rP = GameObject.Find("RightPaddle");
        parentWell = GameObject.Find("Gravity Wells");
        wellRotations = new Dictionary<string, string>();
        curvePoints = new Vector3[RESOLUTION];

        float rNum = Random.Range(-1f, 1f);

        if (rNum <= 0.0f)
        {
            newVelocity = new Vector2(-1f, 0f);
        } else if (rNum > 0.0f)
        {
            newVelocity = new Vector2(1f, 0f);
        }

        foreach(Transform child in parentWell.transform)
        {
            wellRotations.Add(child.name, child.GetComponent<TrajectoryModifier>().rotType);
        }

        //printRotations(wellRotations);
    }

    // Update is called once per frame
    void Update()
    {
        //Move the ball
        if(!inWell){
            transform.Translate(newVelocity * initialSpeed * Time.deltaTime);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, newVelocity, rayDist);

            //Only process the first hit detected by the raycast
            if (hit.collider != null)
            {
                processCollision(hit);
            }
        }else{
            float elapsed = 0f;
            //Interpolation in gravity well
            Vector3 fromPt = curvePoints[i];
            Vector3 toPT = curvePoints[i+1];
                
            transform.position = Vector3.Lerp(fromPt, toPt, elapsed/duration);
        }
    }

    void processCollision(RaycastHit2D firstHit)
    {
        string pName = firstHit.collider.gameObject.name;
        string tag = firstHit.collider.gameObject.tag;
        Vector2 pt = firstHit.point;
        float d = 0.0f;

        /*
         * If a paddle was hit, find the difference from contact point to paddle center
         * We will use this to offset the angle of reflection so the farther from center
         * the more extreme the angle of reflection
         */

        if(tag != "GWell" && tag != "Paddle"){
            newVelocity = Vector2.ClampMagnitude(Vector2.Reflect(newVelocity, firstHit.normal),                                      maxVectorSpeed);
        }else if (tag == "GWell" && !inWell)
        {
            //Gather information about the collider and its position
            points[0] = pt;
            float radius = firstHit.collider.GetComponent<CircleCollider2D>().radius;
            Vector2 collCenter = firstHit.collider.gameObject.transform.position;
            float rad = Mathf.Atan2(pt.y - collCenter.y, pt.x - collCenter.x);
            float deg = rad * Mathf.Rad2Deg;
            Debug.Log("deg: " + deg);

            //Find the second point by extending the radius and find the halfway point 
            //between the first and third point
            float tempDeg = deg;
            float tempRad = radius += radius/2;

            //Modify the angle according to the rotation of the gravity well
            if(wellRotations[pName] == "CW")
            {
                tempDeg += modAngle / 2;
                tempDeg *= Mathf.Deg2Rad;
                deg += modAngle;
            }else if(wellRotations[pName] == "CCW")
            {
                tempDeg += -modAngle / 2;
                tempDeg *= Mathf.Deg2Rad;
                deg += -modAngle;
            }

            //Assign second point
            points[1] = findPointOnCircle(tempDeg, tempRad, collCenter);

            rad = deg * Mathf.Deg2Rad;

            //Assign third point
            points[2] = findPointOnCircle(rad, radius, collCenter);
            
            for(int i = 0; i < 3; i++){
                //Instantiate(test, points[i], Quaternion.identity);
            }
            
            DrawCurve();
            for(int i = 0; i < RESOLUTION; i++){
                Debug.Log("Point " + i + ": " + curvePoints[i]);
            }
            
            Debug.Log("Rotation: " + wellRotations[pName] + " Deg: " + deg);

        } else if (tag == "Paddle")
        {
            //Vectors to calculate reflection angle off of paddles and walls
            newVelocity = Vector2.ClampMagnitude(Vector2.Reflect(newVelocity, firstHit.normal), maxVectorSpeed);

            d = findDiff(pName, pt);

            //Debug.Log(d);
            Vector2 modVec = Vector2.ClampMagnitude(newVelocity + new Vector2(0, newVelocity.y + d), maxVectorSpeed);
            Vector2 refVec = new Vector2(newVelocity.x, 0);

            float angle = Vector2.Angle(refVec, modVec);
            Debug.Log(angle);

            newVelocity = modVec;
        }
    }
    
    void DrawCurve()
    {
        for(int i = 0; i < RESOLUTION; i++)
        {
            float timestep = i/(float)RESOLUTION;
            curvePoints[i] = CalculateBezierPoint(timestep, points[0], points[1], points[2]);
        }
        
        //Set the flag for checking if in well after the bezier curve has been made
        inWell = true;
    }
    
    Vector3 CalculateBezierPoint(float t, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1-t;
        float uu = u*u;
        float tt = t*t;
        
        Vector3 point = (uu*p1)+(2*u*t*p2)+(tt*p3);
        
        return point;
    }

    float findDiff(string n, Vector2 p)
    {
        float diff = 0.0f;

        //For left paddle
        if (n == "LeftPaddle")
        {
            diff = p.y - lP.transform.position.y;
        }

        //For right paddle
        if (n == "RightPaddle")
        {
            diff = p.y - rP.transform.position.y;
        }

        //Maintains a float that is within 0.25
        return diff % 0.25f;
    }

    //Helper function to find a point on a circle given angle, radius, and origin of the circle
    Vector2 findPointOnCircle(float angle, float radius, Vector2 origin)
    { 
        float x = origin.x + radius * Mathf.Cos(angle);
        float y = origin.y + radius * Mathf.Sin(angle);

        Vector2 p = new Vector2(x, y);

        return p;
    }

    //Helper function to check which gravity well has what rotation
    void printRotations(Dictionary<string, string> wellRotations)
    {
        foreach (KeyValuePair<string, string> well in wellRotations)
        {
            Debug.Log("Key: " + well.Key + " Value: " + well.Value);
        }
    }
}
