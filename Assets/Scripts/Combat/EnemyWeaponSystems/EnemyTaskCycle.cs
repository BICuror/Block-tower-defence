using Cashing;
using Combat;

public sealed class EnemyTaskCycle : TaskCycle
{
    [Cached] private EnemyEntity _enemyEntityOwner;

    protected override void CreateTaskCycleCore() => taskCycleCore = new EnemyCycleCore(_enemyEntityOwner, _enemyEntityOwner.StatContainer.Get<TaskRechargeDuration>());
}