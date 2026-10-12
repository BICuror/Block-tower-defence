using System.Collections.Generic;
using UnityEngine;
using Configs;
using Combat;

public abstract class TypedExecutor<T>
{
    private Dictionary<Command, ICommand> _cachedCommands = new();
    private Dictionary<Command, ICommand<T>> _cachedTypedCommands = new();
    private Dictionary<Condition, ICondition> _cachedConditions = new();
    private Dictionary<Condition, ICondition<T>> _cachedTypedConditions = new();

    public void ExecuteCommands(IEnumerable<Command> commands, T dynamicArg)
    {
        foreach (Command command in commands)
        {
            if (!ExecuteConditions(command.ConditionalContainer, dynamicArg)) continue;
            
            if (command is ICommand<T>)
            {
                if (!_cachedTypedCommands.ContainsKey(command)) _cachedTypedCommands.Add(command, command as ICommand<T>);
                
                _cachedTypedCommands[command].Execute(dynamicArg);
            }
            else
            {
                if (!_cachedCommands.ContainsKey(command)) _cachedCommands.Add(command, command as ICommand);

                _cachedCommands[command].Execute();
            }
        }
    }
    
    public bool ExecuteConditions(ConditionalContainer conditionalContainer, T dynamicArg)
    {
        foreach (Condition condition in conditionalContainer.Conditions)
        {
            bool value;

            if (condition is ICondition<T>)
            {
                if (!_cachedTypedConditions.ContainsKey(condition)) _cachedTypedConditions.Add(condition, condition as ICondition<T>);

                value = _cachedTypedConditions[condition].GetValue(dynamicArg);
            }
            else
            {
                if (!_cachedConditions.ContainsKey(condition)) _cachedConditions.Add(condition, condition as ICondition);

                value = _cachedConditions[condition].GetValue();
            }
            
            if (!value && conditionalContainer.Entry.RequiredConditionLogic == ConditionLogic.All) return false;
            
            if (value && conditionalContainer.Entry.RequiredConditionLogic == ConditionLogic.Any) return true;
        }

        return true;
    }
}

public sealed class TypedExecutor
{
    private Dictionary<Command, ICommand> _cachedCommands = new();
    private Dictionary<Condition, ICondition> _cachedConditions = new();
    
    public void ExecuteCommands(IEnumerable<Command> commands)
    {
        foreach (Command command in commands)
        {
            if (!_cachedCommands.ContainsKey(command)) _cachedCommands.Add(command, command as ICommand);
            
            _cachedCommands[command].Execute();
        }
    }
    
    public bool ExecuteConditions(ConditionalContainer conditionalContainer)
    {
        foreach (Condition condition in conditionalContainer.Conditions)
        {
            if (!_cachedConditions.ContainsKey(condition)) _cachedConditions.Add(condition, condition as ICondition);
            
            bool value = _cachedConditions[condition].GetValue();

            if (!value && conditionalContainer.Entry.RequiredConditionLogic == ConditionLogic.All) return false;
            
            if (value && conditionalContainer.Entry.RequiredConditionLogic == ConditionLogic.Any) return true;
        }

        return true;
    }
}

public sealed class VectorTypedExecutor : TypedExecutor<Vector3> {}
public sealed class EntityTypedExecutor : TypedExecutor<CombatEntity> {}
public sealed class FloatTypedExecutor : TypedExecutor<float> {}
public sealed class IntTypedExecutor : TypedExecutor<int> {}