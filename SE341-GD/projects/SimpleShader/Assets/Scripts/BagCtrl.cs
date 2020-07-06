using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagCtrl : MonoBehaviour
{

    public List<GameObject> items = new List<GameObject>();
    //public Dictionary<string, int> itemNum = new Dictionary<string, int>();

    public Transform playerTransform;
    public Text holdingText;

    int holding;

    private void Start()
    {
        holding = 0;
        holdingText.text = items[holding].tag;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            holding++;
            if (items.Count < holding + 1)
                holding = 0;

            holdingText.text = items[holding].tag;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Instantiate(items[holding], playerTransform.position, playerTransform.rotation);
        }
    }
}