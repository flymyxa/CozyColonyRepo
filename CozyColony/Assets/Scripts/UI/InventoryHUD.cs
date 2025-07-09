using CozyWorld;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>Clones a label for every ResourceDef and updates counts via GlobalInventory.</summary>
public class InventoryHUD : MonoBehaviour
{
    [SerializeField] TMP_Text template;          // drag the disabled “Template” text
    readonly Dictionary<ResourceDef, TMP_Text> labels = new();

    void Start()
    {
        foreach (var def in Resources.LoadAll<ResourceDef>("SO_Definitions/Resources"))
        {
            var t = Instantiate(template, template.transform.parent);
            t.gameObject.SetActive(true);
            t.text = $"{def.displayName}: 0";
            labels[def] = t;
        }
        template.gameObject.SetActive(false);

        GlobalInventory.Instance.OnInventoryChanged += UpdateLabel;

        // initialise with existing values (domain reload friendly)
        foreach (var kv in labels)
            UpdateLabel(kv.Key, GlobalInventory.Instance.Get(kv.Key));
    }

    void OnDestroy() =>
        GlobalInventory.Instance.OnInventoryChanged -= UpdateLabel;

    void UpdateLabel(ResourceDef def, int qty)
    {
        if (labels.TryGetValue(def, out var t))
            t.text = $"{def.displayName}: {qty}";
    }
}
