using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    private GameObject lPaddle;
    private GameObject rPaddle;
    private Rigidbody2D lPaddleBody;
    private Rigidbody2D rPaddleBody;
    public float lVelocity;
    public float rVelocity;

    // Start is called before the first frame update
    void Start()
    {
        //Find gameobjects from scene hierarchy to get an accurate object reference
        lPaddle = GameObject.Find("LeftPaddle");
        rPaddle = GameObject.Find("RightPaddle");

        //Just in case, modify paddle positions to be centered on game start
        lPaddle.transform.position = new Vector3(lPaddle.transform.position.x, 0.0f, lPaddle.transform.position.z);
        rPaddle.transform.position = new Vector3(rPaddle.transform.position.x, 0.0f, rPaddle.transform.position.z);

        //Get Rigidbody references for both paddles
        lPaddleBody = lPaddle.GetComponent<Rigidbody2D>();

        rPaddleBody = rPaddle.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            lPaddleBody.MovePosition(lPaddleBody.transform.position + transform.up * Time.fixedDeltaTime * lVelocity);
        }

        if (Input.GetKey(KeyCode.S))
        {
            lPaddleBody.MovePosition(lPaddleBody.transform.position + -(transform.up * Time.fixedDeltaTime * lVelocity));
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rPaddleBody.MovePosition(rPaddleBody.transform.position + transform.up * Time.fixedDeltaTime * rVelocity);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            rPaddleBody.MovePosition(rPaddleBody.transform.position + -(transform.up * Time.fixedDeltaTime * rVelocity));
        }
    }
}
