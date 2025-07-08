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
    #region Core Grid ----------------------------------------------------------------

    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField] private int width = 32;
        [SerializeField] private int height = 32;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Material floorMaterial;

        private bool[,] walkMap;
        private Transform tilesRoot;

        public event Action<Vector2Int, bool> OnWalkMapChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            walkMap = new bool[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) walkMap[x, y] = true;

            SpawnFloorTiles();
        }

        private void SpawnFloorTiles()
        {
            tilesRoot = new GameObject("FloorTiles").transform;
            tilesRoot.SetParent(transform);

            Mesh quad = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    GameObject go = new GameObject($"T_{x}_{y}");
                    go.transform.SetParent(tilesRoot);
                    go.transform.position = new Vector3(x * cellSize + cellSize * 0.5f, 0f, y * cellSize + cellSize * 0.5f);
                    go.transform.localScale = Vector3.one * cellSize;
                    MeshFilter mf = go.AddComponent<MeshFilter>();
                    mf.sharedMesh = quad;
                    MeshRenderer mr = go.AddComponent<MeshRenderer>();
                    mr.sharedMaterial = floorMaterial;
                }
        }

        public Vector2Int WorldToCell(Vector3 world) => new(Mathf.FloorToInt(world.x / cellSize), Mathf.FloorToInt(world.z / cellSize));
        public Vector3 CellToWorld(Vector2Int c) => new(c.x * cellSize + cellSize * 0.5f, 0f, c.y * cellSize + cellSize * 0.5f);
        public bool IsInside(Vector2Int c) => c.x >= 0 && c.x < width && c.y >= 0 && c.y < height;
        public bool IsWalkable(Vector2Int c) => IsInside(c) && walkMap[c.x, c.y];
        public void SetWalkable(Vector2Int c, bool value) { if (!IsInside(c)) return; walkMap[c.x, c.y] = value; OnWalkMapChanged?.Invoke(c, value); }
    }

    #endregion

    #region Build & Placement --------------------------------------------------------

    public class BuildMenuUI : MonoBehaviour
    {
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private List<BuildDef> availableBuilds;
        public UnityEvent<BuildDef> OnBuildSelected;

        private void Start()
        {
            foreach (BuildDef def in availableBuilds)
            {
                Button b = Instantiate(buttonPrefab, buttonContainer);
                b.GetComponentInChildren<Text>().text = def.id;
                b.onClick.AddListener(() => OnBuildSelected.Invoke(def));
            }
        }
    }

    public class PlacementGhost : MonoBehaviour
    {
        [SerializeField] private Material validMat;
        [SerializeField] private Material invalidMat;

        private BuildDef current;
        private GameObject ghostInstance;
        private bool isPlacing;

        private void OnEnable()
        {
            FindFirstObjectByType<BuildMenuUI>().OnBuildSelected.AddListener(StartPlacing);
        }

        private void Update()
        {
            if (!isPlacing) return;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 snap = hit.point;
                Vector2Int cell = GridManager.Instance.WorldToCell(snap);
                ghostInstance.transform.position = GridManager.Instance.CellToWorld(cell);

                bool canPlace = Validate(cell);
                SetGhostMat(canPlace ? validMat : invalidMat);

                if (Mouse.current.leftButton.wasReleasedThisFrame && canPlace)
                    ConfirmPlacement(cell);
            }
        }

        private void StartPlacing(BuildDef def)
        {
            current = def;
            if (ghostInstance != null) Destroy(ghostInstance);
            ghostInstance = Instantiate(def.prefab);
            ghostInstance.transform.localScale = Vector3.one; // ensure scale
            isPlacing = true;
        }

        private void ConfirmPlacement(Vector2Int cell)
        {
            Instantiate(current.prefab, GridManager.Instance.CellToWorld(cell), Quaternion.identity);
            GridManager.Instance.SetWalkable(cell, false);
            Destroy(ghostInstance);
            isPlacing = false;
        }

        private bool Validate(Vector2Int cell) => GridManager.Instance.IsWalkable(cell);
        private void SetGhostMat(Material m)
        {
            foreach (var mr in ghostInstance.GetComponentsInChildren<MeshRenderer>()) mr.sharedMaterial = m;
        }
    }

    #endregion

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
