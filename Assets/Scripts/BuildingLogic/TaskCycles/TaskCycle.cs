using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using System;
using Combat;

public class TaskCycle : MonoBehaviour
{
    [Cached] private EntityHealth _ownerEntityHealth;
    [Cached] private TaskRechargeDuration _taskRechargeDuration;
    [Cached] private ITaskConditionProvider _taskConditionProvider;
    private bool _taskCycleIsActive;
    private CancellationTokenSource _cancellationTokenSource = new();
    
    public Action TaskPerformed;

    private void Start()
    {
        _ownerEntityHealth.EntityDied += _ => StopRechargeProcess();
    }
    
    public void StopRechargeProcess()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    public void TryCycle()
    {
        if (!CanWork()) return; 
        
        if (!_taskConditionProvider.GetTaskCondition().Invoke()) return;
        
        if (_taskCycleIsActive) return;

        StartRechargeProcess();
    }
    
    protected virtual bool CanWork() => true;
    
    private async void StartRechargeProcess()
    {
        _taskCycleIsActive = true;

        try
        {
            await UniTask.WaitForSeconds(_taskRechargeDuration.Value, cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            _taskCycleIsActive = false;
            TaskUtility.LogAsync(e);
            return;
        }
        
        _taskCycleIsActive = false;

        if (_taskConditionProvider.GetTaskCondition().Invoke())
        {
            TryCycle();
            PerformTask();
        }
    }

    public void PerformTask()
    {
        if (!CanWork()) return;
        
        if (!_taskConditionProvider.GetTaskCondition().Invoke()) return; 
        
        TaskPerformed?.Invoke();
    }
}