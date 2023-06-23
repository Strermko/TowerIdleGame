using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;
 
public abstract class DictionaryDrawer<TK, TV> : PropertyDrawer
{
    private SerializableDictionary<TK, TV> _dictionary;
    private bool _foldout;
    private const float ButtonWidth = 18f;
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        CheckInitialize(property, label);
        return _foldout ? (_dictionary.Count + 1) * 17f : 17f;
    }
 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CheckInitialize(property, label);
 
        position.height = 17f;
 
        var foldoutRect = position;
        foldoutRect.width -= 2 * ButtonWidth;
        EditorGUI.BeginChangeCheck();
        _foldout = EditorGUI.Foldout(foldoutRect, _foldout, label, true);
        if (EditorGUI.EndChangeCheck())
            EditorPrefs.SetBool(label.text, _foldout);
 
        var buttonRect = position;
        buttonRect.x = position.width - ButtonWidth + position.x;
        buttonRect.width = ButtonWidth + 2;
        
        if (GUI.Button(buttonRect, new GUIContent("x", "Clear dictionary"), EditorStyles.miniButton))
        {
            ClearDictionary();
        }

        buttonRect.x -= ButtonWidth;
        
        if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButtonRight))
        {
            AddNewItem();
        }
        
        if (!_foldout)
            return;
 
        foreach (var item in _dictionary)
        {
            var key = item.Key;
            var value = item.Value;
 
            position.y += 17f;
 
            var keyRect = position;
            keyRect.width /= 2;
            keyRect.width -= 4;
            EditorGUI.BeginChangeCheck();
            var newKey = DoField(keyRect, typeof(TK), key);
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    _dictionary.Remove(key);
                    _dictionary.Add(newKey, value);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }
                break;
            }
 
            var valueRect = position;
            valueRect.x = position.width / 2 + 15;
            valueRect.width = keyRect.width - ButtonWidth;
            EditorGUI.BeginChangeCheck();
            value = DoField(valueRect, typeof(TV), value);
            if (EditorGUI.EndChangeCheck())
            {
                _dictionary[key] = value;
                break;
            }
 
            var removeRect = valueRect;
            removeRect.x = valueRect.xMax + 2;
            removeRect.width = ButtonWidth;
            if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
            {
                RemoveItem(key);
                break;
            }
        }
    }
 
    private void RemoveItem(TK key)
    {
        _dictionary.Remove(key);
    }
 
    private void CheckInitialize(SerializedProperty property, GUIContent label)
    {
        if (_dictionary == null)
        {
            var target = property.serializedObject.targetObject;
            _dictionary = fieldInfo.GetValue(target) as SerializableDictionary<TK, TV>;
            if (_dictionary == null)
            {
                _dictionary = new SerializableDictionary<TK, TV>();
                fieldInfo.SetValue(target, _dictionary);
            }
 
            _foldout = EditorPrefs.GetBool(label.text);
        }
    }
 
    private static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
        new Dictionary<Type,Func<Rect,object,object>>()
        {
            { typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
            { typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
            { typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
            { typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
            { typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
            { typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
            { typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
            { typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
        };
 
    private static T DoField<T>(Rect rect, Type type, T value)
    {
        Func<Rect, object, object> field;
        if (_Fields.TryGetValue(type, out field))
            return (T)field(rect, value);
 
        if (type.IsEnum)
            return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);
 
        if (typeof(UnityObject).IsAssignableFrom(type))
            return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);
 
        Debug.Log("Type is not supported: " + type);
        return value;
    }
 
    private void ClearDictionary()
    {
        _dictionary.Clear();
    }
 
    private void AddNewItem()
    {
        TK key;
        if (typeof(TK) == typeof(string))
            key = (TK)(object)"";
        else key = default;
 
        var value = default(TV);
        try
        {
            if (key != null) _dictionary.Add(key, value);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}

[CustomPropertyDrawer(typeof(DictionaryOfCost))]
public class DictionaryOfCostsDrawer : DictionaryDrawer<ResourceType, int> { }
