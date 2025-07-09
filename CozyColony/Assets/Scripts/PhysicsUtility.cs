using UnityEngine;
using System.Collections.Generic;

/// <summary>Physics helpers for screen-space selection.</summary>
public static class PhysicsUtility
{
    /// <summary>
    /// Returns up to maxCount colliders under the screen-rect that match nodeMask.
    /// Expects a world-space canvas with the RectTransform passed in.
    /// </summary>
    public static List<GameObject> NodesInScreenRect(RectTransform rect, LayerMask nodeMask, int maxCount)
    {
        var results = new List<GameObject>();
        rect.GetWorldCorners(corners);
        Vector3 center = (corners[0] + corners[2]) * 0.5f;
        Vector3 size = corners[2] - corners[0];

        var hits = Physics.OverlapBox(center, size * 0.5f, Quaternion.identity, nodeMask);
        foreach (var h in hits)
        {
            if (results.Count >= maxCount) break;
            results.Add(h.gameObject);
        }
        return results;
    }

    private static readonly Vector3[] corners = new Vector3[4];
}
