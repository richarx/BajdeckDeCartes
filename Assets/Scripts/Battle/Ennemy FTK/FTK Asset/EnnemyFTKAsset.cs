using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = DisplayName, menuName = "Scriptable Objects/" + DisplayName)]
public class EnnemyFTKAsset : ScriptableObject
{
    const string DisplayName = "Ennemy FTK Asset";

    [Tooltip("Ordered (first in first out).")]
    [SerializeField] List<CardData> deck = new();

    [SerializeReference] List<AEnnemyAction> actions = new();

    public IReadOnlyList<CardData> Deck => deck;

    public IReadOnlyList<AEnnemyAction> Actions => actions;
}
