// -----------------------------------------------------------------------------
// CozyWorld Colony – Vertical Slice
// Initial Script Stubs (Unity 6000 LTS, URP, Landscape) — UPDATED
// -----------------------------------------------------------------------------
//  These stubs compile under a single assembly definition (CozyWorld.asmdef).
//  Fixes:
//    • NeedsComponent.Start now initializes need values with an index loop to avoid
//      CS1654 (cannot modify foreach iteration variable).
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CozyWorld
{
    #region Core Grid ----------------------------------------------------------------

    /// <summary>
    /// Manages the square-grid world. Provides helper methods for converting between
    /// world positions and cell coordinates, and raises events when tiles change.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField] private int width = 32;
        [SerializeField] private int height = 32;
        [SerializeField] private float cellSize = 1f;

        /// <summary>True if a cell is walkable.</summary>
        private bool[,] walkMap;

        public event Action<Vector2Int, bool> OnWalkMapChanged; // cell, isWalkable

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            walkMap = new bool[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) walkMap[x, y] = true;
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / cellSize);
            int y = Mathf.FloorToInt(worldPosition.z / cellSize);
            return new Vector2Int(x, y);
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(cell.x * cellSize + cellSize * 0.5f, 0f, cell.y * cellSize + cellSize * 0.5f);
        }

        public bool IsWalkable(Vector2Int cell) => IsInside(cell) && walkMap[cell.x, cell.y];

        public void SetWalkable(Vector2Int cell, bool value)
        {
            if (!IsInside(cell)) return;
            walkMap[cell.x, cell.y] = value;
            OnWalkMapChanged?.Invoke(cell, value);
        }

        private bool IsInside(Vector2Int c) => c.x >= 0 && c.x < width && c.y >= 0 && c.y < height;
    }

    #endregion

    #region Inventory -----------------------------------------------------------------

    /// <summary>Simple stackable item record.</summary>
    [Serializable]
    public struct InventoryItem
    {
        public int ItemId;
        public int Amount;

        public InventoryItem(int id, int amount = 0) { ItemId = id; Amount = amount; }
    }

    #endregion

    #region ScriptableObject Definitions ----------------------------------------------

    public enum ResourceId { Food = 0, Wood = 1, Stone = 2, Fabric = 3, Crystal = 4 }

    /// <summary>Definition for harvestable resource nodes.</summary>
    [CreateAssetMenu(menuName = "CozyWorld/ResourceDef")]
    public class ResourceDef : ScriptableObject
    {
        public ResourceId id;
        public Color color;
        public float harvestTime = 2f;
    }

    /// <summary>Definition for buildable placeable objects.</summary>
    [CreateAssetMenu(menuName = "CozyWorld/BuildDef")]
    public class BuildDef : ScriptableObject
    {
        public string id;
        public GameObject prefab;
        public Vector2Int size = Vector2Int.one;
        public List<InventoryItem> inputCost = new();
        public int comfortBoost;
    }

    /// <summary>Definition for colonist needs.</summary>
    [CreateAssetMenu(menuName = "CozyWorld/NeedDef")]
    public class NeedDef : ScriptableObject
    {
        public string id;
        public float decayPerHour = 5f;
        public float satisfyValue = 30f;
    }

    /// <summary>Definition for workstation crafting recipes.</summary>
    [CreateAssetMenu(menuName = "CozyWorld/RecipeDef")]
    public class RecipeDef : ScriptableObject
    {
        public string id;
        public List<InventoryItem> inputs = new();
        public List<InventoryItem> outputs = new();
        public float workTime = 3f;
    }

    /// <summary>Definition for weather events.</summary>
    [CreateAssetMenu(menuName = "CozyWorld/WeatherDef")]
    public class WeatherDef : ScriptableObject
    {
        public string id;
        public float durationInHours = 6f;
        public float cropGrowthModifier = 0.7f;
        public bool extinguishCampfires;
    }

    #endregion

    #region Resource Node --------------------------------------------------------------

    /// <summary>Harvestable node placed on the grid.</summary>
    public class ResourceNode : MonoBehaviour
    {
        [SerializeField] private ResourceDef definition;
        [SerializeField] private int quantity = 100;

        public ResourceDef Definition => definition;

        public bool Harvest(int amount)
        {
            if (quantity < amount) return false;
            quantity -= amount;
            if (quantity <= 0) Destroy(gameObject);
            return true;
        }
    }

    #endregion

    #region Needs & Colonist -----------------------------------------------------------

    /// <summary>Holds current need values and ticks decay.</summary>
    public class NeedsComponent : MonoBehaviour
    {
        [Serializable]
        public struct NeedEntry
        {
            public NeedDef def;
            public float value;
        }

        [SerializeField] private List<NeedEntry> needs = new();
        public event UnityAction<NeedDef, float> OnNeedChanged; // def, new value

        private void Start()
        {
            // Initialize all needs to full (100) using index loop to avoid foreach mutability error.
            for (int i = 0; i < needs.Count; i++)
            {
                var entry = needs[i];
                entry.value = 100f;
                needs[i] = entry;
            }
        }

        private void Update()
        {
            float deltaMinutes = Time.deltaTime / 60f; // seconds ➜ minutes
            for (int i = 0; i < needs.Count; i++)
            {
                var e = needs[i];
                e.value = Mathf.Clamp(e.value - e.def.decayPerHour * deltaMinutes, 0f, 100f);
                needs[i] = e;
                OnNeedChanged?.Invoke(e.def, e.value);
            }
        }
    }

    /// <summary>Main colonist agent behaviour (movement, job execution).</summary>
    [RequireComponent(typeof(NeedsComponent))]
    public class ColonistAgent : MonoBehaviour
    {
        [Header("Stats")]
        public float moveSpeed = 2f;

        private NeedsComponent needs;
        private readonly Queue<JobData> jobQueue = new();
        private Vector3[] currentPath;
        private int pathIndex;

        private void Awake() => needs = GetComponent<NeedsComponent>();

        private void Update()
        {
            if (currentPath == null || pathIndex >= currentPath.Length)
            {
                RequestNextJob();
                return;
            }

            MoveAlongPath();
        }

        private void MoveAlongPath()
        {
            Vector3 target = currentPath[pathIndex];
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, target) < 0.05f) pathIndex++;
        }

        private void RequestNextJob()
        {
            if (!JobSystem.TryDequeue(out JobData job)) return;
            // TODO: pathfind & execute job
        }
    }

    #endregion

    #region Job System -----------------------------------------------------------------

    public struct JobData
    {
        public Vector2Int targetCell;
        public float workTime;
    }

    public static class JobSystem
    {
        private static readonly Queue<JobData> queue = new();
        public static void Enqueue(JobData job) => queue.Enqueue(job);
        public static bool TryDequeue(out JobData job)
        {
            if (queue.Count > 0) { job = queue.Dequeue(); return true; }
            job = default; return false;
        }
    }

    #endregion

    #region A* Pathfinder --------------------------------------------------------------

    /// <summary>Grid-based A* pathfinder (placeholder, to be optimized).</summary>
    public class AStarPathfinder
    {
        private readonly GridManager grid;
        private readonly int width, height;

        public AStarPathfinder(GridManager gm)
        {
            grid = gm;
            width = 32; height = 32; // TODO: derive from GridManager
        }

        public Vector3[] FindPath(Vector2Int start, Vector2Int goal)
        {
            // TODO: implement pathfinding; return world positions list
            return Array.Empty<Vector3>();
        }
    }

    #endregion

    #region UI & Placement -------------------------------------------------------------

    /// <summary>Populates build menu from BuildDef list and raises placement events.</summary>
    public class BuildMenuUI : MonoBehaviour
    {
        public UnityEvent<BuildDef> OnBuildSelected;
        [SerializeField] private List<BuildDef> availableBuilds;
        // TODO: Instantiate UI buttons based on availableBuilds
    }

    /// <summary>Shows ghost mesh and validates placement.</summary>
    public class PlacementGhost : MonoBehaviour
    {
        private BuildDef currentDef;
        private bool isPlacing;

        private void OnEnable()
        {
            // Subscribe to BuildMenuUI OnBuildSelected
        }

        private void Update()
        {
            if (!isPlacing) return;
            // TODO: follow cursor / joystick, highlight valid cells
        }

        private void ConfirmPlacement()
        {
            // TODO: instantiate prefab & notify GridManager
        }
    }

    #endregion

    #region Systems --------------------------------------------------------------------

    /// <summary>Saves and loads binary compressed snapshots of game state.</summary>
    public class SaveLoadManager : MonoBehaviour
    {
        public void Save(string slot)
        {
            // TODO: serialize data, compress, write to Application.persistentDataPath
        }

        public void Load(string slot)
        {
            // TODO: read, decompress, deserialize, spawn scene objects
        }
    }

    /// <summary>Schedules timed events like visitor quests, festival triggers.</summary>
    public class EventScheduler : MonoBehaviour
    {
        private readonly List<ScheduledEvent> events = new();

        private void Update()
        {
            float t = Time.time;
            for (int i = events.Count - 1; i >= 0; i--)
            {
                if (t >= events[i].time) { events[i].action.Invoke(); events.RemoveAt(i); }
            }
        }

        public void Schedule(float delaySeconds, Action action) => events.Add(new ScheduledEvent(Time.time + delaySeconds, action));

        private readonly struct ScheduledEvent
        {
            public readonly float time; public readonly Action action;
            public ScheduledEvent(float t, Action a) { time = t; action = a; }
        }
    }

    /// <summary>Handles weather events (rain in vertical slice).</summary>
    public class WeatherSystem : MonoBehaviour
    {
        public event UnityAction<WeatherDef> OnWeatherStarted;
        public event UnityAction OnWeatherEnded;

        [SerializeField] private List<WeatherDef> possibleWeather;
        private WeatherDef current;
        private float timer;

        public void StartWeather(WeatherDef def)
        {
            current = def; timer = def.durationInHours * 60f; // hours ➜ seconds
            OnWeatherStarted?.Invoke(def);
        }

        private void Update()
        {
            if (current == null) return;
            timer -= Time.deltaTime;
            if (timer <= 0f) { current = null; OnWeatherEnded?.Invoke(); }
        }
    }

    #endregion
}
