using UnityEngine;

/// <summary>Enumerates every job the colony currently recognises.</summary>
public enum JobType
{
    Harvest,   // gather berries / general crops
    Chop,      // fell trees for wood
    Mine,      // dig ore deposits
    Cook,      // prepare meals at a stove
    Eat,       // consume a cooked meal
    Play       // leisure activity (e.g. chess table)
}

/// <summary>Plain-data container representing a single queued task.</summary>
public class Job
{
    /// <summary>World-space target (used by path-finding).</summary>
    public Vector3 targetPos;

    /// <summary>Optional scene object this job acts on (tree, ore, seat…).</summary>
    public GameObject node;

    /// <summary>The actual work to perform when the colonist arrives.</summary>
    public JobType type;
}
