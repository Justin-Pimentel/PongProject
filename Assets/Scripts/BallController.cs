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

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, newVelocity, rayDist);

        if (hits.Length > 0)
        {
            processCollision(hits[0]);
        }
    }

    void processCollision(RaycastHit2D firstHit)
    {
        Debug.Log(firstHit.normal);
        string pName = firstHit.collider.gameObject.name;
        Vector2 pt = firstHit.point;
        float d = 0.0f;

        //Vectors to calculate reflection angle off of paddles and walls
        newVelocity = Vector2.ClampMagnitude(Vector2.Reflect(newVelocity, firstHit.normal), maxVectorSpeed);

        /*
         * If a paddle was hit, find the difference from contact point to paddle center
         * We will use this to offset the angle of reflection so the farther from center
         * the more extreme the angle of reflection
         */
        if (tag == "Paddle")
        {
            d = findDiff(pName, pt);
            //Debug.Log(d);

            //Use newVelocity to modify reflect angle
            newVelocity = Vector2.ClampMagnitude(newVelocity + new Vector2(0, newVelocity.y + d), maxVectorSpeed);
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

        return diff;
    }
}
