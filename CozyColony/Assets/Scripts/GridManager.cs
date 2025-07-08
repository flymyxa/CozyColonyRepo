using UnityEngine;
using System;

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
                go.transform.position = new Vector3(
                    x * cellSize + cellSize * 0.5f,
                    0f,
                    y * cellSize + cellSize * 0.5f);

                go.transform.localScale = Vector3.one * cellSize;

                // NEW – lay the quad flat so +Y is its normal
                go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = quad;

                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = floorMaterial;

                // NEW – give NavMesh something to bake against
                go.AddComponent<MeshCollider>().sharedMesh = quad;
                go.isStatic = true;                // mark Navigation Static
            }
    }


    public Vector2Int WorldToCell(Vector3 world) => new(Mathf.FloorToInt(world.x / cellSize), Mathf.FloorToInt(world.z / cellSize));
    public Vector3 CellToWorld(Vector2Int c) => new(c.x * cellSize + cellSize * 0.5f, 0f, c.y * cellSize + cellSize * 0.5f);
    public bool IsInside(Vector2Int c) => c.x >= 0 && c.x < width && c.y >= 0 && c.y < height;
    public bool IsWalkable(Vector2Int c) => IsInside(c) && walkMap[c.x, c.y];
    public void SetWalkable(Vector2Int c, bool value) { if (!IsInside(c)) return; walkMap[c.x, c.y] = value; OnWalkMapChanged?.Invoke(c, value); }
}