public sealed class CanPerformTaskCondition : Condition, ICondition
{
    public bool GetValue()
    {
        return Args.Owner.ComponentsContainer.Get<TaskCycle>().IsPossibleToPerformTask();
    }
}