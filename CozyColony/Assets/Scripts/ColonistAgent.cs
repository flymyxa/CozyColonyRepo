using UnityEngine;
using UnityEngine.AI;

namespace CozyWorld
{
    /// <summary>Very simple agent: grabs a job, walks to target, executes.</summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ColonistAgent : MonoBehaviour
    {
        private NavMeshAgent _nav;
        private NeedsComponent _needs;
        private Job _currentJob;
        private bool _hasJob;

        private void Awake()
        {
            _nav = GetComponent<NavMeshAgent>();
            _needs = GetComponent<NeedsComponent>();
        }

        private void Update()
        {
            // Get a job if idle
            if (!_hasJob && JobSystem.TryDequeue(out _currentJob))
            {
                _nav.SetDestination(_currentJob.targetPos);
                _hasJob = true;
            }

            if (!_hasJob) return;

            // Arrived?
            if (!_nav.pathPending && _nav.remainingDistance <= _nav.stoppingDistance)
            {
                ExecuteJob(_currentJob);
                _hasJob = false;
            }
        }

        private void ExecuteJob(Job job)
        {
            switch (job.type)
            {
                case JobType.Harvest:
                    // simplistic: instantly collect yieldPerHit
                    if (job.node && job.node.TryGetComponent<ResourceNode>(out var node))
                    {
                        node.HarvestOnce(this);
                    }
                    break;
                case JobType.Cook:
                    // cooking logic placeholder
                    break;
                case JobType.Eat:
                    _needs.Eat(25);
                    break;
            }
        }
    }
}