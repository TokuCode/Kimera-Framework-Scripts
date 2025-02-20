using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Features;
using System;
using UnityEditor.Timeline;

[CustomEditor(typeof(Features.AttackPreset))]
public class AttackPresetEditor : Editor
{
    public VisualTreeAsset visualTree;
    public Features.AttackPreset attackPreset;
    Editor clipEditor;

    private void OnEnable()
    {
        attackPreset = (Features.AttackPreset)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        //Add UI Builder staff
        visualTree.CloneTree(root);

        VisualElement objectField = root.Q<ObjectField>("AnimationField");
        ObjectField animatorField = objectField as ObjectField;

        var listView = root.Q<ListView>();
        listView.makeItem = () =>
        {
            return new PropertyField();
        };
        listView.bindItem = (element, i) =>
        {
            if(i >= attackPreset.swings.Length)
            {
                serializedObject.FindProperty("swings").arraySize++;
                serializedObject.ApplyModifiedProperties();
            }

            SerializedProperty property = serializedObject.FindProperty("swings").GetArrayElementAtIndex(i);
            (element as PropertyField).BindProperty(property);
            serializedObject.ApplyModifiedProperties();
        };
        listView.unbindItem = (element, i) =>
        {
            (element as PropertyField).Unbind();
        };
        listView.itemsSource = attackPreset.swings;
        listView.itemsRemoved += (element) =>
        {
            int selected = listView.selectedIndex;

            if (listView.selectedIndex < 0)
                selected = serializedObject.FindProperty("swings").arraySize - 1;

            if (serializedObject.FindProperty("swings").arraySize < 0)
                return;

            serializedObject.FindProperty("swings").DeleteArrayElementAtIndex(listView.selectedIndex);  
            serializedObject.ApplyModifiedProperties();
        };

        var animationField = root.Q<ObjectField>("AnimationField");
        animationField.RegisterValueChangedCallback((evt) =>
        {
            if (evt.previousValue == null)
                return;

            attackPreset.animationClipHuman = (AnimationClip)evt.newValue;

            for (int i = 0; i < serializedObject.FindProperty("swings").arraySize; i++)
            {
                var property = serializedObject.FindProperty("swings").GetArrayElementAtIndex(i);
                property.FindPropertyRelative("duration").floatValue = attackPreset.animationClipHuman.length;
                property.FindPropertyRelative("start").floatValue = 0;
                property.FindPropertyRelative("end").floatValue = attackPreset.animationClipHuman.length;

            }

            var swingSliders = listView.Query<MinMaxSlider>();

            swingSliders.ToList().ForEach((slider) =>
            {
                slider.highLimit = attackPreset.animationClipHuman.length;
                slider.minValue = 0;
                slider.maxValue = attackPreset.animationClipHuman.length;
                slider.lowLimit = 0;
            });

            serializedObject.ApplyModifiedProperties();
        });

        var buttonBuild = root.Q<Button>("buildBtn");
        buttonBuild.clicked += () =>
        {
            attackPreset.BuildAnimationEvents();
        };

        return root;
    }
}
