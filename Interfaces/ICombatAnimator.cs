using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatAnimator
{
    public bool CheckCondition(string interruptionName);
}
