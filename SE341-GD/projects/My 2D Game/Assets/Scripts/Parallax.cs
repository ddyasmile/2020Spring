using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform;
    public float moveRate;
    public bool lockY;

    private float startPosX, startPosY;


    // Start is called before the first frame update
    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lockY)
        {
            transform.position = new Vector2(startPosX + cameraTransform.position.x * moveRate, startPosY);
        }
        else
        {
            transform.position = new Vector2(startPosX + cameraTransform.position.x * moveRate, startPosY + cameraTransform.position.y * moveRate);
        }
    }
}
