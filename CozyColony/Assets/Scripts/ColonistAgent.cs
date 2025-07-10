using UnityEngine;

/// <summary>Very simplified colonist brain for the vertical slice.</summary>
public class ColonistAgent : MonoBehaviour
{
    private const float WalkSpeed = 2.0f;

    private Job currentJob;
    private Vector3 destination;
    private string haulingResource;
    private int haulingQty;

    private void Update()
    {
        /* ---------- Job acquisition ---------- */
        if (currentJob == null)
        {
            JobSystem.TryDequeue(out currentJob);
            if (currentJob == null) return;
            destination = currentJob.node != null ? currentJob.node.transform.position : currentJob.targetPos;
        }

        /* ---------- Movement ---------- */
        transform.position = Vector3.MoveTowards(transform.position, destination, WalkSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, destination) > 0.1f) return;

        /* ---------- Job execution ---------- */
        switch (currentJob.type)
        {
            case JobType.Build:
                // Arrived at blueprint: enqueue a Haul job for logs
                JobSystem.Enqueue(new Job { node = currentJob.node, type = JobType.Haul });
                currentJob = null;
                break;

            case JobType.Haul:
                ExecuteHaul();
                currentJob = null;
                break;

            case JobType.Harvest:
            case JobType.Chop:
            case JobType.Mine:
                Destroy(currentJob.node);   // placeholder gather action
                currentJob = null;
                break;

            default:
                currentJob = null;
                break;
        }
    }

    /* ---------- Helpers ---------- */

    private void ExecuteHaul()
    {
        var bp = currentJob.node.GetComponent<BuildingBlueprint>();
        if (bp == null) return;
        // For slice: assume 5 logs delivered instantly.
        bp.Deliver("Log", 5);
    }
}
