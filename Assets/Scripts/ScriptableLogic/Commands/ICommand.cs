using Cysharp.Threading.Tasks;
using System;

namespace ScriptableLogic.Commands
{
    public interface ICommand<T>
    {
        public Type GetArgumentType() => typeof(T);
            
        public UniTask Execute(T args);
    }
        
    public interface ICommand
    {
        public UniTask Execute();
    }
        
    public interface INonExecutableCommand {}
}