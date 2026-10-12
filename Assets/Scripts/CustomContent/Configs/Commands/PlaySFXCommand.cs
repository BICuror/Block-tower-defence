using Cysharp.Threading.Tasks;
using UnityEngine;
using CuroAudio;
using Configs;
using Combat;

public sealed class PlaySFXCommand : Command, ICommand, ICommand<CombatEntity>, ICommand<Vector3>
{
    [Argument("Audio")] private AudioEnum _audio;
    
    public UniTask Execute()
    {
        AudioSystem.PlaySFX(_audio);
        
        return UniTask.CompletedTask;
    }

    public UniTask Execute(CombatEntity args)
    {
        Execute(args.transform.position);
        
        return UniTask.CompletedTask;
    }

    public UniTask Execute(Vector3 args)
    {
        AudioSystem.PlaySFX(_audio, args);
        
        return UniTask.CompletedTask;
    }
}