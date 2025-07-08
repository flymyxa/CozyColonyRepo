using UnityEngine;

namespace CozyWorld
{
    /// <summary>Tracks core needs; ticks down in Update.</summary>
    public class NeedsComponent : MonoBehaviour
    {
        [Range(0, 100)] public int hunger = 100;
        [Range(0, 100)] public int fun = 100;

        [Tooltip("Points per in‑game hour (default 1 real second = 1 in‑game minute)")]
        public int hungerDecayPerHour = 3;
        public int funDecayPerHour = 2;

        private float _timer;
        private const float REAL_SECONDS_PER_GAME_HOUR = 60f; // tweak for faster decay

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= REAL_SECONDS_PER_GAME_HOUR)
            {
                hunger = Mathf.Max(0, hunger - hungerDecayPerHour);
                fun = Mathf.Max(0, fun - funDecayPerHour);
                _timer -= REAL_SECONDS_PER_GAME_HOUR;
            }
        }

        public void Eat(int amount) => hunger = Mathf.Min(100, hunger + amount);
        public void Play(int amount) => fun = Mathf.Min(100, fun + amount);
    }
}
