using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeckZone : CardZone
{
    public override CardZoneType Type => CardZoneType.Deck;

    protected override void Organize(List<Transform> cardTransforms, float duration)
    {
        foreach (Transform card in cardTransforms)
        {
            card.DOMove(transform.position, duration);
            // set rotation
        }
    }
}
