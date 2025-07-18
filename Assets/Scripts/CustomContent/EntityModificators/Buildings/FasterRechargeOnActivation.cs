using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Combat;

public sealed class FasterRechargeOnActivation : EntityModificator
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private EntityEffectParticleHandler _entityEffectParticleHandler;
    private StatModifier _statModifier;
    private bool _isActive;

    public override bool CanBeApplied() => Entity.StatContainer.Has<TaskRechargeDuration>();
    
    public override void Enable()
    {
        _statModifier = new StatModifier(multiplier: -Args.GetArgument<float>("Multiplier"));
        
        Entity.Activated += Activate;
    }

    private async UniTask StartCountdownToDeactivation()
    {
        try
        {
            await UniTask.WaitForSeconds(Args.GetArgument<float>("Duration"), cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e) { TaskUtility.LogAsync(e); }
        
        Deactivate();
    }

    private void Activate()
    {
        if (_isActive) return;
        
        _isActive = true;
        
        StartCountdownToDeactivation();

        _entityEffectParticleHandler = Entity.ComponentsContainer.Get<ParticleEffectManager>().ApplyCustomEffect(Args.GetArgument<EntityEffectParticleHandler>("EntityEffectParticleHandler"));
        Entity.StatContainer.Get<TaskRechargeDuration>().AddStatModifier(_statModifier);
    }
    
    private void Deactivate()
    {
        _isActive = false;
        Entity.ComponentsContainer.Get<ParticleEffectManager>().DestroyCustomEffect(_entityEffectParticleHandler);
        Entity.StatContainer.Get<TaskRechargeDuration>().RemoveStatModifier(_statModifier);
    }

    public override void Disable()
    {
        _cancellationTokenSource.Cancel();
        
        Entity.Activated -= Activate;
    }
}