using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Setting
{
    public enum ValueType
    {
        Bool,
        Int,
        Float,
        Vector3,
        Vector3Int,
        Vector2,
        String,
        LayerMask,
        AnimationCurve,
        Color,
        Sprite,
        AudioClip,
        Prefab,
        ScriptableObject
    }

    public string key;

    public bool boolValue;
    public int intValue;
    public float floatValue;
    public Vector3 vector3Value;
    public Vector3Int vector3IntValue;
    public Vector2 vector2Value;
    public string stringValue;
    public LayerMask layerMaskValue;
    public AnimationCurve animationCurveValue;
    public Color colorValue;
    public Sprite spriteValue;
    public AudioClip audioClipValue;
    public GameObject prefabValue;
    public ScriptableObject scriptableObjectValue;

    public ValueType valueType;

    public dynamic value
    {
        get
        {
            switch (valueType)
            {
                case ValueType.Bool:
                    return boolValue;
                case ValueType.Int:
                    return intValue;
                case ValueType.Float:
                    return floatValue;
                case ValueType.Vector3:
                    return vector3Value;
                case ValueType.Vector3Int:
                    return vector3IntValue;
                case ValueType.Vector2:
                    return vector2Value;
                case ValueType.String:
                    return stringValue;
                case ValueType.LayerMask:
                    return layerMaskValue;
                case ValueType.AnimationCurve:
                    return animationCurveValue;
                case ValueType.Color:
                    return colorValue;
                case ValueType.Sprite:
                    return spriteValue;
                case ValueType.AudioClip:
                    return audioClipValue;
                case ValueType.Prefab:
                    return prefabValue;
                case ValueType.ScriptableObject:
                    return scriptableObjectValue;

                default:
                    return null;
            }
        }
    }

    public Setting(string key, dynamic value, ValueType valueType)
    {
        this.key = key;
        this.valueType = valueType;

        this.boolValue = default(bool);
        this.intValue = default(int);
        this.floatValue = default(float);
        this.vector3Value = default(Vector3);
        this.vector3IntValue = default(Vector3Int);
        this.vector2Value = default(Vector2);
        this.stringValue = default(string);
        this.layerMaskValue = default(LayerMask);
        this.animationCurveValue = default(AnimationCurve);
        this.colorValue = default(Color);
        this.spriteValue = default(Sprite);
        this.audioClipValue = default(AudioClip);
        this.prefabValue = default(GameObject);
        this.scriptableObjectValue = default(ScriptableObject);

        switch (valueType)
        {
            case ValueType.Bool:
                this.boolValue = value;
                break;
            case ValueType.Int:
                this.intValue = value;
                break;
            case ValueType.Float:
                this.floatValue = value;
                break;
            case ValueType.Vector3:
                this.vector3Value = value;
                break;
            case ValueType.Vector3Int:
                this.vector3IntValue = value;
                break;
            case ValueType.Vector2:
                this.vector2Value = value;
                break;
            case ValueType.String:
                this.stringValue = value;
                break;
            case ValueType.LayerMask:
                this.layerMaskValue = value;
                break;
            case ValueType.AnimationCurve:
                this.animationCurveValue = value;
                break;
            case ValueType.Color:
                this.colorValue = value;
                break;
            case ValueType.Sprite:
                this.spriteValue = value;
                break;
            case ValueType.AudioClip:
                this.audioClipValue = value;
                break;
            case ValueType.Prefab:
                this.prefabValue = value;
                break;
            case ValueType.ScriptableObject:
                this.scriptableObjectValue = value;
                break;
        }
    }
}
