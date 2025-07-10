using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>Vertical toolbar; auto-registers OrderButton children.</summary>
public class OrdersPalette : MonoBehaviour
{
    public static OrdersPalette Instance { get; private set; }

    public OrderType CurrentOrder { get; private set; } = OrderType.None;
    public event Action<OrderType> OnOrderSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // automatically hook every child OrderButton
        foreach (var btn in GetComponentsInChildren<OrderButton>(true))
            btn.Init(this);

        Debug.Log("Palette hooked \" + GetComponentsInChildren<OrderButton>().Length + \" buttons");
    }

    private void Update()
    {
        var k = Keyboard.current;
        if (k.digit1Key.wasPressedThisFrame) Select(OrderType.Harvest);
        else if (k.digit2Key.wasPressedThisFrame) Select(OrderType.CutTrees);
        else if (k.digit3Key.wasPressedThisFrame) Select(OrderType.Mine);
        else if (k.digit4Key.wasPressedThisFrame) Select(OrderType.Cancel);
        else if (k.digit5Key.wasPressedThisFrame) Select(OrderType.BuildHut);
        else if (k.escapeKey.wasPressedThisFrame) Select(OrderType.None);
    }

    public void Select(OrderType type)
    {
        CurrentOrder = type;
        OnOrderSelected?.Invoke(type);
    }
}

public enum OrderType { None, Harvest, CutTrees, Mine, BuildHut, Cancel }
