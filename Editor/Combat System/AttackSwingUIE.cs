using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Features;
using UnityEditor.Rendering;

[CustomPropertyDrawer(typeof(Features.AttackSwing))]
public class AttackSwingUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new Foldout();

        var swingName = new PropertyField(property.FindPropertyRelative("swingName"), "name");

        var labelHitbox = new Label("Hitbox Settings");
        var size = new PropertyField(property.FindPropertyRelative("size"));
        var offset = new PropertyField(property.FindPropertyRelative("offset"));
        var movement = new PropertyField(property.FindPropertyRelative("movement"));

        var labelLinker = new Label("Linker Settings");
        var bodyPart = new PropertyField(property.FindPropertyRelative("bodyPart"));
        var movementLinker = new PropertyField(property.FindPropertyRelative("movementLinker"));
        var movementCurve = new PropertyField(property.FindPropertyRelative("movementCurve"));

        var labelEffects = new Label("Effects");
        var settings = new PropertyField(property.FindPropertyRelative("settings"));

        var labelTiming = new Label("Timing");
        var labelDuration = new FloatField("Duration");
        labelDuration.BindProperty(property.FindPropertyRelative("duration"));
        labelDuration.isReadOnly = true;
        var labelStart = new FloatField("Start");
        labelStart.BindProperty(property.FindPropertyRelative("start"));
        var labelEnd = new FloatField("End");
        labelEnd.BindProperty(property.FindPropertyRelative("end"));

        

        var labelVFX = new Label("VFX");
        var labelVFXattach = new Toggle("Hitbox Attacher");
        labelVFXattach.BindProperty(property.FindPropertyRelative("hitboxAttach"));
        var labelVFXNames = new TextField("VFX Names");
        labelVFXNames.BindProperty(property.FindPropertyRelative("vfxNames"));

        var swingSlider = new MinMaxSlider("Swing");
        swingSlider.lowLimit = 0;
        swingSlider.highLimit = property.FindPropertyRelative("duration").floatValue;
        swingSlider.minValue = property.FindPropertyRelative("start").floatValue;
        swingSlider.maxValue = property.FindPropertyRelative("end").floatValue;

        swingSlider.RegisterValueChangedCallback((evt) =>
        {
            if (evt.previousValue == evt.newValue || evt.previousValue == null)
                return;

            property.FindPropertyRelative("start").floatValue = evt.newValue.x;
            property.FindPropertyRelative("end").floatValue = evt.newValue.y;
            property.serializedObject.ApplyModifiedProperties();
        });

        labelStart.RegisterValueChangedCallback((evt) =>
        {
            if (evt.previousValue == evt.newValue)
                return;

            swingSlider.minValue = evt.newValue;
        });

        labelEnd.RegisterValueChangedCallback((evt) =>
        {
            if (evt.previousValue == evt.newValue)
                return;

            swingSlider.maxValue = evt.newValue;
        });


        container.Add(swingName);

        container.Add(labelHitbox);
        container.Add(size);
        container.Add(offset);
        container.Add(movement);

        container.Add(labelLinker);
        container.Add(bodyPart);
        container.Add(movementLinker);
        container.Add(movementCurve);

        container.Add(labelEffects);
        container.Add(settings);

        

        container.Add(labelVFX);
        container.Add(labelVFXattach);
        container.Add(labelVFXNames);

        container.Add(labelTiming);
        container.Add(labelDuration);
        container.Add(labelStart);
        container.Add(labelEnd);

        container.Add(swingSlider);

        return container;
    }
}
