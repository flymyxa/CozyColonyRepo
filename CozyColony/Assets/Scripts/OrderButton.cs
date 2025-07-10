using UnityEngine;
using UnityEngine.UI;

/// <summary>Assign an icon + enum in Inspector; Init ties the click to palette.</summary>
[RequireComponent(typeof(Button))]
public class OrderButton : MonoBehaviour
{
    [SerializeField] private OrderType orderType;

    public void Init(OrdersPalette palette)
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => palette.Select(orderType));
    }
}
