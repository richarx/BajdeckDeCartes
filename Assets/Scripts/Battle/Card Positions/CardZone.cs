using System.Collections.Generic;
using UnityEngine;

public abstract class CardZone : MonoBehaviour
{
    readonly List<Transform> cards = new();

    [field: SerializeField]
    public CardZoneMaster Master { get; private set; }

    public abstract CardZoneType Type { get; }

    protected abstract void Organize(List<Transform> cardTransforms, float duration);

    internal void Add(Transform cardTransform, float duration)
    {
        cards.Add(cardTransform);

        Organize(cards, duration);
    }

    internal void Remove(Transform cardTransform, float duration)
    {
        if (cards.Remove(cardTransform))
            Organize(cards, duration);
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
