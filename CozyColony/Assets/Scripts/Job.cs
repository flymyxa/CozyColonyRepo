using UnityEngine;

/// <summary>All job categories the colony currently understands.</summary>
public enum JobType
{
    Harvest,   // gather berries / generic crops
    Chop,      // fell trees for wood
    Mine,      // dig ore
    Cook,      // prepare meals at a stove
    Eat,       // sit and consume a meal
    Play       // leisure activity (e.g. chess table)
}

/// <summary>Data-holder for queued work.</summary>
public class Job
{
    /// <summary>Target world position (used by path-finding).</summary>
    public Vector3 targetPos;

    /// <summary>Optional reference to the in-world object we’re acting on
    /// (tree, ore deposit, cooking station, etc.).</summary>
    public GameObject node;

    /// <summary>Kind of work to perform.</summary>
    public JobType type;
}
