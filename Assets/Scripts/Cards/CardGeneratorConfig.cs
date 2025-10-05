using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CardGeneratorConfig", menuName = "Cards/Card Generator Config")]
public class CardGeneratorConfig : ScriptableObject
{
    [SerializeField] private CardDatabase _database;
    [SerializeField] private GameObject _cardPrefab;

    [SerializeField, Range(0, 100)] private int _commonWeight = 69;
    [SerializeField, Range(0, 100)] private int _rareWeight = 20;
    [SerializeField, Range(0, 100)] private int _epicWeight = 10;
    [SerializeField, Range(0, 100)] private int _legendaryWeight = 1;
    [SerializeField] private Vector2Int _wearRange = new Vector2Int(1, 10);

    public GameObject GenerateCard(CardData data, ushort salt, Rarity rarity, int wearLevel)
    {
        GameObject cardPrefab = _cardPrefab;
        if (data.AlternatePrefab != null)
            cardPrefab = data.AlternatePrefab;
        GameObject cardObj = Instantiate(cardPrefab);
        cardObj.GetComponent<CardInstance>().Initialize(data, salt, rarity, wearLevel);
        return cardObj;
    }

    public GameObject GenerateCard(CardData data)
    {
        Rarity rarity = GetRandomRarity();
        var b = new byte[2];
        using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
        ushort salt = (ushort)((b[0] << 8) | b[1]);
        int wearLevel = UnityEngine.Random.Range(_wearRange.x, _wearRange.y + 1);
        return GenerateCard(data, salt, rarity, wearLevel);
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
        return GenerateCard(cardData, data.UUID, (Rarity)data.Quality, data.Wear);
    }

    public GameObject GenerateRandomCard()
    {
        CardData data = _database.GetRandomCard();
        return GenerateCard(data);
    }


    private Rarity GetRandomRarity()
    {
        int roll = UnityEngine.Random.Range(0, _commonWeight + _rareWeight + _epicWeight + _legendaryWeight);
        if (roll < _commonWeight) return Rarity.Common;
        if (roll < _commonWeight + _rareWeight) return Rarity.Rare;
        if (roll < _commonWeight + _rareWeight + _epicWeight) return Rarity.Epic;
        return Rarity.Legendary;
    }
}
