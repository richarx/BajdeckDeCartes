using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardDatabase))]
public class CardDatabaseEditor : Editor
{
    private CardDatabase db;
    private Vector2 scrollPos;
    private string filter = "All"; // All / Player / Enemy
    private List<bool> foldouts = new List<bool>(); // Expansion des cartes

    private void OnEnable()
    {
        db = (CardDatabase)target;

        // Initialize foldouts list
        while (foldouts.Count < db.allCards.Count)
            foldouts.Add(false);
    }

    public override void OnInspectorGUI()
    {
        if (db.allCards == null)
            db.allCards = new List<CardData>();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Database", EditorStyles.boldLabel);

        // Filtre
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filter:", GUILayout.Width(40));
        if (GUILayout.Toggle(filter == "All", "All", EditorStyles.miniButtonLeft)) filter = "All";
        if (GUILayout.Toggle(filter == "Player", "Player", EditorStyles.miniButtonMid)) filter = "Player";
        if (GUILayout.Toggle(filter == "Enemy", "Enemy", EditorStyles.miniButtonRight)) filter = "Enemy";
        if (GUILayout.Toggle(filter == "Incomplete", "Incomplete", EditorStyles.miniButtonRight)) filter = "Incomplete";
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < db.allCards.Count; i++)
        {
            CardData card = db.allCards[i];
            if (card == null) continue;

            bool isIncomplete = string.IsNullOrEmpty(card.cardName) || string.IsNullOrEmpty(card.description);

            // Filtrage
            if (filter == "Player" && !card.availableForPlayer) continue;
            if (filter == "Enemy" && !card.availableForEnemy) continue;
            if (filter == "Incomplete" && !isIncomplete && !foldouts[i]) continue;

            if (i >= foldouts.Count) foldouts.Add(false);

            EditorGUILayout.BeginVertical("box");

            // Header compact
            EditorGUILayout.BeginHorizontal();

            // Sprite editable
            card.artwork = (Sprite)EditorGUILayout.ObjectField(card.artwork, typeof(Sprite), false, GUILayout.Width(60), GUILayout.Height(60));

            EditorGUILayout.BeginVertical();

            // Nom et warning
            card.cardName = EditorGUILayout.TextField(card.cardName);
            if (string.IsNullOrEmpty(card.cardName) || string.IsNullOrEmpty(card.description))
                GUILayout.Label("⚠️", GUILayout.Width(20));

            // Flags + lien + foldout
            EditorGUILayout.BeginHorizontal();

            card.availableForPlayer = EditorGUILayout.ToggleLeft("Player", card.availableForPlayer, GUILayout.Width(60));
            card.availableForEnemy = EditorGUILayout.ToggleLeft("Enemy", card.availableForEnemy, GUILayout.Width(60));


            // Bouton pour “aller” vers l’asset
            if (GUILayout.Button("🔗", GUILayout.Width(25)))
            {
                EditorGUIUtility.PingObject(card);
            }

            GUILayout.FlexibleSpace();

            // Foldout toggleGUILayout.Label("", GUILayout.Width(60)); // espace vide
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], "", true);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            // Détails dépliés
            if (foldouts[i])
            {
                EditorGUILayout.LabelField("Description:");
                card.description = EditorGUILayout.TextArea(card.description, GUILayout.Height(60));
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();

        // Sauvegarde automatique
        if (GUI.changed)
        {
            EditorUtility.SetDirty(db);
        }
    }
}
