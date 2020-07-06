using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairNode
{
    public Vector3 position;
    public Vector3 lastPosition;
    public float length;
}

public class HairSimulator : MonoBehaviour
{
    public Vector3 hairRoot;
    public float damping;
    public int iterationCount;
    public int nodeNum;
    public float hairLength;
    public float hairThickness;
    public Color hairColor = Color.white;


    private List<HairNode> hairNodes = new List<HairNode>();
    private LineRenderer line;
    private Rigidbody rigidbody;
    private Vector3 acceleration;
    private Vector3 preVelocity, curVelocity;

    public const float FixedUpdateTimeStep = 0.02f;

 



    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        for (int i = 0; i < nodeNum; i++)
        {
            HairNode node = new HairNode();
            node.length = hairLength;
            hairNodes.Add(node);
        }

        GameObject lineDrawer = new GameObject();
        line = lineDrawer.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("UI/Default"));
        line.positionCount = nodeNum;
        line.startColor = hairColor;
        line.endColor = hairColor;
        line.startWidth = hairThickness;
        line.endWidth = hairThickness / 2;

    }

    // Update is called once per frame
    void Update()
    {
        foreach (var node in hairNodes)
        {
            Vector3 newPosition = Verlet(node.lastPosition, node.position, damping, acceleration, Time.deltaTime);
            node.lastPosition = node.position;
            node.position = newPosition;
        }

        for (int iter = 0; iter < iterationCount; iter++)
        {
            for (int i = 0; i < hairNodes.Count - 1; i++)
            {
                HairNode nodeA = hairNodes[i], nodeB = hairNodes[i + 1];
                collideBox(GetComponent<SphereCollider>(), ref nodeB.position);
                lengthConstraint(nodeA.position, ref nodeB.position, nodeB.length);
            }
            hairNodes[0].position = hairRoot;
        }

        for (int i = 0; i < hairNodes.Count; i++)
        {
            line.SetPosition(i, hairNodes[i].position);
        }
    }

    void FixedUpdate()
    {
        preVelocity = curVelocity;
        curVelocity = rigidbody.velocity;
        Vector3 tmp = curVelocity - preVelocity;
        acceleration = new Vector3(tmp.x / FixedUpdateTimeStep,
                                tmp.y / FixedUpdateTimeStep,
                                tmp.z / FixedUpdateTimeStep); 
    }

    private Vector3 Verlet(Vector3 lastPosition, Vector3 position, float damping, Vector3 acceleration, float deltaTime)
    {
        acceleration += new Vector3(0, -9.8f, 0);
        return position + (position - lastPosition) * damping + acceleration * deltaTime * deltaTime;
    }

    private void collideBox(SphereCollider collider, ref Vector3 position)
    {
        Vector3 colliderCenter = gameObject.transform.position + collider.center;
        Vector3 diff = colliderCenter - position;
        if (diff.magnitude < collider.radius)
        {
            position = colliderCenter + diff.normalized * collider.radius;
        }
    }

    private void lengthConstraint(Vector3 startPoint, ref Vector3 endPoint, float len)
    {
        Vector3 diff = (endPoint - startPoint).normalized * len;
        endPoint = startPoint + diff;
    }
}
