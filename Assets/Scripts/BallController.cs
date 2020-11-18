using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float modAngle;
    public float initialSpeed;
    public float maxVectorSpeed;
    public float rayDist;
    private GameObject lP;
    private GameObject rP;
    private GameObject parentWell;
    private Dictionary<string, string> wellRotations;
    private Vector2 newVelocity;
    private Vector2[] points = new Vector2[3]; 
    private bool inWell = false;

    // Start is called before the first frame update
    void Start()
    {
        lP = GameObject.Find("LeftPaddle");
        rP = GameObject.Find("RightPaddle");
        parentWell = GameObject.Find("Gravity Wells");
        wellRotations = new Dictionary<string, string>();

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
        transform.Translate(newVelocity * initialSpeed * Time.deltaTime);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, newVelocity, rayDist);

        //Only process the first hit detected by the raycast
        if (hit.collider != null)
        {
            processCollision(hit);
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

        if (tag == "GWell" && !inWell)
        {
            //Gather information about the collider and its position
            inWell = true;
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

            Debug.Log("Rotation: " + wellRotations[pName] + " Deg: " + deg);

        } else if (tag == "Paddle")
        {
            //Vectors to calculate reflection angle off of paddles and walls
            newVelocity = Vector2.ClampMagnitude(Vector2.Reflect(newVelocity, firstHit.normal), maxVectorSpeed);

            d = findDiff(pName, pt);

            Debug.Log(d);

            //TODO: Find a way to modify the y value of the newVelocity vector to keep it from too extreme an angle

            Vector2 modVec = Vector2.ClampMagnitude(newVelocity + new Vector2(0, newVelocity.y + d), maxVectorSpeed);
            Vector2 refVec = new Vector2(newVelocity.x, 0);

            float angle = Vector2.Angle(refVec, modVec);
            Debug.Log(angle);

            newVelocity = modVec;

            //Use newVelocity to modify reflect angle
            //newVelocity = Vector2.ClampMagnitude(newVelocity + new Vector2(0, newVelocity.y + d), maxVectorSpeed);
        }
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

        return diff % 0.25f;
    }

    Vector2 findPointOnCircle(float angle, float radius, Vector2 origin)
    { 
        float x = origin.x + radius * Mathf.Cos(angle);
        float y = origin.y + radius * Mathf.Sin(angle);

        Vector2 p = new Vector2(x, y);

        return p;
    }

    void printRotations(Dictionary<string, string> wellRotations)
    {
        foreach (KeyValuePair<string, string> well in wellRotations)
        {
            Debug.Log("Key: " + well.Key + " Value: " + well.Value);
        }
    }
}
