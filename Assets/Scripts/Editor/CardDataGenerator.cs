using UnityEditor;
using UnityEngine;

public class CardDataGenerator
{
    [MenuItem("Tools/Generate Cards From Sprites")]
    public static void GenerateCards()
    {
        string cardsPath = "Assets/Data/Cards";   // Où créer les SO
        if (!System.IO.Directory.Exists(cardsPath))
            System.IO.Directory.CreateDirectory(cardsPath);

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art/Sprites/Cards" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Récupère tous les sprites dans la texture (y compris les sub-assets)
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

            foreach (var obj in assets)
            {
                if (obj is Sprite sprite)
                {
                    string assetPath = $"Assets/Data/Cards/{sprite.name}.asset";

                    var card = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);
                    if (card == null)
                    {
                        card = ScriptableObject.CreateInstance<CardData>();
                        card.cardName = sprite.name;
                        card.artwork = sprite;

                        AssetDatabase.CreateAsset(card, assetPath);
                        Debug.Log($"Created card: {sprite.name}");
                    }
                }
            }
        }


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
