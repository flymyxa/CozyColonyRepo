using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

/// <summary>Singleton that owns the vertical orders toolbar and stores current selection.</summary>
public class OrdersPalette : MonoBehaviour
{
    public static OrdersPalette Instance { get; private set; }

    [SerializeField] private List<OrderButton> buttons;
    public OrderType CurrentOrder { get; private set; } = OrderType.None;
    public event Action<OrderType> OnOrderSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        foreach (var b in buttons) b.Init(this);
    }

    public void Select(OrderType type)
    {
        CurrentOrder = type;
        OnOrderSelected?.Invoke(type);
    }

    private void Update()        // 1-4 = tools, Esc = deselect
    {
        var k = Keyboard.current;
        if (k.digit1Key.wasPressedThisFrame) Select(OrderType.Harvest);
        else if (k.digit2Key.wasPressedThisFrame) Select(OrderType.CutTrees);
        else if (k.digit3Key.wasPressedThisFrame) Select(OrderType.Mine);
        else if (k.digit4Key.wasPressedThisFrame) Select(OrderType.Cancel);
        else if (k.escapeKey.wasPressedThisFrame) Select(OrderType.None);
    }
}

public enum OrderType { None, Harvest, CutTrees, Mine, Cancel }
