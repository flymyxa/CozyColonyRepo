using UnityEngine;

/// <summary>Generic helper utilities.</summary>
public static class Utils
{
    /// <summary>Resize a full-screen RectTransform to cover the drag area.</summary>
    public static void DrawScreenRect(RectTransform rect, Vector2 start, Vector2 end)
    {
        Vector2 lower = Vector2.Min(start, end);
        Vector2 upper = Vector2.Max(start, end);
        rect.anchoredPosition = lower;
        rect.sizeDelta = upper - lower;
    }
}