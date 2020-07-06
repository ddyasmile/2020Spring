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
        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;
        line.SetPosition(0, new Vector3(start.x, start.y, start.z));
        line.SetPosition(1, new Vector3(end.x, end.y, end.z));
    }
}
