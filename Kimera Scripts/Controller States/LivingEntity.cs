using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LivingEntity
{
    public int currentHealth { get; set; }
    public int maxHealth { get; set; }
}
