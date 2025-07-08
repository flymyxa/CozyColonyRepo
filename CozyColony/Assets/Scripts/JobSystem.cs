using System.Collections.Generic;
using UnityEngine;

namespace CozyWorld
{
    // Types of work colonists can perform.
    public enum JobType { Harvest, Cook, Eat }

    // Lightweight descriptor a ColonistAgent can execute.
    public struct Job
    {
        public JobType type;       // what to do
        public Vector3 targetPos;  // where to go
        public ResourceDef resource; // item involved (optional)
        public GameObject node;      // world object involved (optional)
    }

    /// <summary>
    /// Global static queue: anything can enqueue jobs, colonists dequeue.
    /// </summary>
    public static class JobSystem
    {
        private static readonly Queue<Job> queue = new();

        public static void Enqueue(Job job) => queue.Enqueue(job);

        public static bool TryDequeue(out Job job)
        {
            if (queue.Count == 0)
            {
                job = default;
                return false;
            }

            job = queue.Dequeue();
            return true;
        }
    }
}
