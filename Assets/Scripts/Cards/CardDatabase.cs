using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Cards/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    [SerializeField] private List<CardData> _allCards = new List<CardData>();
    public IReadOnlyList<CardData> AllCards => _allCards;

    public void InitializeEmptyList()
    {
        if (_allCards == null)
            _allCards = new List<CardData>();
    }

    public CardData GetByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return _allCards.Find(c => c != null && string.Equals(c.CardName, name, StringComparison.Ordinal));
    }

    public CardData GetRandomCard(Predicate<CardData> predicate)
    {
        List<CardData> pool = (predicate == null)
            ? _allCards.FindAll(c => c != null)
            : _allCards.FindAll(c => c != null && predicate(c));

        if (pool.Count == 0) return null;
        return pool[UnityEngine.Random.Range(0, pool.Count)];
    }
}
