// Assets/Scripts/ResourceDef.cs
using UnityEngine;

namespace CozyWorld
{
    /// <summary>Defines a harvestable resource type (berries, wood, stone)</summary>
    [CreateAssetMenu(fileName = "ResourceDef",
                     menuName = "CozyWorld/Resource Definition",
                     order = 0)]
    public class ResourceDef : ScriptableObject
    {
        public string id;              // unique key (e.g. "wood")
        public string displayName;     // UI label
        public Color color = Color.white;

        [Header("Gameplay")]
        public float harvestTime = 1f; // seconds per unit
        public int yieldPerHit = 1;  // amount added to inventory

        // Future fields: icon, stackSize, rarity, etc.
    }
}
