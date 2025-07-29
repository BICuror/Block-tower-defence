using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using System;
using Combat;

public class TaskCycle : MonoBehaviour
{
    [Cached] private ITaskConditionProvider _taskConditionProvider;
    [Cached] private TaskRechargeDuration _taskRechargeDuration;
    [Cached] private EntityHealth _ownerEntityHealth;
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _taskCycleIsActive;
    private int _taskBlockStack;
    
    public Action TaskPerformed;
    public Action TaskCycled;

    private void Start()
    {
        _ownerEntityHealth.Died += StopRechargeProcess;
    }

    public void AddBlockStack() => _taskBlockStack++;

    public void RemoveBlockStack()
    {
        if (_taskBlockStack <= 0) Debug.LogError("Trying to remove block stack, while block stack is empty");

        _taskBlockStack--;
    }
    
    public void TryCycle()
    {
        if (!CanWork()) return; 
        
        if (!_taskConditionProvider.GetTaskCondition().Invoke()) return;
        
        if (_taskCycleIsActive) return;

        StartRechargeProcess();
    }
    
    public void PerformTask()
    {
        if (!CanWork()) return;
        
        if (!_taskConditionProvider.GetTaskCondition().Invoke()) return; 
        
        TaskPerformed?.Invoke();
    }
    
    protected virtual bool CanWork() => true;
    
    protected void StopRechargeProcess()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }
    
    protected void OnDestroy()
    {
        _ownerEntityHealth.Died += StopRechargeProcess;
    }
    
    private async void StartRechargeProcess()
    {
        _taskCycleIsActive = true;

        try
        {
            float elapsedTime = 0;
                
            while (elapsedTime < _taskRechargeDuration.Value)
            {
                await UniTask.WaitForFixedUpdate(cancellationToken: _cancellationTokenSource.Token);
                  
                if (_taskBlockStack <= 0) elapsedTime += Time.fixedDeltaTime;
            }
        }
        catch (Exception e)
        {
            _taskCycleIsActive = false;
            e.LogAsync();
            return;
        }
        
        _taskCycleIsActive = false;

        if (_taskConditionProvider.GetTaskCondition().Invoke())
        {
            TaskCycled?.Invoke();
            TryCycle();
            PerformTask();
        }
    }
}