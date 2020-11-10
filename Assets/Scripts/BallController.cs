using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed;
    public float maxVectorSpeed;
    public float rayDist;
    private GameObject lP;
    private GameObject rP;
    private GameObject parentWell;
    private Dictionary<string, string> wellRotations;
    private Vector2 newVelocity;
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
            inWell = true;
            Debug.Log("Hit dat well");
            float radius = firstHit.collider.GetComponent<CircleCollider2D>().radius;
            Vector2 collCenter = firstHit.collider.gameObject.transform.position;
            float deg = Mathf.Atan2(pt.y - collCenter.y, pt.x - collCenter.x) * Mathf.Rad2Deg;
            Debug.Log("deg: " + deg);

            if(wellRotations[pName] == "CW")
            {
                deg += 135;
            }else if(wellRotations[pName] == "CCW")
            {
                deg += -135;
            }

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

    void printRotations(Dictionary<string, string> wellRotations)
    {
        foreach (KeyValuePair<string, string> well in wellRotations)
        {
            Debug.Log("Key: " + well.Key + " Value: " + well.Value);
        }
    }
}
