using System.Collections.Generic;
using UnityEngine;

public class HandZone : CardZone
{
    public override CardZoneType Type => CardZoneType.Hand;

    protected override void Organize(List<Transform> cardTransforms, float duration)
    {
        // TODO
    }
}
