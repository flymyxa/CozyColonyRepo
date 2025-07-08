// Assets/Scripts/PlacementGhost.cs
using UnityEngine;
using UnityEngine.InputSystem;

namespace CozyWorld
{
    /// <summary>
    /// Listens to BuildMenuUI.OnBuildSelected, shows a colour-coded ghost,
    /// validates the target cell, and places the prefab when you click.
    /// </summary>
    public class PlacementGhost : MonoBehaviour
    {
        [Header("Visuals")]
        public Material validMat;
        public Material invalidMat;

        private BuildDef _current;
        private GameObject _ghost;
        private bool _isPlacing;

        private void OnEnable()
        {
            // Safe even if BuildMenuUI lives in another scene; just try once.
            var menu = FindFirstObjectByType<BuildMenuUI>();
            if (menu) menu.OnBuildSelected.AddListener(BeginPlacing);
        }

        private void BeginPlacing(BuildDef def)
        {
            _current = def;
            if (_ghost) Destroy(_ghost);
            _ghost = Instantiate(def.prefab);
            _ghost.transform.localScale = Vector3.one;     // reset any prefab scale
            _isPlacing = true;
        }

        private void Update()
        {
            if (!_isPlacing || _ghost == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;

            Vector2Int cell = GridManager.Instance.WorldToCell(hit.point);
            _ghost.transform.position = GridManager.Instance.CellToWorld(cell);

            bool canPlace = GridManager.Instance.IsWalkable(cell);
            ApplyMat(canPlace ? validMat : invalidMat);

            // Confirm
            if (Mouse.current.leftButton.wasReleasedThisFrame && canPlace)
            {
                Instantiate(_current.prefab,
                            GridManager.Instance.CellToWorld(cell),
                            Quaternion.identity);
                GridManager.Instance.SetWalkable(cell, false);
                Cancel();
            }

            // Cancel with right-click
            if (Mouse.current.rightButton.wasReleasedThisFrame) Cancel();
        }

        private void ApplyMat(Material m)
        {
            foreach (var mr in _ghost.GetComponentsInChildren<MeshRenderer>())
                mr.sharedMaterial = m;
        }

        private void Cancel()
        {
            if (_ghost) Destroy(_ghost);
            _isPlacing = false;
        }
    }
}
