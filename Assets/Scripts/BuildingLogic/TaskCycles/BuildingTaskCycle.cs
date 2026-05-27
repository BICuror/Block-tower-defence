using Cashing;
using Combat;

public sealed class BuildingTaskCycle : TaskCycle
{
    [Cached] private BuildingEntity _buildingEntityOwner;

    protected override void CreateTaskCycleCore() => taskCycleCore = new BuildingCycleCore(_buildingEntityOwner, _buildingEntityOwner.StatContainer.Get<TaskRechargeDuration>());
}