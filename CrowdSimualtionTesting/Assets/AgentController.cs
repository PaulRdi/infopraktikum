using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{

    public bool debug = false;
    int id;
    public Walker data;
    Vector3 lastPosition;
    //private void Update()
    //{
    //    transform.Translate(new Vector3(1.0f, 0f, 0f) * Time.deltaTime);
    //}

    public void Init(int id)
    {
        this.id = id;
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (!debug) return;
        int rayResolution = 30;
        float maxAngle = 30.0f;


        Vector3 direction = (transform.position - lastPosition).normalized;

        for (int j = 0; j < rayResolution; j++)
        {
            float angle = Mathf.Lerp(0, maxAngle, (float)j / rayResolution);
            float currAngleRad = Mathf.Deg2Rad * angle;
            Vector3 rayDir = new Vector3(
                direction.x * Mathf.Cos(currAngleRad) - direction.z * Mathf.Sin(currAngleRad),
                0.0f,
                direction.x * Mathf.Sin(currAngleRad) + direction.z * Mathf.Cos(currAngleRad));

            Debug.DrawRay(transform.position, rayDir * 2.0f, Color.red);

            //if (IntersectCircle(rayDir, origin, otherPosition, colliderRadius))
            //{
            //    return (int)rayResolution + j;
            //}
            if (j == 0) continue; //center ray only needs to be checked once

            currAngleRad = Mathf.Deg2Rad * -angle;
            rayDir = new  Vector3(
                direction.x * Mathf.Cos(currAngleRad) - direction.z * Mathf.Sin(currAngleRad),
                0.0f,
                direction.x * Mathf.Sin(currAngleRad) + direction.z * Mathf.Cos(currAngleRad));

            Debug.DrawRay(transform.position, rayDir * 2.0f, Color.red);

            //if (IntersectCircle(rayDir, origin, otherPosition, colliderRadius))
            //{
            //    return (int)rayResolution - j;
            //}
            lastPosition = transform.position;

        }

    }

    bool IntersectCircle(
    Vector3 d,
    Vector3 o,
    Vector3 otherPosition,
    float otherColliderRadius)
    {
        return Vector3.Cross(d, otherPosition - o).magnitude < otherColliderRadius;
    }
}
