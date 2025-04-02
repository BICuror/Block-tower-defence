using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public sealed class EntityEffectRemovalHandler
{
    private CancellationTokenSource _cancelationTokenSource = new();
    private Type _effectType;
    
    public Action<Type> EffectRemovalTimerFinished;

    public EntityEffectRemovalHandler(Type effectType)
    {
        _effectType = effectType;
    }
    
    public void SetRemovalTimer(float duration)
    {
        _cancelationTokenSource.Cancel();
        _cancelationTokenSource.Dispose();
        WaitToRemoveEffect(duration);
    }
    
    private async UniTask WaitToRemoveEffect(float duration)
    {
        try
        {
            await UniTask.WaitForSeconds(duration);
        }
        catch (Exception e) { TaskUtility.LogAsync(e); }
        
        EffectRemovalTimerFinished.Invoke(_effectType);
    }

    public void StopRemovalTimer()
    {
        _cancelationTokenSource.Cancel();
    }
}