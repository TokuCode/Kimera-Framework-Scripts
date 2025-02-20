using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Features;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

public class ComboGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(300, 400);

    public AttackLibrary attackLibrary;
    
    public ComboGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("ComboGraph"));
        
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0,grid);
        grid.StretchToParentSize();

        graphViewChanged += OnGraphViewChanged;
    }

    public void OnDisable()
    {
        graphViewChanged -= OnGraphViewChanged;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }
    
    private Port GeneratePort(ComboNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(string));
    }
    
    public void CreateNode(string nodeName)
    {
        AddElement(CreateComboNode(nodeName));
    }

    public ComboNode CreateComboNode(string nodeName)
    {
        var comboNode = new ComboNode
        {
            title = nodeName,
            GUID = Guid.NewGuid().ToString(),
            attackKeys = "",
            interruptions = new List<ComboInterruption>()
        };
        
        var inputPort = GeneratePort(comboNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        comboNode.inputContainer.Add(inputPort);
        
        GenerateContent(comboNode);
        
        var button = new Button(() => { AddInterruptionPort(comboNode);});
        button.text = "New Interruption";
        comboNode.titleContainer.Add(button);
        
        comboNode.RefreshExpandedState();
        comboNode.RefreshPorts();
        
        comboNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return comboNode;
    }

    private void GenerateContent(ComboNode node)
    {
        var comboLabel = new Label("Combo");
        var comboField = new TextField()
        {
            name = "Combo Key",
            value = node.comboKey
        };
        comboField.RegisterValueChangedCallback(evt =>
        {
            node.comboKey = evt.newValue;
        });
        var conditionLabel = new Label("Condition");
        var conditionField = new TextField()
        {
            name = "Condition",
            value = node.condition
        };
        conditionField.RegisterValueChangedCallback(evt =>
        {
            node.Q<Label>("title-label").text = evt.newValue;
            node.condition = evt.newValue;
        });
        var attackLabel = new Label("Attack Chain");
        var attackKeys = new TextField()
        {
            name = "Attack Keys",
            value = node.attackKeys
        };
        attackKeys.RegisterValueChangedCallback(evt =>
        {
            node.attackKeys = evt.newValue;
        });
        node.mainContainer.Add(comboLabel);
        node.mainContainer.Add(comboField);
        node.mainContainer.Add(conditionLabel);
        node.mainContainer.Add(conditionField);
        node.mainContainer.Add(attackLabel);
        node.mainContainer.Add(attackKeys);
    }
    
    public void AddInterruptionPort(ComboNode comboNode, string condition = "", string overridePortName = "", string overrideComboKey = "")
    {
        var generatedPort = GeneratePort(comboNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        
        var outputPortCount = comboNode.outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Interruption {outputPortCount}";

        var choiceCondition =
            string.IsNullOrEmpty(condition) ? $"Choice {outputPortCount + 1}" : condition;
        
        var portGUID = string.IsNullOrEmpty(overridePortName) ? Guid.NewGuid().ToString(): overridePortName;
        
        var comboKey = string.IsNullOrEmpty(overrideComboKey) ? "": overrideComboKey;
        
        var textField = new TextField
        {
            name = string.Empty,
            value = choiceCondition
        };
        
        comboNode.interruptions.Add(new ComboInterruption
        {
            GUID = portGUID,
            comboKey = comboKey,
            condition = choiceCondition,
        });
        
        generatedPort.portName = portGUID;
        
        textField.RegisterValueChangedCallback(evt =>
        {
            var interruption = comboNode.interruptions.Where(x => x.GUID == portGUID).ToList().Cast<ComboInterruption>()
                .ToList();
            
            if (!interruption.Any()) return;
            
            var interruptionData = interruption.First();
            interruptionData.condition = evt.newValue;
        });
        
        var deleteButton = new Button(() => RemovePort(comboNode, generatedPort))
        {
            text = "X"
        };
        
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        generatedPort.contentContainer.Add(deleteButton);
        
        comboNode.outputContainer.Add(generatedPort);
        comboNode.RefreshPorts();
        comboNode.RefreshExpandedState();
    }

    public void RemovePort(ComboNode node, Port port)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == port.portName && x.output.node == port.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());   
        }

        var interruptions = node.interruptions.Where(x => x.GUID == port.portName).ToList().Cast<ComboInterruption>()
            .ToList();

        if (interruptions.Any())
        {
            node.interruptions.Remove(interruptions.First());
        }
        
        node.outputContainer.Remove(port);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.edgesToCreate != null)
        {
            foreach (var edgeCreated in graphViewChange.edgesToCreate)
            {
                var nextComboNode = edgeCreated.input.node as ComboNode;
                var comboNode = edgeCreated.output.node as ComboNode;
                ComboInterruption comboInterruption = comboNode.interruptions.Find(x => x.GUID == edgeCreated.output.portName);
            
                if(comboInterruption == null) continue;

                comboInterruption.comboKey = nextComboNode.comboKey;
            }
        }
        
        if(graphViewChange.elementsToRemove == null) return graphViewChange;

        foreach (var element in graphViewChange.elementsToRemove)
        {
            if (element is Edge edge)
            {
                var inputNode = edge.input.node as ComboNode;
                var outputNode = edge.output.node as ComboNode;
                ComboInterruption comboInterruption = outputNode.interruptions.Find(x => x.GUID == edge.output.portName);
                
                if(comboInterruption == null) continue;

                comboInterruption.comboKey = "";
            }
        }

        return graphViewChange;
    }

    public void BuildCombos()
    {
        if(attackLibrary == null || attackLibrary.IsDestroyed()) return;
        
        foreach (ComboNode node in nodes)
        {
            var targetCombo = attackLibrary.comboFiles.Where(x => x.key == node.comboKey);
            
            if(!targetCombo.Any()) continue;
            
            ComboPreset combo = targetCombo.First().combo;

            combo.condition = node.condition;
            
            var attackKeys = node.attackKeys.Split(',');

            var attacks = new List<Features.AttackPreset>();
            
            foreach (var attackKey in attackKeys)
            {
                var targetAttack = attackLibrary.attackFiles.Where(x => x.key == attackKey);
                
                if(!targetAttack.Any()) continue;
                
                attacks.Add(targetAttack.First().attack);
            }

            combo.attackChain = attacks.ToArray();
            
            var interruptions = new List<ComboPreset.Interruption>();

            foreach (var nodeInterruption in node.interruptions)
            {
                var targetInterruption = attackLibrary.comboFiles.Where(x => x.key == nodeInterruption.comboKey);
                
                if(!targetInterruption.Any()) continue;
                
                interruptions.Add(new ComboPreset.Interruption
                {
                    condition = nodeInterruption.condition,
                    nextCombo = targetInterruption.First().combo
                });
            }
            
            combo.interruptions = interruptions.ToArray();
        }
    }
}
