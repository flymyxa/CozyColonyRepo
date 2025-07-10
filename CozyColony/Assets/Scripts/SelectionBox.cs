using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>Handles click-drag marquee selection for resource orders.</summary>
public class SelectionBox : MonoBehaviour
{
    [SerializeField] private RectTransform boxUI;
    [SerializeField] private LayerMask nodeMask;
    private Vector2 dragStart;
    private bool dragging;

    public delegate void OnSelection(OrderType order, List<GameObject> targets);
    public static event OnSelection SelectionIssued;

    private void Update()
    {
        if (OrdersPalette.Instance == null) return;
        if (OrdersPalette.Instance.CurrentOrder == OrderType.None) return;

        var mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            dragStart = mouse.position.ReadValue();
            dragging = true;
            boxUI.gameObject.SetActive(true);
        }
        else if (mouse.leftButton.isPressed && dragging)
        {
            var dragEnd = mouse.position.ReadValue();
            Utils.DrawScreenRect(boxUI, dragStart, dragEnd);
        }
        else if (mouse.leftButton.wasReleasedThisFrame && dragging)
        {
            dragging = false;
            boxUI.gameObject.SetActive(false);

            var targets = PhysicsUtility.NodesInScreenRect(boxUI, nodeMask, 256);
            SelectionIssued?.Invoke(OrdersPalette.Instance.CurrentOrder, targets);
        }
    }
}
