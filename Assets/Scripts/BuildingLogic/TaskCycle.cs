using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using System;

public class TaskCycle
{
    [Cached] private TaskRecharge _taskRecharge;
    public delegate bool ShouldWork(); 
    public ShouldWork ShouldWorkDelegate;
    private bool _taskCycleIsActive;
    private CancellationTokenSource _cancellationTokenSource;
    
    public Action TaskPerformed;
    
    public void StopRechargeProcess()
    {
        _taskCycleIsActive = false;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }
    
    public void StartCycle() => Recharge();
    
    private void Recharge()
    {
        if (CanWork() && ShouldWorkDelegate() && _taskCycleIsActive == false)
        {
            _taskCycleIsActive = true;
            StartRechargeProcess();
        }
    }
    
    protected virtual bool CanWork() => true;
    
    private async void StartRechargeProcess()
    {
        await UniTask.WaitForSeconds(_taskRecharge.Value, cancellationToken: _cancellationTokenSource.Token);
        _taskCycleIsActive = false;

        if (ShouldWorkDelegate())
        {
            Recharge();
            PerformTask();
        }
    }
    
    private void PerformTask() => TaskPerformed?.Invoke();
}