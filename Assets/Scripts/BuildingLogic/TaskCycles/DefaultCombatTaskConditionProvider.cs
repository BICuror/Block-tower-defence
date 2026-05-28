using UnityEngine;
using Cashing;

namespace Combat
{
    public class DefaultCombatTaskConditionProvider : MonoBehaviour, ITaskConditionProvider
    {
        [Cached] private AreaEntityDetector _scaner;
        [Cached] private TaskCycle _taskCycle;

        protected void Start()
        {
            _scaner.AddedItem += _ => _taskCycle.TryCycle();
        }
        
        public ResolveTaskCondition GetTaskCondition()
        {
            return ResloveTaskCondition;
        }

        private bool ResloveTaskCondition() => _scaner.IsEmpty == false;
    }
}