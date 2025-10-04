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
        else
        {
            db.allCards.Clear();
            Debug.Log("CardDatabase asset reset.");
        }

        db.allCards.AddRange(allCards);

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();

        Debug.Log($"Database built with {db.allCards.Count} cards");
    }
}
