using UnityEngine;
using TMPro;

/// <summary>Handles resource delivery and transformation into a finished hut.</summary>
public class BuildingBlueprint : MonoBehaviour
{
    [Header("Requirements")]
    public int logsNeeded = 20;
    public int berriesNeeded = 5;

    private int logsDelivered;
    private int berriesDelivered;

    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI progressText;

    /* ---------- Public API ---------- */

    public void Deliver(string resourceName, int qty)
    {
        if (resourceName == "Log") logsDelivered += qty;
        if (resourceName == "Berry") berriesDelivered += qty;
        UpdateUI();
        if (IsComplete()) FinishConstruction();
    }

    /* ---------- Internal ---------- */

    private void Start() => UpdateUI();

    private bool IsComplete() =>
        logsDelivered >= logsNeeded &&
        berriesDelivered >= berriesNeeded;

    private void FinishConstruction()
    {
        var hut = Instantiate(Resources.Load<GameObject>("Prefabs/Hut"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        progressText.text = $"{logsDelivered}/{logsNeeded} Logs\n{berriesDelivered}/{berriesNeeded} Berries";
    }
}
