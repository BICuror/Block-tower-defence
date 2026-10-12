using Cysharp.Threading.Tasks;
using UnityEngine;
using Configs;

public sealed class DebugCommand : Command, ICommand
{
    [Argument("Message")] private string _message;

    public UniTask Execute()
    {
        Debug.Log($"Debug: {_message}");
        
        return UniTask.CompletedTask;
    }
}