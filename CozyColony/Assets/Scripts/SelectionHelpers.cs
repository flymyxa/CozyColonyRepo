using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Geometric helpers for translating drag-selected cells into world positions.
/// </summary>
public static class SelectionHelpers
{
    /// <summary>
    /// Converts a list of terrain-cell GameObjects (the ones returned by
    /// SelectionBox) into the centre point of each cell so we can spawn a
    /// blueprint exactly on the grid.
    /// </summary>
    /// <param name="cells">GameObjects that represent a terrain tile.</param>
    /// <returns>IEnumerable of Vector3 world-space centres.</returns>
    public static IEnumerable<Vector3> CellCenters(IEnumerable<GameObject> cells)
    {
        foreach (var c in cells)
        {
            // Use the object’s collider bounds if present, otherwise its transform.
            if (c.TryGetComponent(out Collider col))
                yield return col.bounds.center;
            else
                yield return c.transform.position;
        }
    }
}
