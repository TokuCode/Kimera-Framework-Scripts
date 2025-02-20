using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ComboGraph : EditorWindow
{
    private ComboGraphView _graphView;
    private string _fileName;
    
    [MenuItem("Graph/Combo Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<ComboGraph>();
        window.titleContent = new GUIContent("Combo Graph");
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
        GenerateMiniMap();
    }

    private void ConstructGraph()
    {
        _graphView = new ComboGraphView
        {
            name = "Combo Graph"
        };
        
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap{ anchored = true};
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        _graphView.Add(miniMap);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();
        
        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify("New Combo");
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => { _fileName = evt.newValue; });
        toolbar.Add(fileNameTextField);
        
        toolbar.Add(new Button(() => { RequestDataOperation(true); }) { text = "Save Data" });
        toolbar.Add(new Button(() => { RequestDataOperation(false); }) { text = "Load Data" });
        
        var nodeCreateButton = new Button(() => { _graphView.CreateNode("Combo Node"); });
        nodeCreateButton.text = "Create Combo";
        toolbar.Add(nodeCreateButton);
        
        var attackLibraryField = new ObjectField();
        attackLibraryField.objectType = typeof(AttackLibrary);
        attackLibraryField.RegisterValueChangedCallback(evt =>
        {
            var attackLibrary = evt.newValue as AttackLibrary;
            if (attackLibrary == null) return;
            _graphView.attackLibrary = attackLibrary;
        });
        
        toolbar.Add(attackLibraryField);
        toolbar.Add(new Button(() => { _graphView.BuildCombos(); }) { text = "Build Combos" });
        
        rootVisualElement.Add(toolbar);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
            return;
        }
        
        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if(save)
            saveUtility.SaveGraph(_fileName);
        else 
            saveUtility.LoadGraph(_fileName);
    }
}
