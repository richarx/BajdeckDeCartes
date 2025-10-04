using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardDatabaseBuilder
{
    private const string databasePath = "Assets/Data/CardDatabase.asset";

    [MenuItem("Tools/Build Card Database")]
    public static void BuildDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("t:CardData", new[] { "Assets/Data/Cards" });
        List<CardData> allCards = new List<CardData>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CardData card = AssetDatabase.LoadAssetAtPath<CardData>(path);
            if (card != null)
                allCards.Add(card);
        }

        CardDatabase db = AssetDatabase.LoadAssetAtPath<CardDatabase>(databasePath);

        if (db == null)
        {
            db = ScriptableObject.CreateInstance<CardDatabase>();
            AssetDatabase.CreateAsset(db, databasePath);
            Debug.Log("CardDatabase asset created.");
        }

        SerializedObject dbSO = new SerializedObject(db);
        SerializedProperty allCardsProp = dbSO.FindProperty("_allCards");

        allCardsProp.ClearArray();

        for (int i = 0; i < allCards.Count; i++)
        {
            allCardsProp.InsertArrayElementAtIndex(i);
            allCardsProp.GetArrayElementAtIndex(i).objectReferenceValue = allCards[i];
        }

        dbSO.ApplyModifiedProperties();

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Database built with {allCards.Count} cards");
    }
}
