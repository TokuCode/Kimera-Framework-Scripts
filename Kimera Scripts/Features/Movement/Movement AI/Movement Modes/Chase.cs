using Features;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MovementMode
{
    //Properties
    [Header("Properties Chase")]
    public float chaseDistance;
    [HideInInspector] public float chaseSpeed;

    public override void SetupFeature(Controller controller)
    {
        base.SetupFeature(controller);

        chaseDistance = settings.Search("chaseDistance");
        chaseSpeed = settings.Search("chaseSpeed");

        modeName = "chase";
        modeSpeed = chaseSpeed;
    }

    public override Vector3 RequestNextPoint(FollowEntity follow)
    {
        if(follow == null) return transform.position;

        if(follow.target == null) return transform.position;

        float distance = Vector3.Distance(transform.position, follow.target.transform.position);

        Vector3 interpolation = Vector3.Lerp(transform.position, follow.target.transform.position, Mathf.Clamp01(chaseDistance/distance));
        
        return ToFloor(interpolation);
    }
}
