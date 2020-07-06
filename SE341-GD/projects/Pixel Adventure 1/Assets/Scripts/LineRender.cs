using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRender : MonoBehaviour
{
    LineRenderer line;
    public Transform startPoint;
    public Transform endPoint;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, new Vector3(startPoint.position.x, startPoint.position.y, 0.5f));
        line.SetPosition(1, new Vector3(endPoint.position.x, endPoint.position.y, 0.5f));
    }
}
