using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private CardGeneratorConfig _generatorConfig;


    private void Start()
    {
        SpawnRandomCard();
    }

    public void SpawnRandomCard()
    {
        if (_generatorConfig == null)
        {
            Debug.LogWarning("CardGeneratorConfig non assigné !");
            return;
        }

        GameObject cardObj = _generatorConfig.GenerateRandomPlayerCard();
        if (cardObj == null) return;

        cardObj.transform.SetParent(transform, false);
        cardObj.transform.localPosition = Vector3.zero;
    }
}
