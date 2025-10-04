using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CemeteryZone : CardZone
{
    public override CardZoneType Type => CardZoneType.Cemetery;

    protected override void Organize(List<Transform> cardTransforms, float duration)
    {
        foreach (Transform card in cardTransforms)
        {
            card.DOMove(transform.position, duration);
            // set rotation
        }
    }
}
