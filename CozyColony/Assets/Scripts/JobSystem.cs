using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Static FIFO queue that colonist brains pull work from.
/// Includes legacy helpers so older scripts keep compiling.
/// </summary>
public static class JobSystem
{
    /* ---------- internal queue ---------- */

    private static readonly Queue<Job> queue = new();

    /* ---------- basic queue ops ---------- */

    public static int Count => queue.Count;

    public static void Enqueue(Job job)
    {
        if (job == null)
        {
            Debug.LogWarning("Tried to enqueue a null Job.");
            return;
        }
        queue.Enqueue(job);
    }

    public static Job Dequeue() => queue.Count > 0 ? queue.Dequeue() : null;

    public static Job Peek() => queue.Count > 0 ? queue.Peek() : null;

    public static void Clear() => queue.Clear();

    /* ---------- legacy helpers ---------- */

    /// <summary>Returns true if *any* queued job matches the requested type.</summary>
    public static bool HasPending(JobType type) => queue.Any(j => j.type == type);

    /// <summary>
    /// New signature: tries to dequeue the first job of the requested type.
    /// Returns true + job if found, otherwise false.
    /// </summary>
    public static bool TryDequeue(JobType type, out Job job)
    {
        job = null;
        if (queue.Count == 0) return false;

        var tmp = new Queue<Job>();
        bool found = false;

        while (queue.Count > 0)
        {
            var j = queue.Dequeue();
            if (!found && j.type == type)
            {
                job = j;
                found = true;
                continue;                    // don’t requeue the consumed job
            }
            tmp.Enqueue(j);
        }
        while (tmp.Count > 0) queue.Enqueue(tmp.Dequeue());
        return found;
    }

    /// <summary>
    /// **Legacy overload** for older code that expected a direct Job return.
    /// Returns the first matching job or null. Internally calls the new signature.
    /// </summary>
    public static Job TryDequeue(JobType type)
    {
        TryDequeue(type, out Job job);
        return job;
    }

    /* ---------- cancel & priority helpers (Orders Palette) ---------- */

    public static void Remove(Predicate<Job> match)
    {
        if (queue.Count == 0) return;

        var tmp = new Queue<Job>();
        while (queue.Count > 0)
        {
            var j = queue.Dequeue();
            if (!match(j)) tmp.Enqueue(j);
        }
        while (tmp.Count > 0) queue.Enqueue(tmp.Dequeue());
    }

    public static void Insert(int index, Job job)
    {
        if (job == null) return;
        if (index <= 0 || index >= queue.Count) { Enqueue(job); return; }

        var tmp = new Queue<Job>();
        int i = 0;
        while (queue.Count > 0)
        {
            if (i++ == index) tmp.Enqueue(job);
            tmp.Enqueue(queue.Dequeue());
        }
        while (tmp.Count > 0) queue.Enqueue(tmp.Dequeue());
    }
}
