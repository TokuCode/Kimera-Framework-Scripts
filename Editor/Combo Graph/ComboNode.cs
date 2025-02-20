using System.Collections;
using System.Collections.Generic;
using Features;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class ComboInterruption
{
    public string comboKey;
    public string condition;
    public string GUID;
}

public class ComboNode : Node
{
    //Node Data
    public string GUID;
    
    //Combo Data
    public string comboKey;
    public string condition;
    public string attackKeys;
    
    //Graph Data
    public List<ComboInterruption> interruptions;
}
