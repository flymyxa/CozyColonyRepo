using System.Collections.Generic;
using UnityEngine;

/// <summary>Listens for SelectionBox events and mutates the JobSystem.</summary>
public class OrderDispatcher : MonoBehaviour
{
    private void OnEnable() => SelectionBox.SelectionIssued += Handle;
    private void OnDisable() => SelectionBox.SelectionIssued -= Handle;

    private void Handle(OrderType order, List<GameObject> targets)
    {
        if (targets == null || targets.Count == 0) return;

        // Cancel tool simply removes matching jobs
        if (order == OrderType.Cancel)
        {
            JobSystem.Remove(j => j != null && targets.Contains(j.node));
            return;
        }

        // Map order → job type
        JobType jobType = order switch
        {
            OrderType.Harvest => JobType.Harvest,
            OrderType.CutTrees => JobType.Chop,
            OrderType.Mine => JobType.Mine,
            _ => JobType.Harvest
        };

        foreach (var t in targets)
        {
            JobSystem.Enqueue(new Job { node = t, type = jobType });
        }
    }
}
