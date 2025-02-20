using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILinkFixedUpdate
{
    public void RequestActorFixedUpdate(Controller actor);
    public void RequestReactorFixedUpdate(Controller reactor);
}
