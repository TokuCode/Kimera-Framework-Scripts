using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    public static Vector3 GetStartSpeed(Vector3 direction, float height, float horizontalSpeed, float gravity)
    {
        float verticalComponent = Mathf.Sqrt(2 * gravity * height);

        Vector3 speed = direction.normalized * horizontalSpeed + Vector3.up * verticalComponent;
    
        return speed;
    }

    public static float GetFlightTime(float height, float gravity)
    {
        float time = Mathf.Sqrt(2 * height / gravity);

        return time;
    }
}
