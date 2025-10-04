using System.Collections.Generic;
using UnityEngine;

public class MonsterZone : CardZone
{
    public override CardZoneType Type => CardZoneType.Monster;

    protected override void Organize(List<Transform> cardTransforms, float duration)
    {
        // TODO
    }
}
