// ---------- 6. ResourceNode (full implementation with HarvestOnce) ----------------
using UnityEngine;
using System.Collections;

namespace CozyWorld
{
    /// <summary>
    /// Attach to any harvestable prefab (BerryBush, TreeStump…). Handles quantity,
    /// respawn timer, color tint when depleted, and exposes HarvestOnce() for agents.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ResourceNode : MonoBehaviour
    {
        [Header("Definition")]
        public ResourceDef resourceDef;

        [Header("Quantities")]
        public int totalQuantity = 6;
        public int yieldPerHit = 2;
        public float harvestTimeOverride = 0f; // 0 = use ResourceDef.harvestTime
        public float respawnSeconds = 0f;      // 0 = no respawn

        private int _quantity;
        private float _depletedTime;
        private Renderer _rend;
        private Color _origColor;

        private void Awake()
        {
            _quantity = totalQuantity;
            _rend = GetComponentInChildren<Renderer>();
            if (_rend)
            {
                _origColor = _rend.material.color;
                if (resourceDef) _rend.material.color = resourceDef.color;
            }
        }

        private void Update()
        {
            if (_quantity > 0 || respawnSeconds <= 0f) return;
            if (Time.time - _depletedTime >= respawnSeconds)
            {
                _quantity = totalQuantity;
                if (_rend) _rend.material.color = _origColor;
            }
        }

        /// <summary>Called by ColonistAgent when they reach the node.</summary>
        /// <returns>true if something was harvested</returns>
        public bool HarvestOnce(ColonistAgent agent = null)
        {
            if (_quantity <= 0) return false;

            int realYield = Mathf.Min(yieldPerHit, _quantity);
            _quantity -= realYield;

            // TODO: Add items to agent inventory once implemented.
            Debug.Log($"{agent?.name ?? "[Unknown]"} harvested {realYield} {resourceDef.displayName}");

            if (_quantity <= 0)
            {
                _depletedTime = Time.time;
                if (_rend) _rend.material.color = Color.gray;
            }
            return true;
        }
    }
}
