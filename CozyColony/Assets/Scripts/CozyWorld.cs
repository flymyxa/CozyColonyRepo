// -----------------------------------------------------------------------------
// CozyWorld Colony – Vertical Slice
// Core Implementation Pass #1 (Grid Visuals, Build/Place, Basic Harvest)
// Unity 6000 LTS – Landscape
// -----------------------------------------------------------------------------
// This update fleshes out the following systems:
//   • GridManager – now spawns a floor tile mesh per cell using a shared material
//   • BuildMenuUI – instantiates runtime UI buttons from BuildDef list
//   • PlacementGhost – previews selected BuildDef, validates cells, places prefab
//   • ResourceNode – adds simple interaction: right‑click harvest for test
//   • AStarPathfinder – minimal Manhattan-distance path for vertical slice
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI; // for UI Button

namespace CozyWorld
{
    
    #region Minimal Pathfinder --------------------------------------------------------

    public class AStarPathfinder
    {
        public Vector3[] FindPath(Vector2Int start, Vector2Int goal)
        {
            List<Vector3> path = new();
            Vector2Int cur = start;
            while (cur != goal)
            {
                if (cur.x < goal.x) cur.x++;
                else if (cur.x > goal.x) cur.x--;
                else if (cur.y < goal.y) cur.y++;
                else cur.y--;
                path.Add(GridManager.Instance.CellToWorld(cur));
            }
            return path.ToArray();
        }
    }

    #endregion
}
