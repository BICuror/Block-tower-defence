using UnityEngine;

[RequireComponent(typeof(TaskCycle))]

public sealed class TaskCycleShaker : Shaker
{
    private void Awake()
    {
        CaptureDefaultScale();

        GetComponent<TaskCycle>().TaskPerformed.AddListener(Shake);
    }
}
