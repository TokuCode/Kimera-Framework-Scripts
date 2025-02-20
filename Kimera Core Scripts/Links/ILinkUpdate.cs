using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILinkUpdate
{
    public void RequestActorUpdate(Controller actor);
    public void RequestReactorUpdate(Controller reactor);
}
