using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AEnnemyAction), true)]
public class AEnnemyActionPropertyDrawer : PropertyDrawer
{
    // cache derived types
    static Type[] s_DerivedTypes;
    static string[] s_DerivedTypeNames;
    static GUIContent[] s_PopupContents;

    const float k_VerticalSpacing = 2f;

    static void EnsureTypes()
    {
        if (s_DerivedTypes != null) return;

        var baseType = typeof(AEnnemyAction);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
            .OrderBy(t => t.Name)
            .ToArray();

        s_DerivedTypes = types;
        s_DerivedTypeNames = new string[types.Length + 1];
        s_PopupContents = new GUIContent[types.Length + 1];

        s_DerivedTypeNames[0] = "None";
        s_PopupContents[0] = new GUIContent("None");
        for (int i = 0; i < types.Length; i++)
        {
            s_DerivedTypeNames[i + 1] = types[i].Name;
            s_PopupContents[i + 1] = new GUIContent(types[i].Name);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnsureTypes();

        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        var popupRect = new Rect(position.x + 14f, position.y, position.width - 14f, lineHeight);

        // Determine current concrete type
        Type currentType;
        try { currentType = property.managedReferenceValue != null ? property.managedReferenceValue.GetType() : null; }
        catch { currentType = null; }

        int currentIndex = 0; // 0 = None
        if (currentType != null)
        {
            for (int i = 0; i < s_DerivedTypes.Length; i++)
            {
                if (s_DerivedTypes[i] == currentType)
                {
                    currentIndex = i + 1;
                    break;
                }
            }
        }

        // Popup to select type
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(popupRect, label, currentIndex, s_PopupContents);
        if (EditorGUI.EndChangeCheck())
        {
            if (newIndex == 0)
            {
                // set to null
                property.managedReferenceValue = null;
            }
            else
            {
                Type t = s_DerivedTypes[newIndex - 1];
                object instance = Activator.CreateInstance(t);
                property.managedReferenceValue = instance;
            }
            // Apply change to serialized object
            property.serializedObject.ApplyModifiedProperties();
        }

        // If there's an instance, draw its visible properties
        if (property.managedReferenceValue != null)
        {
            // Move to children
            SerializedProperty iterator = property.Copy();
            SerializedProperty end = iterator.GetEndProperty();
            float y = position.y + lineHeight + k_VerticalSpacing;

            // Enter first child
            bool entered = iterator.NextVisible(true);
            EditorGUI.indentLevel++;
            while (entered && !SerializedProperty.EqualContents(iterator, end))
            {
                float h = EditorGUI.GetPropertyHeight(iterator, true);
                Rect r = new Rect(position.x, y, position.width, h);
                EditorGUI.PropertyField(r, iterator, true);
                y += h + k_VerticalSpacing;
                entered = iterator.NextVisible(false);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        EnsureTypes();

        float height = EditorGUIUtility.singleLineHeight;

        if (property.managedReferenceValue != null)
        {
            // sum child property heights
            SerializedProperty iterator = property.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            bool entered = iterator.NextVisible(true);
            while (entered && !SerializedProperty.EqualContents(iterator, end))
            {
                height += EditorGUI.GetPropertyHeight(iterator, true) + k_VerticalSpacing;
                entered = iterator.NextVisible(false);
            }
        }

        return height;
    }
}

