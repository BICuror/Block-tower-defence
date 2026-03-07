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
    private TokenContainer _cycleBlockTokenContainer = new(false);
    private bool _taskCycleIsActive;
    
    public TokenContainer CycleBlockTokenContainer => _cycleBlockTokenContainer;
    
    public event Action TaskPerformed;
    public event Action TaskCycled;

    private void Start()
    {
        _ownerEntityHealth.Died += StopRechargeProcess;
    }
    
    public void TryCycle()
    {
        if (!IsPossibleToPerformTask()) return; 
        
        if (_taskCycleIsActive) return;

        StartRechargeProcess();
    }
    
    public void PerformTask()
    {
        if (!IsPossibleToPerformTask()) return;
        
        TaskPerformed?.Invoke();
    }

    public bool IsPossibleToPerformTask()
    {
        if (!_ownerEntityHealth.IsAlive()) return false;
        
        if (!CanWork()) return false;
        
        if (!_taskConditionProvider.GetTaskCondition().Invoke()) return false; 
        
        return true;
    }
    
    protected virtual bool CanWork() => true;
    
    public void StopRechargeProcess()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
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
                  
                if (_cycleBlockTokenContainer.IsEmpty) elapsedTime += Time.fixedDeltaTime;
            }
        }
        catch (Exception e)
        {
            _taskCycleIsActive = false;
            e.LogAsync();
            return;
        }
        
        _taskCycleIsActive = false;

        if (IsPossibleToPerformTask())
        {
            TryCycle();
            PerformTask();
            TaskCycled?.Invoke();
        }
    }
    
    protected void OnDestroy() 
    { 
        _ownerEntityHealth.Died -= StopRechargeProcess;
        StopRechargeProcess();
    }
}