using UnityEngine;

/// <summary>Enumerates every job the colony currently recognises.</summary>
public enum JobType
{
    Harvest,
    Chop,
    Mine,
    Build,
    Haul,
    Cook,
    Eat,
    Play
}

/// <summary>Plain-data container for queued work.</summary>
public class Job
{
    public Vector3 targetPos;   // world position (optional)
    public GameObject node;        // scene object acted on
    public JobType type;
    public int amount;      // used by Haul (logs carried)
}
