using UnityEngine;

[RequireComponent(typeof(TaskCycle))]

public sealed class TaskCycleShaker : Shaker
{
    private void Awake()
    {
        Initialize();
        GetComponent<TaskCycle>().TaskPerformed += Shake;
    }
}
