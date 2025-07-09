using CozyWorld;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton dictionary that every script can query or mutate.
/// Fires an event whenever the quantity of a ResourceDef changes.
/// </summary>
public class GlobalInventory : MonoBehaviour
{
    public static GlobalInventory Instance { get; private set; }

    public event Action<ResourceDef, int> OnInventoryChanged;

    readonly Dictionary<ResourceDef, int> _data = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int Get(ResourceDef def) => _data.TryGetValue(def, out var qty) ? qty : 0;

    public void Add(ResourceDef def, int amount)
    {
        if (amount <= 0) return;
        _data.TryAdd(def, 0);
        _data[def] += amount;
        OnInventoryChanged?.Invoke(def, _data[def]);
    }

    public bool Remove(ResourceDef def, int amount)
    {
        if (Get(def) < amount) return false;
        _data[def] -= amount;
        OnInventoryChanged?.Invoke(def, _data[def]);
        return true;
    }
}
