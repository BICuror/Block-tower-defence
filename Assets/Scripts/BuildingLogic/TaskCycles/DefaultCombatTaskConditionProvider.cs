using UnityEngine;
using Cashing;

namespace Combat
{
    public class DefaultCombatTaskConditionProvider : MonoBehaviour, ITaskConditionProvider
    {
        [Cached] private TaskCycle _taskCycle;
        [Cached] private EnemyAreaScaner _scaner;

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