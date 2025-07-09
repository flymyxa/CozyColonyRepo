using UnityEngine;

namespace CozyWorld
{
    /// <summary>
    /// Tracks core needs (Hunger & Fun) and enqueues jobs when thresholds are reached.
    /// One real-time second ≈ one in-game minute by default.
    /// </summary>
    public class NeedsComponent : MonoBehaviour
    {
        [Header("Eating")]
        [Tooltip("Reference to Cooked Meal ResourceDef (optional; if left null, the script uses Resources.Load).")]
        public ResourceDef cookedMealDef;

        [Header("Current Need Values")]
        [Range(0, 100)] public int hunger = 100;
        [Range(0, 100)] public int fun = 100;

        [Header("Need Decay per In-Game Hour")]
        public int hungerDecayPerHour = 3;
        public int funDecayPerHour = 2;

        const float REAL_SECONDS_PER_GAME_HOUR = 60f;   // tweak for faster or slower simulation
        float _timer;

        void Update()
        {
            // Tick timer until one in-game hour elapses
            _timer += Time.deltaTime;
            if (_timer < REAL_SECONDS_PER_GAME_HOUR) return;
            _timer -= REAL_SECONDS_PER_GAME_HOUR;

            // Decay needs
            hunger = Mathf.Max(0, hunger - hungerDecayPerHour);
            fun = Mathf.Max(0, fun - funDecayPerHour);

            // When hunger low, a Cook/Eat job is queued elsewhere; handle Fun here
            if (fun <= 40 && !JobSystem.HasPending(JobType.Play))
                EnqueuePlayJob();
        }

        /// <summary>Consume one cooked meal (if available) and restore hunger.</summary>
        public void Eat(int amount = 25)
        {
            // Resolve the cooked-meal asset
            var mealDef = cookedMealDef ??
                          Resources.Load<ResourceDef>("SO_Definitions/Resources/CookedMeal_ResourceDef");
            if (mealDef == null) return;

            // Only eat if meal exists in global inventory
            if (!GlobalInventory.Instance.Remove(mealDef, 1)) return;
            hunger = Mathf.Min(100, hunger + amount);
        }

        /// <summary>Instantly restores Fun (called by Play job on completion).</summary>
        public void Play(int amount = 20) =>
            fun = Mathf.Min(100, fun + amount);

        /// <summary>Find nearest FunSeat and enqueue a Play job.</summary>
        void EnqueuePlayJob()
        {
            var seats = Object.FindObjectsByType<FunSeat>(FindObjectsSortMode.None);
            if (seats.Length == 0) return;

            FunSeat nearest = null;
            float bestDist = float.MaxValue;
            foreach (var seat in seats)
            {
                float d = Vector3.Distance(transform.position, seat.transform.position);
                if (d < bestDist) { bestDist = d; nearest = seat; }
            }
            if (nearest == null) return;

            JobSystem.Enqueue(new Job
            {
                type = JobType.Play,
                targetPos = nearest.transform.position,
                node = nearest.gameObject
            });
        }
    }
}
