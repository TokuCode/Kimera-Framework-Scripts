using Features;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MovementMode
{
    //Properties
    [Header("Properties Escape")]
    public float escapeDistance;
    [HideInInspector] public float escapeSpeed;

    public override void SetupFeature(Controller controller)
    {
        base.SetupFeature(controller);

        escapeDistance = settings.Search("escapeDistance");
        escapeSpeed = settings.Search("escapeSpeed");

        modeName = "escape";
        modeSpeed = escapeSpeed;
    }

    public override Vector3 RequestNextPoint(FollowEntity follow)
    {
        if (follow == null) return transform.position;

        if (follow.target == null) return transform.position;

        Vector3 direction = transform.position - follow.target.transform.position;
        direction.y = transform.position.y;
        direction.Normalize();

        Vector3 newPosition = transform.position + direction * escapeDistance;

        if(CheckBlocked(newPosition)) return transform.position;

        return ToFloor(newPosition);
    }
}
