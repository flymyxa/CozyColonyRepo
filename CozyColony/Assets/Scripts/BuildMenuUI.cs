using CozyWorld;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI; // for UI Button
using TMPro;     

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
            b.GetComponentInChildren<TextMeshProUGUI>().text = def.displayName;
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
