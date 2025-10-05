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
    private HashSet<int> wrongCardNumbers = new();
    private int lowestMissingNumber = -1;

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

    private void SyncNumbers()
    {
        var orderedCards = new List<CardData>(db.AllCards);
        orderedCards.Sort((a, b) => a.Number.CompareTo(b.Number));
        lowestMissingNumber = -1;
        int expectedNumber = 1;
        wrongCardNumbers.Clear();
        foreach (var card in orderedCards)
        {
            if (card == null || card.Number <= 0) continue;
            if (card.Number < expectedNumber)
            {
                wrongCardNumbers.Add(card.Number);
            }
            else if (card.Number == expectedNumber)
            {
                expectedNumber++;
            }
            else
            {
                if (lowestMissingNumber == -1) lowestMissingNumber = expectedNumber;
                expectedNumber++;
            }
        }
        if (lowestMissingNumber == -1) lowestMissingNumber = expectedNumber;
    }

    public override void OnInspectorGUI()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || Selection.activeObject != target) return;

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Database", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Cards From Sprites"))
        {
            CardDataGenerator.GenerateCards();
            return;
        }

        if (GUILayout.Button("Rebuild Database"))
        {
            CardDatabaseBuilder.BuildDatabase();
            return;
        }

        if (GUILayout.Button("Order Cards By Number"))
        {
            db.Sort((a, b) =>
            {
                int A = a.Number;
                int B = b.Number;

                int groupA = A > 0 ? 0 : (A == 0 ? 1 : 2);
                int groupB = B > 0 ? 0 : (B == 0 ? 1 : 2);

                if (groupA != groupB)
                    return groupA.CompareTo(groupB);

                if (groupA == 0)
                    return A.CompareTo(B);
                else if (groupA == 1)   // zéros → tous égaux
                    return 0;
                else                    // négatifs → décroissant
                    return B.CompareTo(A);
            });
            foldouts.Clear();
            SyncFoldouts();
            EditorUtility.SetDirty(db);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Lowest Missing Number:", lowestMissingNumber.ToString(), EditorStyles.helpBox);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filter:", GUILayout.Width(40));
        if (GUILayout.Toggle(filter == "All", "All", EditorStyles.miniButtonLeft)) filter = "All";
        if (GUILayout.Toggle(filter == "Incomplete", "Incomplete", EditorStyles.miniButtonRight)) filter = "Incomplete";
        if (GUILayout.Toggle(filter == "WrongNumber", "Wrong Number", EditorStyles.miniButtonRight)) filter = "WrongNumber";
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        SyncFoldouts();
        SyncNumbers();

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
            SerializedProperty propRarity = cardSO.FindProperty("_rarity");
            SerializedProperty propNumber = cardSO.FindProperty("_number");
            SerializedProperty propArtwork = cardSO.FindProperty("_artwork");
            SerializedProperty propAlternatePrefab = cardSO.FindProperty("_alternatePrefab");

            cardSO.Update();

            bool isIncomplete = string.IsNullOrEmpty(propCardName.stringValue) ||
                                string.IsNullOrEmpty(propDescription.stringValue) ||
                                propNumber.intValue == 0;

            if (filter == "Incomplete" && !isIncomplete && !foldouts[i]) continue;
            if (filter == "WrongNumber" && !foldouts[i] && !wrongCardNumbers.Contains(propNumber.intValue)) continue;

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

            EditorGUILayout.PropertyField(propRarity, GUIContent.none, GUILayout.Width(80));
            // ---- Flags, lien et foldout ----
            EditorGUILayout.BeginHorizontal();

            // Petit label fixe
            GUILayout.Label("#", GUILayout.Width(12));
            // Champ numérique sans label
            EditorGUILayout.PropertyField(propNumber, GUIContent.none, GUILayout.Width(40));

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
