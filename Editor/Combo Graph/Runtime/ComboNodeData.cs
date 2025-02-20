using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboNodeData
{
    public string NodeGUID;
    public Vector2 position;
    
    //Combo Data
    public string comboKey;
    public string condition;
    public string attackKeys;
    public List<ComboInterruption> interruptions;
    
}
