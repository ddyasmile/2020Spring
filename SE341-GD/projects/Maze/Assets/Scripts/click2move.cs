using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click2move : MonoBehaviour
{
    public float speed;
    public GameObject player;
    public CharacterController cc;

    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        cc = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            movePlayer();
        }
    }

    void movePlayer()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        Vector3 velocity = direction * speed;
        velocity = Camera.main.transform.TransformDirection(velocity);
        cc.Move(velocity * Time.deltaTime);
    }

    public void startMoving()
    {
        active = true;
    }

    public void stopMoving()
    {
        active = false;
    }
}
