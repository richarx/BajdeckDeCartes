using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardDatabase))]
public class CardDatabaseEditor : Editor
{
    private CardDatabase db;
    private Vector2 scrollPos;
    private string filter = "All";
    private List<bool> foldouts = new List<bool>();

    private SerializedProperty allCardsProp;

    private void OnEnable()
    {
        db = (CardDatabase)target;

        serializedObject.Update();
        allCardsProp = serializedObject.FindProperty("_allCards");
        SyncFoldouts();
    }

    private void SyncFoldouts()
    {
        if (db.AllCards == null)
        {
            db.InitializeEmptyList();

            serializedObject.Update();
            allCardsProp = serializedObject.FindProperty("_allCards");
        }

        while (foldouts.Count < allCardsProp.arraySize)
            foldouts.Add(false);

        if (foldouts.Count > allCardsProp.arraySize)
            foldouts.RemoveRange(allCardsProp.arraySize, foldouts.Count - allCardsProp.arraySize);
    }

    public override void OnInspectorGUI()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || Selection.activeObject != target) return;

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Database", EditorStyles.boldLabel);

        if (GUILayout.Button("Rebuild Database"))
        {
            CardDatabaseBuilder.BuildDatabase();
            return;
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filter:", GUILayout.Width(40));
        if (GUILayout.Toggle(filter == "All", "All", EditorStyles.miniButtonLeft)) filter = "All";
        if (GUILayout.Toggle(filter == "Player", "Player", EditorStyles.miniButtonMid)) filter = "Player";
        if (GUILayout.Toggle(filter == "Enemy", "Enemy", EditorStyles.miniButtonMid)) filter = "Enemy";
        if (GUILayout.Toggle(filter == "Incomplete", "Incomplete", EditorStyles.miniButtonRight)) filter = "Incomplete";
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        SyncFoldouts();

        for (int i = 0; i < allCardsProp.arraySize; i++)
        {
            SerializedProperty cardProp = allCardsProp.GetArrayElementAtIndex(i);
            if (cardProp == null || cardProp.objectReferenceValue == null) continue;

            CardData card = (CardData)cardProp.objectReferenceValue;
            if (card == null) continue;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorGUILayout.LabelField("Loading...");
                continue;
            }

            SerializedObject cardSO = new SerializedObject(card);
            SerializedProperty propCardName = cardSO.FindProperty("_cardName");
            SerializedProperty propDescription = cardSO.FindProperty("_description");
            SerializedProperty propArtwork = cardSO.FindProperty("_artwork");
            //SerializedProperty propAvailableForPlayer = cardSO.FindProperty("_availableForPlayer");
            //SerializedProperty propAvailableForEnemy = cardSO.FindProperty("_availableForEnemy");
            SerializedProperty propAlternatePrefab = cardSO.FindProperty("_alternatePrefab");

            cardSO.Update();

            bool isIncomplete = string.IsNullOrEmpty(propCardName.stringValue) ||
                                string.IsNullOrEmpty(propDescription.stringValue);

            //if (filter == "Player" && !propAvailableForPlayer.boolValue) continue;
            //if (filter == "Enemy" && !propAvailableForEnemy.boolValue) continue;
            if (filter == "Incomplete" && !isIncomplete && !foldouts[i]) continue;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            // Récupération du Sprite (toujours depuis SerializedProperty, safe pour l'editor)
            Sprite sprite = (Sprite)propArtwork.objectReferenceValue;

            // Affichage dans un ObjectField pour pouvoir changer le sprite
            Sprite newSprite = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), false, GUILayout.Width(60), GUILayout.Height(60));

            // Appliquer le changement si modifié
            if (newSprite != sprite)
            {
                propArtwork.objectReferenceValue = newSprite;
            }


            // ---- Nom et warning ----
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(propCardName, GUIContent.none);
            if (string.IsNullOrEmpty(propCardName.stringValue) || string.IsNullOrEmpty(propDescription.stringValue))
            {
                GUILayout.Label("⚠️ Incomplete", GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();

            // ---- Flags, lien et foldout ----
            EditorGUILayout.BeginHorizontal();

            //propAvailableForPlayer.boolValue = EditorGUILayout.ToggleLeft("Player", propAvailableForPlayer.boolValue, GUILayout.Width(80));
            //propAvailableForEnemy.boolValue = EditorGUILayout.ToggleLeft("Enemy", propAvailableForEnemy.boolValue, GUILayout.Width(80));

            if (GUILayout.Button("🔗", GUILayout.Width(25)))
            {
                Selection.activeObject = card;
                EditorGUIUtility.PingObject(card);
            }

            GUILayout.FlexibleSpace();

            // Foldout
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], "", true);

            EditorGUILayout.EndHorizontal(); // end flags + link + foldout
            EditorGUILayout.EndVertical();   // end vertical nom + warning + flags
            EditorGUILayout.EndHorizontal(); // end horizontal header

            if (foldouts[i])
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Description:");
                propDescription.stringValue = EditorGUILayout.TextArea(propDescription.stringValue, GUILayout.Height(60));

                EditorGUILayout.PropertyField(propAlternatePrefab);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();   // end box

            EditorGUILayout.Space();

            if (cardSO.hasModifiedProperties)
            {
                Undo.RecordObject(card, "Edit CardData");
                cardSO.ApplyModifiedProperties();
                EditorUtility.SetDirty(card);
            }
        }

        EditorGUILayout.EndScrollView();

        if (serializedObject.hasModifiedProperties)
            serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(db);
        }
    }
}
