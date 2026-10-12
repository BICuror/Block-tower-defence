using System.Collections.Generic;
using Configs;

public sealed class ConditionalContainer
{
    private readonly ConditionalExecutionEntry _entry;
    private readonly List<Condition> _conditions;
    
    public IReadOnlyCollection<Condition> Conditions => _conditions;
    public ConditionalExecutionEntry Entry => _entry;

    public ConditionalContainer(List<Condition> conditions, ConditionalExecutionEntry entry)
    {
        _conditions = conditions;
        _entry = entry;
    }

    public IEnumerable<object> GetAllInjectableObjects()
    {
        foreach (Condition condition in _conditions)
        {
            yield return condition;
        }
    }
}