using UnityEngine;

namespace Combat.Animation
{
    [RequireComponent(typeof(TaskCycle))]
    
    public sealed class TaskCycleShaker : Shaker
    {
        private void Awake()
        {
            Initialize();
            GetComponent<TaskCycle>().TaskPerformed += Shake;
        }
    }
}