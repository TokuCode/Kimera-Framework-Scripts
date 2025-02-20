using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR

using UnityEditor;

#endif

[CreateAssetMenu(menuName = "Entity Settings")]
public class Settings : ScriptableObject
{
    public List<Setting> settingsList;
    public Dictionary<string, Setting> settings;

    public dynamic Search(string key)
    {
        if (!settings.ContainsKey(key)) return null;

        return settings[key].value;
    }

    public void AddSetting(string key, dynamic value, Setting.ValueType valueType)
    {
        if(settingsList == null) settingsList = new List<Setting>();

        Setting setting = new Setting(key, value, valueType);

        Setting other = settingsList.Find((s => s.key == key));
        if (other != null) settingsList.Remove(other);

        settingsList.Add(setting);
    }

    public void AssemblySettings()
    {
        settings = new Dictionary<string, Setting>();
        foreach (var setting in settingsList)
        {
            settings.Add(setting.key, setting);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor
    {
        //PANEL
        string key;
        Setting.ValueType valueType;
        bool boolValue;
        int intValue;
        float floatValue;
        Vector3 vector3Value;
        Vector3Int vector3IntValue;
        Vector2 vector2Value;
        string stringValue;
        LayerMask layerMaskValue;
        AnimationCurve animationCurveValue;
        Color colorValue;
        Sprite spriteValue;
        AudioClip audioClipValue;
        GameObject prefabValue;
        ScriptableObject scriptableObjectValue;
        dynamic value;

        //READ
        public string readKey;
        Setting readSetting;
        
        //SHOW ALL
        bool showAll;

        public override void OnInspectorGUI()
        {
            Settings settings = (Settings)target;

            if (settings.settingsList == null)
            {
                settings.settingsList = new List<Setting>();
            }

            EditorGUILayout.LabelField("Create, Read, Update, Delete Settings");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", GUILayout.MaxWidth(250));
            key = EditorGUILayout.TextField(key, GUILayout.MaxWidth(200));
            EditorGUILayout.EndHorizontal();
            valueType = (Setting.ValueType)EditorGUILayout.EnumPopup("Type", valueType);

            switch (valueType)
            {
                case Setting.ValueType.Bool:
                    boolValue = EditorGUILayout.Toggle("Value", boolValue);
                    value = boolValue;
                    break;
                case Setting.ValueType.Int:
                    intValue = EditorGUILayout.IntField("Value", intValue);
                    value = intValue;
                    break;
                case Setting.ValueType.Float:
                    floatValue = EditorGUILayout.FloatField("Value", floatValue);
                    value = floatValue;
                    break;
                case Setting.ValueType.Vector3:
                    vector3Value = EditorGUILayout.Vector3Field("Value", vector3Value);
                    value = vector3Value;
                    break;
                case Setting.ValueType.Vector3Int:
                    vector3IntValue = EditorGUILayout.Vector3IntField("Value", vector3IntValue);
                    value = vector3IntValue;
                    break;
                case Setting.ValueType.Vector2:
                    vector2Value = EditorGUILayout.Vector2Field("Value", vector2Value);
                    value = vector2Value;
                    break;
                case Setting.ValueType.String:
                    stringValue = EditorGUILayout.TextField("Value", stringValue);
                    value = stringValue;
                    break;
                case Setting.ValueType.LayerMask:
                    layerMaskValue = EditorGUILayout.LayerField("Value", layerMaskValue);
                    value = layerMaskValue;
                    break;
                case Setting.ValueType.AnimationCurve:
                    if(animationCurveValue == null)
                    {
                        animationCurveValue = AnimationCurve.Linear(0,0,1,1);
                    }

                    animationCurveValue = EditorGUILayout.CurveField("Value", animationCurveValue);
                    value = animationCurveValue;
                    break;
                case Setting.ValueType.Color:
                    colorValue = EditorGUILayout.ColorField("Value", colorValue);
                    value = colorValue;
                    break;
                case Setting.ValueType.Sprite:
                    spriteValue = (Sprite)EditorGUILayout.ObjectField("Value", spriteValue, typeof(Sprite), false);
                    value = spriteValue;
                    break;
                case Setting.ValueType.AudioClip:
                    audioClipValue = (AudioClip)EditorGUILayout.ObjectField("Value", audioClipValue, typeof(AudioClip), false);
                    value = audioClipValue;
                    break;
                case Setting.ValueType.Prefab:
                    prefabValue = (GameObject)EditorGUILayout.ObjectField("Value", prefabValue, typeof(GameObject), false);
                    value = prefabValue;
                    break;
                case Setting.ValueType.ScriptableObject:
                    scriptableObjectValue = (ScriptableObject)EditorGUILayout.ObjectField("Value", scriptableObjectValue, typeof(ScriptableObject), false);
                    value = scriptableObjectValue;
                    break;
            }

            if(GUILayout.Button("Create/Update Setting"))
            {
                Setting setting = new Setting(key, value, valueType);

                Setting other = settings.settingsList.Find((s => s.key == key));

                if (other != null)
                {
                    settings.settingsList.Remove(other);
                }

                settings.settingsList.Add(setting);
                FlushPanelData();
                EditorUtility.SetDirty(target);
            }

            if(GUILayout.Button("Read Setting"))
            {
                Setting other = settings.settingsList.Find((s => s.key == key));

                if (other != null)
                {
                    readKey = key;
                    readSetting = other;
                    FlushPanelData();

                    key = readKey;
                    valueType = readSetting.valueType;
                    boolValue = readSetting.boolValue;
                    intValue = readSetting.intValue;
                    floatValue = readSetting.floatValue;
                    vector3Value = readSetting.vector3Value;
                    vector3IntValue = readSetting.vector3IntValue;
                    vector2Value = readSetting.vector2Value;
                    stringValue = readSetting.stringValue;
                    layerMaskValue = readSetting.layerMaskValue;
                    animationCurveValue = readSetting.animationCurveValue;
                    colorValue = readSetting.colorValue;
                    spriteValue = readSetting.spriteValue;
                    audioClipValue = readSetting.audioClipValue;
                    prefabValue = readSetting.prefabValue;
                    scriptableObjectValue = readSetting.scriptableObjectValue;
                } else
                {
                    readKey = "";
                    readSetting = null;
                }
            }

            if(GUILayout.Button("Delete Setting"))
            {
                Setting other = settings.settingsList.Find((s => s.key == key));

                if (other != null)
                {
                    settings.settingsList.Remove(other);
                    FlushPanelData();
                    EditorUtility.SetDirty(target);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Show All Settings");
            //Serialize

            showAll = EditorGUILayout.Toggle("Show All", showAll);
            EditorGUILayout.EndHorizontal();

            if (showAll)
            {
                EditorGUILayout.LabelField("Key  | Type | Value");

                foreach (var setting in settings.settingsList)
                {
                    EditorGUILayout.Space();
                    SerializeSetting(setting.key, setting);
                }
            }

        }

        static void SerializeSetting(string key, Setting setting)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(key, GUILayout.MaxWidth(150));
            string type = "Invalid Type";

            switch (setting.valueType)
            {
                case Setting.ValueType.Bool:
                    type = "Bool";
                    break;
                case Setting.ValueType.Int:
                    type = "Int";
                    break;
                case Setting.ValueType.Float:
                    type = "Float";
                    break;
                case Setting.ValueType.Vector3:
                    type = "Vector3";
                    break;
                case Setting.ValueType.Vector3Int:
                    type = "Vector3Int";
                    break;
                case Setting.ValueType.Vector2:
                    type = "Vector2";
                    break;
                case Setting.ValueType.String:
                    type = "String";
                    break;
                case Setting.ValueType.LayerMask:
                    type = "LayerMask";
                    break;
                case Setting.ValueType.AnimationCurve:
                    type = "AnimationCurve";
                    break;
                case Setting.ValueType.Color:
                    type = "Color";
                    break;
                case Setting.ValueType.Sprite:
                    type = "Sprite";
                    break;
                case Setting.ValueType.AudioClip:
                    type = "AudioClip";
                    break;
                case Setting.ValueType.Prefab:
                    type = "Prefab";
                    break;
                case Setting.ValueType.ScriptableObject:
                    type = "ScriptableObject";
                    break;
            }
            EditorGUILayout.LabelField(type, GUILayout.MaxWidth(50));
            string value = "Invalid Value";

            switch (setting.valueType)
            {
                case Setting.ValueType.Bool:
                    value = setting.boolValue.ToString();
                    break;
                case Setting.ValueType.Int:
                    value = setting.intValue.ToString();
                    break;
                case Setting.ValueType.Float:
                    value = setting.floatValue.ToString();
                    break;
                case Setting.ValueType.Vector3:
                    value = setting.vector3Value.ToString();
                    break;
                case Setting.ValueType.Vector3Int:
                    value = setting.vector3IntValue.ToString();
                    break;
                case Setting.ValueType.Vector2:
                    value = setting.vector2Value.ToString();
                    break;
                case Setting.ValueType.String:
                    value = setting.stringValue;
                    break;
                case Setting.ValueType.LayerMask:
                    value = setting.layerMaskValue.ToString();
                    break;
                case Setting.ValueType.AnimationCurve:
                    value = setting.animationCurveValue.ToString();
                    break;
                case Setting.ValueType.Color:
                    value = setting.colorValue.ToString();
                    break;
                case Setting.ValueType.Sprite:
                    value = setting.spriteValue == null ? setting.spriteValue.name : "Null Reference";
                    break;
                case Setting.ValueType.AudioClip:
                    value = setting.audioClipValue == null ? setting.audioClipValue.name : "Null Reference";
                    break;
                case Setting.ValueType.Prefab:
                    value = setting.prefabValue == null && !setting.prefabValue.IsDestroyed() ? setting.prefabValue.name : "Null Reference";
                    break;
                case Setting.ValueType.ScriptableObject:
                    value = !setting.scriptableObjectValue.IsDestroyed() ? setting.scriptableObjectValue.name : "Null Reference";
                    break;
            }
            EditorGUILayout.LabelField(value, GUILayout.MaxWidth(150));
            EditorGUILayout.EndHorizontal();
        }

        void FlushPanelData()
        {
            key = "";
            valueType = Setting.ValueType.Bool;
            boolValue = false;
            intValue = 0;
            floatValue = 0;
            vector3Value = Vector3.zero;
            vector3IntValue = Vector3Int.zero;
            vector2Value = Vector2.zero;
            stringValue = "";
            layerMaskValue = 0;
            animationCurveValue = null;
            colorValue = Color.white;
            spriteValue = null;
            audioClipValue = null;
            prefabValue = null;
            scriptableObjectValue = null;
        }
    }

#endif
}
