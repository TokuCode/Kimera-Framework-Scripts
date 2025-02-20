using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ComboContainer : ScriptableObject
{
    public List<ComboNodeData> comboNodeData = new List<ComboNodeData>();
    public List<NodeLinkData> nodeLinkData = new List<NodeLinkData>();
    
}
