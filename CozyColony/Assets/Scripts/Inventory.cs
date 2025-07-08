// -----------------------------------------------------------------------------
// CozyWorld Colony – Support Types Added
// -----------------------------------------------------------------------------
//  • Inventory.cs – lightweight serializable resource stack
//  • RecipeDef.cs – ScriptableObject for crafting recipes
// -----------------------------------------------------------------------------

// Inventory.cs ---------------------------------------------------------------
using System;
using UnityEngine;


namespace CozyWorld
{
    /// <summary>
    /// Compact representation of an item stack (id + count).
    /// Serializable so it appears in the inspector and saves easily.
    /// </summary>
    [Serializable]
    public struct Inventory
    {
        public string itemId;   // must match ResourceDef.id
        [Min(0)] public int count;

        public Inventory(string id, int amount)
        {
            itemId = id;
            count = amount;
        }
    }
}

// RecipeDef.cs --------------------------------------------------------------

