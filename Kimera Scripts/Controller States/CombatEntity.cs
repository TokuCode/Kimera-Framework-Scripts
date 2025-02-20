using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CombatEntity
{
    public bool block { get; set; }
    public bool parry { get; set; }
    public int attack { get; set; }
    public int comboCount { get; set; }
}
