using UnityEngine;

public static class Vector2Extensions
{
    /// <summary>
    /// Round off to remove precision error when comparing two vectors
    /// </summary>
    public static Vector2 RoundOff(this Vector2 vector)
    {
        float roundedX = Mathf.Round(vector.x * 1000f) / 1000f;
        float roundedY = Mathf.Round(vector.y * 1000f) / 1000f;
        return new Vector2(roundedX, roundedY);
    }
}
