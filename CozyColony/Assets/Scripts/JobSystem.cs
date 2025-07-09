using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Central FIFO queue all colonist AI pulls work from.
/// Includes every legacy helper the older scripts expect,
/// plus new helpers used by the Orders-Palette feature.
/// </summary>
public static class JobSystem
{
    /* ---------- backing store ---------- */

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

    /* ---------- legacy helpers (still used by NeedsComponent, ColonistAgent, etc.) ---------- */

    /// <summary>True if any queued job matches the requested type.</summary>
    public static bool HasPending(JobType type) => queue.Any(j => j.type == type);

    /// <summary>
    /// NEW signature – dequeue the first job of the requested type.
    /// </summary>
    public static bool TryDequeue(JobType type, out Job job)
    {
        job = null;
        if (queue.Count == 0) return false;

        var temp = new Queue<Job>();
        bool done = false;

        while (queue.Count > 0)
        {
            var j = queue.Dequeue();
            if (!done && j.type == type)
            {
                job = j;
                done = true;          // consume this one
                continue;
            }
            temp.Enqueue(j);
        }
        while (temp.Count > 0) queue.Enqueue(temp.Dequeue());
        return done;
    }

    /// <summary>
    /// LEGACY overload – older code called <c>TryDequeue(out job)</c>.
    /// Equivalent to “give me the next job, whatever it is”.
    /// </summary>
    public static bool TryDequeue(out Job job)
    {
        if (queue.Count == 0)
        {
            job = null;
            return false;
        }
        job = queue.Dequeue();
        return true;
    }

    /// <summary>
    /// Another legacy convenience overload that just returns the job
    /// (or null) without a boolean flag.
    /// </summary>
    public static Job TryDequeue(JobType type)
    {
        TryDequeue(type, out Job job);
        return job;
    }

    /* ---------- cancel & priority helpers (Orders Palette) ---------- */

    /// <summary>Remove every queued job whose predicate returns true.</summary>
    public static void Remove(Predicate<Job> match)
    {
        if (queue.Count == 0) return;

        var temp = new Queue<Job>();
        while (queue.Count > 0)
        {
            var j = queue.Dequeue();
            if (!match(j)) temp.Enqueue(j);
        }
        while (temp.Count > 0) queue.Enqueue(temp.Dequeue());
    }

    /// <summary>
    /// Insert a job at a specific index (used for priority layering).
    /// Falls back to <see cref="Enqueue"/> if index is out of range.
    /// </summary>
    public static void Insert(int index, Job job)
    {
        if (job == null) return;
        if (index <= 0 || index >= queue.Count) { Enqueue(job); return; }

        var temp = new Queue<Job>();
        int i = 0;
        while (queue.Count > 0)
        {
            if (i++ == index) temp.Enqueue(job);
            temp.Enqueue(queue.Dequeue());
        }
        while (temp.Count > 0) queue.Enqueue(temp.Dequeue());
    }
}
