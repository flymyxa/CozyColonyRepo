using UnityEngine;
using UnityEngine.UI;

/// <summary>Lightweight bridge between a UI Button and the OrdersPalette.</summary>
[RequireComponent(typeof(Button))]
public class OrderButton : MonoBehaviour
{
    [SerializeField] private OrderType order;

    public void Init(OrdersPalette palette)
        => GetComponent<Button>().onClick.AddListener(() => palette.Select(order));
}
