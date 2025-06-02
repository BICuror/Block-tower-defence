using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using System;
using Combat;
using Zenject;

public sealed class BuildingDraggable : DraggableEntity
{
    [Inject] private WaveStateMachine _waveStateMachine;
    [Cached] private CombatEntity _ownerEntity;
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _isBuilt = true;
    
    private BuildTime _buildTime;
    private bool _hasBuildTime;
    
    public Action BuildCompleted;
    public Action BuildProgressStarted;
    public Action<float> BuildProcessUpdated;
    public Action<BuildingDraggable> BuildingPickedUp;
    public Action<BuildingDraggable> BuildingPlaced;
    public Action<BuildingDraggable> BuildingBuilt;

    public bool IsBuilt => _isBuilt;

    private void Awake()
    {;
        base.Awake();
        
        Placed += StartBuildingProcess;
        PickedUp += PickUpBuilding;
        PickedUp += StopBuildingProcess;
    }

    private void Start()
    {
        base.Start();

        _hasBuildTime = _ownerEntity.StatContainer.Has<BuildTime>();
        if (_hasBuildTime) _buildTime = _ownerEntity.StatContainer.Get<BuildTime>();
    }

    private void PickUpBuilding()
    {
        _isBuilt = false;
        BuildingPickedUp?.Invoke(this);
    }

    private async void StartBuildingProcess()
    {
        BuildingPlaced?.Invoke(this);

        if (_hasBuildTime && _waveStateMachine.CurrentState == WaveState.Attack)
        {
            BuildProgressStarted?.Invoke();
            BuildProcessUpdated?.Invoke(0);
            
            try
            {
                float elapsedTime = 0;
                
                while (elapsedTime < _buildTime.Value)
                {
                    await UniTask.WaitForFixedUpdate(cancellationToken: _cancellationTokenSource.Token);
                    
                    elapsedTime += Time.fixedDeltaTime;
                    
                    BuildProcessUpdated?.Invoke(elapsedTime / _buildTime.Value);
                }
                
                BuildProcessUpdated?.Invoke(1f);
            }
            catch (Exception e)
            {
                TaskUtility.LogAsync(e);
                return;
            }
        }
        
        CompleteBuild();
    }

    private void CompleteBuild()
    {
        _isBuilt = true;
        BuildCompleted?.Invoke();
        BuildingBuilt?.Invoke(this);
    }

    private void StopBuildingProcess()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        StopBuildingProcess();
    }
}