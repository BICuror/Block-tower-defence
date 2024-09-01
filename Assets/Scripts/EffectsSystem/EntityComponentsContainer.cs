using UnityEngine.AI;

public sealed class EntityComponentsContainer 
{
    public readonly EntityHealth Health;

    public readonly TaskCycle Task;

    public readonly UnityEngine.AI.NavMeshAgent Agent;

    public EntityComponentsContainer(EntityHealth entityHealth, TaskCycle taskCycle, NavMeshAgent agent)
    {
        Health = entityHealth;

        Task = taskCycle; 

        Agent = agent;
    }

    public bool HasHealth() => Health != null;
    public bool HasTaskCycle() => Task != null;
    public bool HasNavMeshAgent() => Agent != null;
}
