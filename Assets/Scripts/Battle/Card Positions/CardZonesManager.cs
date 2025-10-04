using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CardZonesManager
{
    internal static List<CardZone> Zones { get; } = new();

    public static void MoveCardToZone(Transform card, CardZoneType type, CardZoneMaster master, float duration = 0.5f)
    {
        CardZone target = Zones.FirstOrDefault(cz => cz.Type == type && cz.Master == master);
        if (target == null)
            throw new Exception($"Couldn't find zone of type '{type}' and master '{master}'.");

        foreach (CardZone zone in Zones)
        {
            if (zone == target)
                continue;

            zone.Remove(card, duration);
        }

        target.Add(card, duration);
    }

    public static void ClearAllZones()
    {
        foreach (CardZone zone in Zones)
            zone.Clear();
    }
}
