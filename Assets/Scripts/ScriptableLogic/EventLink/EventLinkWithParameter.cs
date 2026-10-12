using System.Collections.Generic;
using Configs;
using System;

namespace EventLinks
{
    public abstract class EventLink : ArgumentsRequester, IDisposable
    {
        protected ScriptableLogicFactory.ScriptableLogicConstructorArgs Args;
        protected ConditionalContainer ConditionalContainer;
        protected IEnumerable<Command> Commands;

        public void Initialize(ScriptableLogicFactory.ScriptableLogicConstructorArgs args, IEnumerable<Command> commands, ConditionalContainer conditionalContainer, object executorObject)
        {
            Args = args;
            Commands = commands;
            ConditionalContainer = conditionalContainer;
            
            OnInitialize(executorObject);
        }

        protected abstract void OnInitialize(object executorObject);

        public IEnumerable<object> GetAllInjectableObjects()
        {
            foreach (Command command in Commands)
            {
                yield return command;

                yield return command.ConditionalContainer.GetAllInjectableObjects();
            }
        }
        
        public abstract void Enable();
        public abstract void Disable();
        
        public void Dispose() => Disable();
    }
    
    public abstract class EventLinkWithParameter<T> : EventLink
    {
        private TypedExecutor<T> _executor;
        
        protected override void OnInitialize(object executorObject)
        {
            _executor = executorObject as TypedExecutor<T>;
        }

        public override void Enable() => SubscribeToEvent(TryExecuteCommands);
        public override void Disable() => UnsubscribeToEvent(TryExecuteCommands);
    
        protected abstract void SubscribeToEvent(Action<T> action);
        protected abstract void UnsubscribeToEvent(Action<T> action);
        
        private void TryExecuteCommands(T args)
        {
            if (!_executor.ExecuteConditions(ConditionalContainer, args)) return;
            
            _executor.ExecuteCommands(Commands, args);
        }
    }
    
    public abstract class ParameterlessEventLink : EventLink
    {
        private TypedExecutor _executor;
        
        protected override void OnInitialize(object executorObject)
        {
            _executor = executorObject as TypedExecutor;
        }
    
        public override void Enable() => SubscribeToEvent(TryExecuteCommands);
        public override void Disable() => UnsubscribeToEvent(TryExecuteCommands);
        
        protected abstract void SubscribeToEvent(Action action);
        protected abstract void UnsubscribeToEvent(Action action);
        
        private void TryExecuteCommands()
        {
            if (!_executor.ExecuteConditions(ConditionalContainer)) return;
            
            _executor.ExecuteCommands(Commands);
        }
    }
}