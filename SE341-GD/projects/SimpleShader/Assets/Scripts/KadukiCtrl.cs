using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class KadukiCtrl : MonoBehaviour
{
    Animator anim;

    public GameObject playerCamera;
    public float cameHeight, cameDepth;
    public float yCorrection;

    public float speed;
    float xVelocity, yVelocity, zVelocity;

    public float mouseSensitivity;
    float lastMousex;

    Vector3 frontVec;
    Vector3 upVec;
    Vector3 leftVec;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        lastMousex = Input.mousePosition.x;

        frontVec = new Vector3(0.0f, 0.0f, 1.0f);
        upVec = new Vector3(0.0f, 1.0f, 0.0f);
        leftVec = Vector3.Normalize(Vector3.Cross(upVec, frontVec));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        Vector3 cameraOffset = (-frontVec) * cameDepth + new Vector3(0.0f, cameHeight, 0.0f);
        playerCamera.transform.position = transform.position + cameraOffset;
        playerCamera.transform.rotation = Quaternion.LookRotation(-cameraOffset + new Vector3(0.0f, yCorrection, 0.0f));

        Movement();
        AnimCtrl();
    }

    void Movement()
    {
        xVelocity = Input.GetAxisRaw("Horizontal");
        yVelocity = 0.0f;
        zVelocity = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(xVelocity * speed, yVelocity, zVelocity * speed);

        transform.Translate(moveDir * speed * Time.deltaTime);

        float mousePosx = Input.mousePosition.x;
        float mouseOffsetx = mousePosx - lastMousex;
        frontVec = Vector3.Normalize(frontVec + leftVec * mouseOffsetx * mouseSensitivity);
        transform.rotation = Quaternion.LookRotation(frontVec);

        lastMousex = mousePosx;
        leftVec = Vector3.Normalize(Vector3.Cross(upVec, frontVec));
    }

    void AnimCtrl()
    {
        if (Mathf.Abs(xVelocity) > 0 || Mathf.Abs(zVelocity) > 0)
            anim.Play("FreeVoxelGirl-walk");
        else
            anim.Play("FreeVoxelGirl-idle");
    }
}
