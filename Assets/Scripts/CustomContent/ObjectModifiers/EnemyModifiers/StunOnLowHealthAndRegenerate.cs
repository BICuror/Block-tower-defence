using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Combat;

public sealed class StunOnLowHealthAndRegenerate : EntityModificator
{
    private const float HEAL_STEP_DURATION = 0.5f;

    private CancellationTokenSource _cancellationTokenSource = new();
    
    private float _healthActivationThreshold;
    private bool _oncePerEntityModifier;
    private float _healPercentage;
    private float _duration;

    private bool _isActive;
    
    public override void Enable()
    {
        _healthActivationThreshold = Args.GetArgument<float>("HealthActivationThreshold");
        _oncePerEntityModifier = Args.GetArgument<bool>("OncePerEntityModifier");
        _healPercentage = Args.GetArgument<float>("HealPercentage");
        _duration = Args.GetArgument<float>("Duration");

        Entity.Health.Died += StopHealthRegeneration;
        Entity.Health.Damaged += TryActivate;
    }

    private void TryActivate()
    {
        if (_isActive) return;
        
        if (_healthActivationThreshold < Entity.Health.GetHpPercent()) return;
        
        StartHealthRegeneration().Forget();
    }

    private async UniTask StartHealthRegeneration()
    {
        _isActive = true;
        
        Entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(typeof(StunEffect), 1, _duration);
        Entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(typeof(InvincibilityEffect), 1, _duration);

        float elapsedTime = 0f;

        float stepHeal = Entity.Health.GetMaxHp() * (_healPercentage / (_duration / HEAL_STEP_DURATION));

        while (elapsedTime < _duration)
        {
            Entity.Health.ReceiveHeal(stepHeal, Entity);

            try
            {
                await UniTask.WaitForSeconds(HEAL_STEP_DURATION, cancellationToken:_cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                e.LogAsync();
                break;
            }
            
            elapsedTime += HEAL_STEP_DURATION;
        }

        if (!_oncePerEntityModifier) _isActive = false;
    }

    private void StopHealthRegeneration()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
        
        Entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(typeof(StunEffect), 1);
    }
    
    public override void Disable()
    {
        Entity.Health.Died -= StopHealthRegeneration;
        Entity.Health.Damaged -= TryActivate;

        if (_isActive) StopHealthRegeneration();
    }
}