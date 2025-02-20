using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InputEntity
{
    public Vector2 inputDirection { get; set; }
    public Vector3 playerForward { get; set; }
    public Camera playerCamera { get; set; }
}
