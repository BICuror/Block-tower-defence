namespace Combat
{
    public interface ITaskConditionProvider
    {
        public ResolveTaskCondition GetTaskCondition();
    }
    
    public delegate bool ResolveTaskCondition();
}