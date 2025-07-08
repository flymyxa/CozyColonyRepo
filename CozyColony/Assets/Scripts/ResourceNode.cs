// Assets/Scripts/ResourceNode.cs
// --------------------------------------------------------------
// Cozy-World – harvestable resource node (complete & standalone)
// --------------------------------------------------------------
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;   // for Mouse.current
#endif

namespace CozyWorld
{
    /// <summary>
    /// Attach to harvestable prefabs (BerryBush, TreeStump, …).
    /// Handles quantity, respawn, colour-tint, and right-click queuing
    /// for the vertical-slice prototype.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ResourceNode : MonoBehaviour
    {
        [Header("Definition")]
        public ResourceDef resourceDef;

        [Header("Quantities")]
        public int totalQuantity = 6;
        public int yieldPerHit = 2;
        public float harvestTimeOverride = 0f;   // 0 → use ResourceDef.harvestTime
        public float respawnSeconds = 0f;   // 0 → no respawn

        private int _quantity;
        private float _depletedTime;
        private Renderer _rend;
        private Color _origColour;

        // ---------------------------------------------------- lifecycle
        private void Awake()
        {
            _quantity = totalQuantity;
            _rend = GetComponentInChildren<Renderer>();

            if (_rend)
            {
                _origColour = _rend.material.color;
                if (resourceDef) _rend.material.color = resourceDef.color;
            }
        }

        private void Update()
        {
            // Handle respawn countdown
            if (_quantity > 0 || respawnSeconds <= 0f) return;

            if (Time.time - _depletedTime >= respawnSeconds)
            {
                _quantity = totalQuantity;
                if (_rend) _rend.material.color = _origColour;
            }
        }

        // ---------------------------------------------------- prototype input
        private void OnMouseOver()
        {
#if ENABLE_INPUT_SYSTEM
            bool rightClick = Mouse.current.rightButton.wasReleasedThisFrame;
#else
            bool rightClick = Input.GetMouseButtonUp(1);
#endif
            if (!rightClick) return;

            // Queue a harvest job
            JobSystem.Enqueue(new Job
            {
                type = JobType.Harvest,
                targetPos = transform.position,
                node = gameObject
            });

            Debug.Log($"[Input] Harvest job queued for {resourceDef.displayName}");
        }

        // ---------------------------------------------------- called by ColonistAgent
        /// <summary>
        /// Executes one harvest action when the agent arrives.
        /// </summary>
        public bool HarvestOnce(ColonistAgent agent = null)
        {
            if (_quantity <= 0) return false;

            int realYield = Mathf.Min(yieldPerHit, _quantity);
            _quantity -= realYield;

            string harvester = agent != null ? agent.name : "[Unknown]";
            Debug.Log($"{harvester} harvested {realYield} {resourceDef.displayName}");

            if (_quantity <= 0)
            {
                _depletedTime = Time.time;
                if (_rend) _rend.material.color = Color.gray;
            }
            return true;
        }
    }
}
