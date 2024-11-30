using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using System;
using Combat;

public class TaskCycle : MonoBehaviour
{
    [Cached] private TaskRecharge _taskRecharge;
    [Cached] private DefaultCombatTaskConditionProvider _defaultCombatTaskConditionProvider;
    private bool _taskCycleIsActive;
    private CancellationTokenSource _cancellationTokenSource = new();
    
    public Action TaskPerformed;
    
    public void StopRechargeProcess()
    {
        _taskCycleIsActive = false;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    public void TryCycle()
    {
        if (!CanWork()) return; 
        
        if (!_defaultCombatTaskConditionProvider.GetTaskCondition().Invoke()) return;
        
        if (_taskCycleIsActive) return;

        StartRechargeProcess();
    }
    
    protected virtual bool CanWork() => true;
    
    private async void StartRechargeProcess()
    {
        _taskCycleIsActive = true;
        await UniTask.WaitForSeconds(_taskRecharge.Value, cancellationToken: _cancellationTokenSource.Token);
        _taskCycleIsActive = false;

        if (_defaultCombatTaskConditionProvider.GetTaskCondition().Invoke())
        {
            TryCycle();
            PerformTask();
        }
    }
    
    private void PerformTask() => TaskPerformed?.Invoke();
}