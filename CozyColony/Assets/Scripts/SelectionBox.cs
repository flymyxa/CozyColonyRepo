using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Diagnostics;

/// <summary>Handles click-drag selection and fires an event with targets.</summary>
public class SelectionBox : MonoBehaviour
{
    [SerializeField] private RectTransform boxUI;     // assign the transparent Image
    [SerializeField] private LayerMask nodeMask;      // ResourceNode / Tree / Ore
    private Vector2 start;
    private bool active;

    public delegate void OnSelection(OrderType type, List<GameObject> targets);
    public static event OnSelection SelectionIssued;

    private void Update()
    {
        if (OrdersPalette.Instance == null) return;
        if (OrdersPalette.Instance.CurrentOrder == OrderType.None) return;

        var mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            active = true;
            start = mouse.position.ReadValue();
            boxUI.gameObject.SetActive(true);
        }
        else if (mouse.leftButton.isPressed && active)
        {
            var end = mouse.position.ReadValue();
            Utils.DrawScreenRect(boxUI, start, end);
        }
        else if (mouse.leftButton.wasReleasedThisFrame && active)
        {
            active = false;
            boxUI.gameObject.SetActive(false);

            var targets = PhysicsUtility.NodesInScreenRect(boxUI, nodeMask, 256); // cap to 256
            SelectionIssued?.Invoke(OrdersPalette.Instance.CurrentOrder, targets);
        }
    }
}
