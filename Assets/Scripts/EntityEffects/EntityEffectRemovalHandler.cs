using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public sealed class EntityEffectRemovalHandler
{
    private CancellationTokenSource _cancelationTokenSource = new();
    private int _temporaryStacks;
    private Type _effectType;
    
    public Action<Type, int> EffectRemovalTimerFinished;

    public EntityEffectRemovalHandler(Type effectType)
    {
        _effectType = effectType;
    }
    
    public void AddStacks(int count) => _temporaryStacks += count;
    
    public void SetRemovalTimer(float duration)
    {
        _cancelationTokenSource.Cancel();
        _cancelationTokenSource = new();
        WaitToRemoveEffect(duration);
    }
    
    private async UniTask WaitToRemoveEffect(float duration)
    {
        try
        {
            await UniTask.WaitForSeconds(duration, cancellationToken: _cancelationTokenSource.Token);
        }
        catch (Exception e)
        {
            e.LogAsync();
            return;
        }
        
        EffectRemovalTimerFinished?.Invoke(_effectType, _temporaryStacks);
    }

    public void StopRemovalTimer()
    {
        _cancelationTokenSource.Cancel();
    }
}