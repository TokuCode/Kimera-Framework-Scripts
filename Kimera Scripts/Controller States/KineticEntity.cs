using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface KineticEntity
{
    public Vector3 speed {  get; set; }
    public float currentSpeed { get; set; }
    public float maxSpeed { get; set; }
}
