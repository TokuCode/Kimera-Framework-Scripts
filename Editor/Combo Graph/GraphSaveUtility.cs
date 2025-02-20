using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private ComboGraphView _targetGraphView;
    private ComboContainer _cachedContainer;
    
    private List<Edge> edges => _targetGraphView.edges.ToList();
    private List<ComboNode> nodes => _targetGraphView.nodes.ToList().Cast<ComboNode>().ToList();
    
    public static GraphSaveUtility GetInstance(ComboGraphView targetGraphView)
    {
        return new GraphSaveUtility()
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        if(!edges.Any())return;
        
        var comboContainer = ScriptableObject.CreateInstance<ComboContainer>();
        
        var connectedPorts = edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as ComboNode;
            var inputNode = connectedPorts[i].input.node as ComboNode;
            
            var conditionCombo = outputNode.interruptions.First(x => x.GUID == connectedPorts[i].output.portName);
            string condition = conditionCombo.condition;
            
            comboContainer.nodeLinkData.Add(new NodeLinkData()
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID,
                condition = condition,
                comboKey = conditionCombo.comboKey
            });
        }

        foreach (var comboNode in nodes)
        {
            comboContainer.comboNodeData.Add(new ComboNodeData()
            {
                NodeGUID = comboNode.GUID,
                position = comboNode.GetPosition().position,
                comboKey = comboNode.comboKey,
                condition = comboNode.condition,
                attackKeys = comboNode.attackKeys,
                interruptions = comboNode.interruptions
            });
        }
        
        if(!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        
        AssetDatabase.CreateAsset(comboContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        _cachedContainer = Resources.Load<ComboContainer>(fileName);

        if (_cachedContainer == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target graph file does not exist!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }
    
    private void ClearGraph()
    {
        foreach (var comboNode in nodes)
        {
            edges.Where(x => x.input.node == comboNode).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
            _targetGraphView.RemoveElement(comboNode);
        }
    }
    
    private void CreateNodes()
    {
        foreach (var nodeData in _cachedContainer.comboNodeData)
        {
            var tempNode = _targetGraphView.CreateComboNode("New Combo");
            tempNode.GUID = nodeData.NodeGUID;
            tempNode.comboKey = nodeData.comboKey;
            tempNode.mainContainer.Q<TextField>("Combo Key").value = nodeData.comboKey;
            tempNode.condition = nodeData.condition;
            tempNode.mainContainer.Q<TextField>("Condition").value = nodeData.condition;
            tempNode.Q<Label>("title-label").text = nodeData.condition;
            tempNode.attackKeys = nodeData.attackKeys;
            tempNode.mainContainer.Q<TextField>("Attack Keys").value = nodeData.attackKeys;
            _targetGraphView.AddElement(tempNode);
            
            var nodePorts = _cachedContainer.nodeLinkData.Where(x => x.BaseNodeGuid == nodeData.NodeGUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddInterruptionPort(tempNode, x.condition, x.PortName, x.comboKey));
        }
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var connections = _cachedContainer.nodeLinkData.Where(x => x.BaseNodeGuid == nodes[i].GUID).ToList();
            
            for(int j = 0; j < connections.Count; j++)
            {
                string targerNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = nodes.First(x => x.GUID == targerNodeGuid);
                
                LinkNodes(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                
                targetNode.SetPosition(new Rect(_cachedContainer.comboNodeData.First(x => x.NodeGUID == targerNodeGuid).position, _targetGraphView.defaultNodeSize));
            }
        }
    }
    
    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge()
        {
            output = output,
            input = input
        };
            
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }
}
