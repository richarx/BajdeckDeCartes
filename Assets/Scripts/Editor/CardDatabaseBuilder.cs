using UnityEditor;
using UnityEngine;

public class CardDatabaseBuilder
{
    [MenuItem("Tools/Build Card Database")]
    public static void BuildDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("t:CardData", new[] { "Assets/Data/Cards" });

        CardDatabase db = ScriptableObject.CreateInstance<CardDatabase>();
        db.allCards = new System.Collections.Generic.List<CardData>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CardData card = AssetDatabase.LoadAssetAtPath<CardData>(path);
            if (card != null)
                db.allCards.Add(card);
        }

        AssetDatabase.CreateAsset(db, "Assets/Data/CardDatabase.asset");
        AssetDatabase.SaveAssets();
        Debug.Log($"Database built with {db.allCards.Count} cards");
    }
}
