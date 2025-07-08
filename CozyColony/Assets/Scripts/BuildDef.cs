// Assets/Scripts/BuildDef.cs
using UnityEngine;

namespace CozyWorld               //  ? matches the other scripts
{
    /// <summary>
    /// Data-only asset that describes a buildable object
    /// (shown in the Build menu, used by placement and AI).
    /// </summary>
    [CreateAssetMenu(fileName = "BuildDef",
                     menuName = "CozyWorld/Build Definition",
                     order = 0)]
    public class BuildDef : ScriptableObject
    {
        [Header("Identity")]
        public string id;                 // unique key (e.g. "stockpile")
        public string displayName;        // UI label
        public Sprite icon;               // button icon

        [Header("Placed Prefab")]
        public GameObject prefab;         // mesh that appears in the world
        public Vector2Int size = new(1, 1);   // grid footprint (tiles)

        [Header("Build Costs")]
        public Inventory[] inputCosts;    // resources consumed to build

        [Header("Gameplay")]
        public int comfortBoost;          // beds/chairs raise Comfort
        public bool isStorage;            // stockpile flag
        public RecipeDef[] recipes;       // crafting bench can run these
    }
}
