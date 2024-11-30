using Cysharp.Threading.Tasks;
using System.Threading;
using Cashing;
using System;
using Combat;
using UnityEngine;

public sealed class BuildingDraggable : DraggableEntity
{
    [Cached] private BuildTime _buildTime;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isBuilt = true;

    private CancellationTokenSource CancellationTokenSource
    {
        get
        {
            if (_cancellationTokenSource == null) _cancellationTokenSource = new();
            return _cancellationTokenSource;
        }
    }
    

    public Action BuildCompleted;
    public Action<BuildingDraggable> BuildingPickedUp;
    public Action<BuildingDraggable> BuildingPlaced;
    public Action<BuildingDraggable> BuildingBuilt;

    public bool IsBuilt => _isBuilt;

    private void Start()
    {
        Placed += StartBuildingProcess;
        PickedUp += PickUpBuilding;
        PickedUp += StopBuildingProcess;
    }

    private void PickUpBuilding()
    {
        _isBuilt = false;
        BuildingPickedUp?.Invoke(this);
    }

    private async void StartBuildingProcess()
    {
        BuildingPlaced?.Invoke(this);
        await UniTask.WaitForSeconds(_buildTime.Value, cancellationToken: CancellationTokenSource.Token);
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
        CancellationTokenSource.Cancel();
        _cancellationTokenSource = null;
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        StopBuildingProcess();
    }
}