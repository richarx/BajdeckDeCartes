using System.Collections.Generic;
using UnityEngine;

internal abstract class CardZone : MonoBehaviour
{
    readonly List<Transform> cards = new();

    [field: SerializeField]
    internal CardZoneMaster Master { get; private set; }

    internal abstract CardZoneType Type { get; }

    protected abstract void Organize(List<Transform> cardTransforms);

    internal void Add(Transform cardTransform)
    {
        cards.Add(cardTransform);

        Organize(cards);
    }

    internal void Remove(Transform cardTransform)
    {
        if (cards.Remove(cardTransform))
            Organize(cards);
    }

    internal void Clear()
    {
        cards.Clear();
    }

    /// <summary> Remember to call base.OnEnable(). </summary>
    protected virtual void OnEnable()
    {
        CardZonesManager.Zones.Add(this);
    }

    /// <summary> Remember to call base.OnDisable(). </summary>
    protected virtual void OnDisable()
    {
        CardZonesManager.Zones.Remove(this);
    }
}
