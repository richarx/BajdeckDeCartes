using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CardGeneratorConfig", menuName = "Cards/Card Generator Config")]
public class CardGeneratorConfig : ScriptableObject
{
    [SerializeField] private CardDatabase _database;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Vector2Int _wearRange = new(1, 10);

    [Serializable]
    public struct WeightedEnum<T> where T : Enum
    {
        public T Type;
        [Range(0, 100)] public int Weight;
    }

    [Header("Rarity weights")]
    [SerializeField] private List<WeightedEnum<Rarity>> _rarityWeights = new();

    [Header("Quality weights")]
    [SerializeField] private List<WeightedEnum<Quality>> _qualityWeights = new();

    private void OnValidate()
    {
        EnsureEnumEntries(_rarityWeights);
        EnsureEnumEntries(_qualityWeights);
    }

    private void EnsureEnumEntries<T>(List<WeightedEnum<T>> list) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>();
        foreach (var value in values)
        {
            if (!list.Any(w => EqualityComparer<T>.Default.Equals(w.Type, value)))
                list.Add(new WeightedEnum<T> { Type = value, Weight = 1 });
        }

        list.RemoveAll(w => !values.Contains(w.Type));
    }

    public GameObject GenerateCard(CardData data, ushort salt, Quality quality, int wearLevel)
    {
        GameObject cardPrefab = _cardPrefab;
        if (data.AlternatePrefab != null)
            cardPrefab = data.AlternatePrefab;
        GameObject cardObj = Instantiate(cardPrefab);
        cardObj.GetComponent<CardInstance>().Initialize(data, salt, quality, wearLevel);
        return cardObj;
    }

    public GameObject GenerateCard(CardData data)
    {
        Quality quality = GetRandomWeighted(_qualityWeights);
        var b = new byte[2];
        using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
        ushort salt = (ushort)((b[0] << 8) | b[1]);
        int wearLevel = UnityEngine.Random.Range(_wearRange.x, _wearRange.y + 1);
        return GenerateCard(data, salt, quality, wearLevel);
    }

    public GameObject GenerateCard(int number)
    {
        CardData data = _database.GetByNumber(number);
        if (data == null)
        {
            Debug.LogWarning($"Unknown card number: {number}");
            return null;
        }
        return GenerateCard(data);
    }

    public GameObject GenerateCard(string code, string key)
    {
        Conversion.Data data = Conversion.FromCode(code, key);
        if (data == null)
        {
            Debug.LogWarning($"Invalid card code: {code}");
            return null;
        }
        CardData cardData = _database.GetByNumber(data.Number);
        if (cardData == null)
        {
            Debug.LogWarning($"Unknown card number: {data.Number}");
            return null;
        }
        return GenerateCard(cardData, data.UUID, (Quality)data.Quality, data.Wear);
    }

    public GameObject GenerateRandomCard()
    {
        Rarity rarity = GetRandomWeighted(_rarityWeights);
        CardData data = _database.GetRandomCard((x) => x.Rarity == rarity);
        return GenerateCard(data);
    }

    private static TEnum GetRandomWeighted<TEnum>(List<WeightedEnum<TEnum>> list) where TEnum : Enum
    {
        int total = list.Sum(x => x.Weight);
        int roll = UnityEngine.Random.Range(1, total + 1);
        int cumulative = 0;

        foreach (var entry in list)
        {
            cumulative += entry.Weight;
            if (roll <= cumulative)
                return entry.Type;
        }

        return list.Last().Type;
    }
}
