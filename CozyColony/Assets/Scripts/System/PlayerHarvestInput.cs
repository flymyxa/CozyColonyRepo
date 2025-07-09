// Assets/Scripts/Systems/PlayerHarvestInput.cs
using UnityEngine;
using UnityEngine.InputSystem;          // Mouse
using CozyWorld;                        // JobSystem + ResourceNode

/// <summary>
/// Listens for a right-click, raycasts, and queues a Harvest job
/// for the first ResourceNode hit.
/// Attach this to an empty “Systems” GameObject (the same one that
/// holds GlobalInventory).
/// </summary>
public class PlayerHarvestInput : MonoBehaviour
{
    void Update()
    {
        if (!Mouse.current.rightButton.wasReleasedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;

        var node = hit.collider.GetComponentInParent<ResourceNode>();
        if (node == null) return;

        JobSystem.Enqueue(new Job
        {
            type = JobType.Harvest,
            targetPos = node.transform.position,
            node = node.gameObject
        });
        Debug.Log($"[Input] Harvest job queued for {node.name}");
    }
}
