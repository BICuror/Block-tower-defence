using Combat;

public sealed class EnemyTaskCycle : TaskCycle
{
    protected override void CreateTaskCycleCore()
    {
        EnemyEntity entity = transform.parent.GetComponent<EnemyEntity>();
        
        taskCycleCore = new EnemyCycleCore(entity, entity.StatContainer.Get<TaskRechargeDuration>());
    }
}