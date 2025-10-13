using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SortingLayerAttribute))]
public class SortingLayerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            EditorGUI.LabelField(position, label.text, "Use [SortingLayer] with int.");
            return;
        }

        string[] layerNames = GetSortingLayerNames();
        int[] layerIDs = GetSortingLayerUniqueIDs();

        int currentID = property.intValue;
        int currentIndex = System.Array.IndexOf(layerIDs, currentID);
        if (currentIndex == -1) currentIndex = 0;

        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, layerNames);
        if (newIndex >= 0 && newIndex < layerIDs.Length)
        {
            property.intValue = layerIDs[newIndex];
        }
    }

    private string[] GetSortingLayerNames()
    {
        return SortingLayer.layers.Select(l => l.name).ToArray();
    }

    private int[] GetSortingLayerUniqueIDs()
    {
        return SortingLayer.layers.Select(l => l.id).ToArray();
    }
}
