using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;
using Combat;

public abstract class TaskCycle : MonoBehaviour
{
    protected TaskCycleCore taskCycleCore;
    
    public TokenContainer CycleBlockTokenContainer => taskCycleCore.CycleBlockTokenContainer;
    
    public event Action TaskPerformed;
    public event Action TaskCycled;
    
    private void Start()
    {
        CreateTaskCycleCore();
        taskCycleCore.TaskCycled = InvokeTaskCycled;
        taskCycleCore.TaskPerformed = InvokeTaskPerformed;
        
        taskCycleCore.Enable();
    }

    public void TryCycle() => taskCycleCore.TryCycle();
    public void PerformTask() => taskCycleCore.PerformTask();
    public void StopRechargeProcess() => taskCycleCore.StopRechargeProcess();
    public bool IsPossibleToPerformTask() => taskCycleCore.IsPossibleToPerformTask();
    
    private void OnEnable() => taskCycleCore?.Enable();
    private void OnDisable() => taskCycleCore?.Disable();
    
    private void InvokeTaskCycled() => TaskCycled?.Invoke();
    private void InvokeTaskPerformed() => TaskPerformed?.Invoke();

    protected abstract void CreateTaskCycleCore();
}

public sealed class EnemyCycleCore : TaskCycleCore
{
    private ITaskConditionProvider _taskConditionProvider;
    private TaskRechargeDuration _taskRechargeDuration;
    
    public override float TaskRechargeDuration => _taskRechargeDuration.Value;
    
    public EnemyCycleCore(EnemyEntity enemyEntity, TaskRechargeDuration taskRechargeDuration) : base(enemyEntity)
    {
        _taskConditionProvider = enemyEntity.ComponentsContainer.Get<ITaskConditionProvider>();
        _taskRechargeDuration = taskRechargeDuration;
    }
    
    protected override bool CanWork() => _taskConditionProvider.GetTaskCondition().Invoke();
}

public sealed class BuildingCycleCore : TaskCycleCore
{
    private ITaskConditionProvider _taskConditionProvider;
    private TaskRechargeDuration _taskRechargeDuration;
    private BuildingDraggable _buildingDraggable;

    public override float TaskRechargeDuration => _taskRechargeDuration.Value;
    
    public BuildingCycleCore(BuildingEntity buildingEntity, TaskRechargeDuration taskRechargeDuration) : base(buildingEntity)
    {
        _taskConditionProvider = buildingEntity.ComponentsContainer.Get<ITaskConditionProvider>();
        _buildingDraggable = buildingEntity.ComponentsContainer.Get<BuildingDraggable>();
        _taskRechargeDuration = taskRechargeDuration;
    }
    
    public override void Enable()
    {
        base.Enable();
        _buildingDraggable.PickedUp += StopRechargeProcess;
        _buildingDraggable.BuildCompleted += TryCycle;
    }

    public override void Disable()
    {
        base.Disable();
        _buildingDraggable.PickedUp -= StopRechargeProcess;
        _buildingDraggable.BuildCompleted -= TryCycle;
    }
    
    protected override bool CanWork() => _buildingDraggable.IsBuilt && _taskConditionProvider.GetTaskCondition().Invoke();
}

public sealed class StaticCycleCore : TaskCycleCore
{
    private readonly float _taskRechargeDuration;

    public override float TaskRechargeDuration => _taskRechargeDuration;
    
    public StaticCycleCore(CombatEntity ownerEntity, float taskRechargeDuration) : base(ownerEntity)
    {
        _taskRechargeDuration = taskRechargeDuration;
    }
}

public abstract class TaskCycleCore
{
    private readonly TokenContainer _cycleBlockTokenContainer = new(false);
    private readonly CombatEntity _ownerEntity;
    
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _taskCycleIsActive;
    private bool _enabled;
    
    public abstract float TaskRechargeDuration { get; }
    public TokenContainer CycleBlockTokenContainer => _cycleBlockTokenContainer;
    
    public Action TaskPerformed;
    public Action TaskCycled;

    protected TaskCycleCore(CombatEntity ownerEntity)
    {
        _ownerEntity = ownerEntity;   
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
    
    public void StopRechargeProcess()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    public bool IsPossibleToPerformTask()
    {
        if (!_ownerEntity.Health.IsAlive()) return false;

        return CanWork();
    }
    
    public virtual void Enable()
    {
        if (_enabled) return;

        _enabled = true;
        _ownerEntity.Health.Died += StopRechargeProcess;
        TryCycle();
    }

    public virtual void Disable()
    {
        if (!_enabled) return;
        
        _enabled = false;
        _ownerEntity.Health.Died -= StopRechargeProcess;
        StopRechargeProcess();
    }
    
    private async void StartRechargeProcess()
    {
        _taskCycleIsActive = true;

        try
        {
            float elapsedTime = 0;
                
            while (elapsedTime < TaskRechargeDuration)
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
    
    protected virtual bool CanWork() => true;
}