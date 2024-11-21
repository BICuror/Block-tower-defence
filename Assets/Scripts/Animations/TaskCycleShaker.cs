using UnityEngine;

[RequireComponent(typeof(TaskCycle))]

public sealed class TaskCycleShaker : Shaker
{
    private void Awake()
    {
        GetComponent<TaskCycle>().TaskPerformed += Shake;
    }
}
