using System.Collections.Generic;
using UnityEngine;

/// <summary>Converts SelectionBox output into concrete jobs (Harvest, Build…).</summary>
public class OrderDispatcher : MonoBehaviour
{
    private void OnEnable() => SelectionBox.SelectionIssued += HandleSelection;
    private void OnDisable() => SelectionBox.SelectionIssued -= HandleSelection;

    private void HandleSelection(OrderType order, List<GameObject> targets)
    {
        if (targets == null || targets.Count == 0) return;

        /* ---- Cancel tool ---- */
        if (order == OrderType.Cancel)
        {
            JobSystem.Remove(j => j != null && targets.Contains(j.node));
            return;
        }

        /* ---- Build Hut ---- */
        if (order == OrderType.BuildHut)
        {
            foreach (Vector3 pos in SelectionHelpers.CellCenters(targets))
            {
                var bp = Instantiate(Resources.Load<GameObject>("Prefabs/Blueprint_Hut"), pos, Quaternion.identity);
                JobSystem.Enqueue(new Job { node = bp, type = JobType.Build });
            }
            return;
        }

        /* ---- Resource orders ---- */
        JobType mapped = order switch
        {
            OrderType.Harvest => JobType.Harvest,
            OrderType.CutTrees => JobType.Chop,
            OrderType.Mine => JobType.Mine,
            _ => JobType.Harvest
        };

        foreach (var node in targets)
            JobSystem.Enqueue(new Job { node = node, type = mapped });
    }
}
