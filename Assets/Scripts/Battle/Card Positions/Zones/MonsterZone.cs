using System.Collections.Generic;
using UnityEngine;

internal class MonsterZone : CardZone
{
    internal override CardZoneType Type => CardZoneType.Monster;

    protected override void Organize(List<Transform> cardTransforms)
    {

    }
}
