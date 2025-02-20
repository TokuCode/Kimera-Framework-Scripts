using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFeatureAction
{
    public void FeatureAction(Controller controller, params Setting[] settings);
}
