using UnityEngine;

[CreateAssetMenu(fileName = "CardGeneratorConfig", menuName = "Cards/Card Generator Config")]
public class CardGeneratorConfig : ScriptableObject
{
    [SerializeField] private CardDatabase database;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField, Range(0, 100)] private int commonWeight = 69;
    [SerializeField, Range(0, 100)] private int rareWeight = 20;
    [SerializeField, Range(0, 100)] private int epicWeight = 10;
    [SerializeField, Range(0, 100)] private int legendaryWeight = 1;
    [SerializeField] private Vector2Int wearRange = new Vector2Int(10, 95);

    public GameObject GenerateCard(CardData data, Rarity rarity, int wearLevel)
    {
        GameObject cardObj = Instantiate(cardPrefab);
        cardObj.GetComponent<CardInstance>().Initialize(data, rarity, wearLevel);
        return cardObj;
    }

    public GameObject GenerateCard(CardData data)
    {
        Rarity rarity = GetRandomRarity();
        int wearLevel = Random.Range(wearRange.x, wearRange.y + 1);
        return GenerateCard(data, rarity, wearLevel);
    }

    public GameObject GenerateRandomPlayerCard()
    {
        CardData data = database.GetRandomCard(c => c.availableForPlayer);
        return GenerateCard(data);
    }

    public GameObject GenerateRandomEnemyCard()
    {
        CardData data = database.GetRandomCard(c => c.availableForEnemy);
        return GenerateCard(data, Rarity.Legendary, 100);
    }

    private Rarity GetRandomRarity()
    {
        int roll = Random.Range(0, commonWeight + rareWeight + epicWeight + legendaryWeight);
        if (roll < commonWeight) return Rarity.Common;
        if (roll < commonWeight + rareWeight) return Rarity.Rare;
        if (roll < commonWeight + rareWeight + epicWeight) return Rarity.Epic;
        return Rarity.Legendary;
    }
}
