using System.Collections.Generic;
using UnityEngine;

internal class HandZone : CardZone
{
    internal override CardZoneType Type => CardZoneType.Hand;

    protected override void Organize(List<Transform> cardTransforms)
    {

    }
}
