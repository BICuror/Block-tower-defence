using Cysharp.Threading.Tasks;
using System.Threading;
using Cashing;
using System;
using System.Threading.Tasks;
using Combat;
using UnityEngine;

public sealed class BuildingDraggable : DraggableEntity
{
    [Cached] private BuildTime _buildTime;
    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _isBuilt = true;

    public Action BuildCompleted;
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

    private void PickUpBuilding()
    {
        _isBuilt = false;
        BuildingPickedUp?.Invoke(this);
    }

    private async void StartBuildingProcess()
    {
        BuildingPlaced?.Invoke(this);

        try
        {
            await UniTask.WaitForSeconds(_buildTime.Value, cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            TaskUtility.LogAsync(e);
            return;
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