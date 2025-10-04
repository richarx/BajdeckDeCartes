using UnityEngine;

public static class Tools
{
    public static float NormalizeValue(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
}
