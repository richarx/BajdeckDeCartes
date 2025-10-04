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
    [SerializeField] private Vector2Int _wearRange = new Vector2Int(10, 95);

    public GameObject GenerateCard(CardData data, Rarity rarity, int wearLevel)
    {
        GameObject cardPrefab = _cardPrefab;
        if (data.AlternatePrefab != null)
            cardPrefab = data.AlternatePrefab;
        GameObject cardObj = Instantiate(cardPrefab);
        cardObj.GetComponent<CardInstance>().Initialize(data, rarity, wearLevel);
        return cardObj;
    }

    public GameObject GenerateCard(CardData data)
    {
        Rarity rarity = GetRandomRarity();
        int wearLevel = Random.Range(_wearRange.x, _wearRange.y + 1);
        return GenerateCard(data, rarity, wearLevel);
    }

    public GameObject GenerateRandomPlayerCard()
    {
        CardData data = _database.GetRandomCard(c => c.AvailableForPlayer);
        return GenerateCard(data);
    }

    public GameObject GenerateRandomEnemyCard()
    {
        CardData data = _database.GetRandomCard(c => c.AvailableForEnemy);
        return GenerateCard(data, Rarity.Legendary, 100);
    }

    private Rarity GetRandomRarity()
    {
        int roll = Random.Range(0, _commonWeight + _rareWeight + _epicWeight + _legendaryWeight);
        if (roll < _commonWeight) return Rarity.Common;
        if (roll < _commonWeight + _rareWeight) return Rarity.Rare;
        if (roll < _commonWeight + _rareWeight + _epicWeight) return Rarity.Epic;
        return Rarity.Legendary;
    }
}
