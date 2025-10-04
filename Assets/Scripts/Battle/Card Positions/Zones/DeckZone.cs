using System.Collections.Generic;
using UnityEngine;

internal class DeckZone : CardZone
{
    internal override CardZoneType Type => CardZoneType.Deck;

    protected override void Organize(List<Transform> cardTransforms)
    {

    }
}
