using System.Collections.Generic;
using EasyButtons;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private CardGeneratorConfig _generatorConfig;
    [SerializeField] private List<CardData> cardsToSpawn;


    private void Start()
    {
        foreach (var cardData in cardsToSpawn)
        {
            if (cardData == null)
            {
                SpawnRandomCard();
            }
            else
            {
                SpawnCardFromData(cardData);
            }
        }
    }

    [Button]
    public void SpawnRandomCard()
    {
        GameObject cardObj = _generatorConfig.GenerateRandomCard();
        if (cardObj == null) return;

        cardObj.transform.SetParent(transform, false);
        cardObj.transform.localPosition = Vector3.zero;
    }

    [Button]
    public void SpawnCardFromData(CardData cardData)
    {
        if (cardData == null) return;
        GameObject cardObj = _generatorConfig.GenerateCard(cardData);
        if (cardObj == null) return;

        cardObj.transform.SetParent(transform, false);
        cardObj.transform.localPosition = Vector3.zero;
    }

    [Button]
    public void SpawnCardFromNumber(int number)
    {
        GameObject cardObj = _generatorConfig.GenerateCard(number);
        if (cardObj == null) return;

        cardObj.transform.SetParent(transform, false);
        cardObj.transform.localPosition = Vector3.zero;
    }
}
