using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EasyButtons;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private CardGeneratorConfig _generatorConfig;
    [SerializeField] private List<CardData> cardsToSpawn;

    [SerializeField] private Transform boosterParent;
    [SerializeField] private float startScale = 2;


    async private void Start()
    {
        await UniTask.WaitUntil(() => CardTableManager.Instance != null);
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


        SetupCard(cardObj);
    }

    [Button]
    public void SpawnCardFromData(CardData cardData)
    {
        if (cardData == null) return;
        GameObject cardObj = _generatorConfig.GenerateCard(cardData);
        if (cardObj == null) return;

        SetupCard(cardObj);
    }

    [Button]
    public void SpawnCardFromNumber(int number)
    {
        GameObject cardObj = _generatorConfig.GenerateCard(number);
        if (cardObj == null) return;

        SetupCard(cardObj);
    }

    public void SpawnNRandomCardsSortedByRarity(int N, bool putOnTable = true)
    {
        List<CardInstance> spawned = new List<CardInstance>();

        
        for (int i = 0; i < N; i++)
        {

            GameObject cardObj = _generatorConfig.GenerateRandomCard();
            // Pour les effets lumineux plus tard ?
            var cardInstance = cardObj.GetComponent<CardInstance>();
            if (cardInstance == null)
            {
                Debug.LogError("No card instance in card spawned");
                return;
            }
            spawned.Add(cardInstance);
        }

        spawned.Sort((a, b) =>
        {
            return ( b.Data.Rarity - a.Data.Rarity);
        });

        for (int i = 0; i <  N; i++)
        {
            SetupCard(spawned[i].gameObject, i, putOnTable);
        }
    }

    private void SetupCard(GameObject cardObj, int sortingOrder = 0, bool putOnTable = true)
    {


        cardObj.transform.localPosition = Vector3.zero;

        var draggable = cardObj.GetComponent<Draggable>();

        draggable.Canvas_.sortingOrder = sortingOrder;
        if (!putOnTable)
            draggable.Canvas_.sortingLayerName = "Cards";
        if (boosterParent != null)
        {
            cardObj.transform.SetParent(boosterParent, true);
            cardObj.transform.SetAsFirstSibling();
        }

        draggable.SetToScale(startScale);
        cardObj.transform.localScale = new Vector3(startScale, startScale, 1);

    }
}
