using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed;
    public float maxSpeed;
    private GameObject lP;
    private GameObject rP;
    private Vector2 newVelocity;

    // Start is called before the first frame update
    void Start()
    {
        lP = GameObject.Find("LeftPaddle");
        rP = GameObject.Find("RightPaddle");

        float rNum = Random.Range(-1f, 1f);

        if (rNum <= 0.0f)
        {
            newVelocity = new Vector2(-1f, 0f);
        }else if(rNum > 0.0f)
        {
            newVelocity = new Vector2(1f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Move the ball
        transform.Translate(newVelocity * initialSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        //Get collision info
        string pName = coll.gameObject.name;
        string tag = coll.gameObject.tag;
        ContactPoint2D p = coll.contacts[0];
        float d = 0.0f;

        //Vectors to calculate reflection angle off of paddles and walls
        Vector2 inNormal = coll.contacts[0].normal;
        newVelocity = Vector2.ClampMagnitude(Vector2.Reflect(newVelocity, inNormal), maxSpeed);
        Debug.Log("newVelocity: " + newVelocity);

        /*
         * If a paddle was hit, find the difference from contact point to paddle center
         * We will use this to offset the angle of reflection so the farther from center
         * the more extreme the angle of reflection
         */
        if (tag == "Paddle")
        {
            d = findDiff(pName, p);
            Debug.Log(d);

            //Use newVelocity to modify reflect angle
            newVelocity = Vector2.ClampMagnitude(newVelocity + new Vector2(0, newVelocity.y + d), maxSpeed);
        }

        //xVal = newVelocity.x;
        //yVal = newVelocity.y;
    }

    float findDiff(string n, ContactPoint2D p)
    {
        float diff = 0.0f;

        //For left paddle
        if (n == "LeftPaddle")
        {
            diff = p.point.y - lP.transform.position.y;
        }

        //For right paddle
        if (n == "RightPaddle")
        {
            diff = p.point.y - rP.transform.position.y;
        }

        return diff;
    }
}
