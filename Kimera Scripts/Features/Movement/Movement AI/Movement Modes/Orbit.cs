using Features;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Orbit : MovementMode
{
    //State
    [Header("States Orbit")]
    public int orbitDirection;
    //Properties
    [Header("Properties Orbit")]
    public float orbitDistance;
    [HideInInspector] public float orbitSpeed;

    public override void SetupFeature(Controller controller)
    {
        base.SetupFeature(controller);

        orbitDistance = settings.Search("orbitDistance");
        orbitSpeed = settings.Search("orbitSpeed");

        orbitDirection = Random.Range(0, 2) == 0 ? 1: -1;

        modeName = "orbit";
        modeSpeed = orbitSpeed;
    }

    public override Vector3 RequestNextPoint(FollowEntity follow)
    {
        if (follow == null) return transform.position;

        if (follow.target == null) return transform.position;

        Vector3 direction = (follow.target.transform.position - transform.position).normalized;

        Vector3 tangent = Vector3.Cross(direction, Vector3.up);

        Vector3 positionTangent = transform.position + tangent * orbitDirection * orbitDistance;

        Vector3 altPositionTangent = transform.position - tangent * orbitDirection * orbitDistance;

        if (CheckBlocked(positionTangent) && CheckBlocked(altPositionTangent))
        {
            return transform.position;
        }

        if (CheckBlocked(positionTangent))
        {
            orbitDirection *= -1;
            return ToFloor(altPositionTangent);
        }

        return ToFloor(positionTangent);
    }
}