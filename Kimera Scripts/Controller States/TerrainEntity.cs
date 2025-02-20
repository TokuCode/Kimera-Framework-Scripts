using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TerrainEntity
{
    public bool onGround { get; set; }

    public bool onSlope { get; set; }
}
