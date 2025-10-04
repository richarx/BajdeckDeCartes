using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Cards/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    public List<CardData> allCards;

    public CardData GetByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return allCards.Find(c => c != null && string.Equals(c.name, name, StringComparison.Ordinal));
    }

    public CardData GetRandomCard(Predicate<CardData> predicate)
    {
        List<CardData> pool = (predicate == null)
            ? allCards.FindAll(c => c != null)
            : allCards.FindAll(c => c != null && predicate(c));

        if (pool.Count == 0) return null;
        return pool[UnityEngine.Random.Range(0, pool.Count)];
    }
}
