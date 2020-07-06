using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public GameObject basePoint;
    public GameObject penPoint;
    public float startSita;
    public float g;

    public string type;

    float sita;
    float omega;
    float l;
    Vector3 cycloid;

    float time;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 basePos = basePoint.transform.position;
        Vector3 penPos = penPoint.transform.position;
        cycloid = penPos - basePos;
        l = cycloid.magnitude;
        sita = startSita;
        omega = 0.0f;

        Vector3 newCycloid = Quaternion.Euler(0, 0, sita) * cycloid;
        penPoint.transform.SetPositionAndRotation(basePos + newCycloid, penPoint.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 basePos = basePoint.transform.position;

        time = Time.deltaTime;

        if (type == "midpoint")
        {
            float halfTime = time / 2.0f;
            float midSita = sita + omega * halfTime;
            float midOmega = omega - (g / l) * Mathf.Sin(sita * Mathf.Deg2Rad) * halfTime;

            sita = sita + midOmega * time;
            omega = omega - (g / l) * time * Mathf.Sin(midSita * Mathf.Deg2Rad);
        }
        else if (type == "trapezoid")
        {
            float nextSita = sita + omega * time;
            float nextOmega = omega - (g / l) * Mathf.Sin(sita * Mathf.Deg2Rad) * time;

            float localSita = sita;
            float localOmega = omega;

            sita = sita + (nextOmega + omega) / 2 * time;
            omega = omega - ((g / l) * Mathf.Sin(localSita * Mathf.Deg2Rad) + (g / l) * Mathf.Sin(nextSita * Mathf.Deg2Rad)) / 2 * time;
        }
        else
        {
            float localSita = sita;
            float localOmega = omega;

            sita = sita + localOmega * time;
            omega = omega - (g / l) * Mathf.Sin(localSita * Mathf.Deg2Rad) * time;
        }



        Vector3 newCycloid = Quaternion.Euler(0, 0, sita) * cycloid;
        penPoint.transform.SetPositionAndRotation(basePos + newCycloid, penPoint.transform.rotation);
    }
}
