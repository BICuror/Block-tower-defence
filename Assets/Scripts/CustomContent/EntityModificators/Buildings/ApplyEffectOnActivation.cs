using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Combat;

public sealed class ApplyEffectOnActivation : EntityModificator
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _isActive;

    public override bool CanBeApplied() => Entity.StatContainer.Has<TaskRechargeDuration>();
    
    public override void Enable()
    {
        Entity.Activated += Activate;
    }

    private void Activate()
    {
        if (_isActive) return;
        
        _isActive = true;
        
        Entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(Type.GetType(Args.GetArgument<string>("EffectType")), Args.GetArgument<int>("Stacks"));
        StartCountdownToDeactivation().Forget();
        
    }
    private async UniTask StartCountdownToDeactivation()
    { 
        await UniTask.WaitForSeconds(Args.GetArgument<float>("Duration"), cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow();
        
        Deactivate();
    }
    
    private void Deactivate()
    {
        _isActive = false;
        
        Entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(Type.GetType(Args.GetArgument<string>("EffectType")), Args.GetArgument<int>("Stacks"));
    }

    public override void Disable()
    {
        _cancellationTokenSource.Cancel();
        
        Entity.Activated -= Activate;
    }
}