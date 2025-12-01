using Cysharp.Threading.Tasks;
using System.Threading;
using Combat;
using System;

public abstract class EntityTickEffect : EntityEffect
{
    private CancellationTokenSource _cancelationTokenSource = new();

    private float _secondsPerTick;

    protected override void OnInitialized()
    {
        _secondsPerTick = ArgumentsContainer.GetArgument<float>("SecondsPerTick");
    }

    public override void Update() {}

    public override void ApplyToEntity()
    {
        InvokeTickEffect();
    }

    public override void RemoveFromEntity()
    {
        _cancelationTokenSource.Cancel();
    }

    private async UniTask InvokeTickEffect()
    {
        try
        {
            await UniTask.WaitForSeconds(_secondsPerTick, cancellationToken: _cancelationTokenSource.Token);
        }
        catch (Exception e)
        {
            e.LogAsync();
            return;
        }
        
        Tick();
        
        InvokeTickEffect();
    }

    protected abstract void Tick();
}