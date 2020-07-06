using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Text countText;
    public float speed;
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        setCountText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        GetComponent<Rigidbody>().AddForce(movement * speed * Time.deltaTime);

        // if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
        // {
        //     Application.Quit();
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickUp")
        {
            other.gameObject.SetActive(false);
            count++;
            setCountText();
        }
    }

    void setCountText()
    {
        countText.text = "Count: " + count;
    }
}
