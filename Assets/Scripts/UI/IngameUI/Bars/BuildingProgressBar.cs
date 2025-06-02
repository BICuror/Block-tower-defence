using UnityEngine;
using Cashing;

[RequireComponent(typeof(MeshRenderer))]

public class BuildingProgressBar : ProgressBarBase
{
    [Cached] private BuildingDraggable _buildingDraggable;
    [Cached] private BuildTime _buildTime;

    protected override string ProgressFieldName => "BuildProgress";

    protected void Start()
    {
        base.Start();
        
        _buildingDraggable.BuildingProcessStarted += StartFillingBar;
        _buildingDraggable.PickedUp += StopFillingBar;
        
        gameObject.SetActive(false);
    }
    
    private void StartFillingBar()
    {
        gameObject.SetActive(true);
        Shake();
        FillBar(1, 0, _buildTime.Value);
    }

    private void StopFillingBar()
    {
        StopBarFill();
        gameObject.SetActive(false);
    }
    
    protected override void OnFillComplete() => StopFillingBar();
    
    protected void OnDestroy()
    {
        base.OnDestroy();
        _buildingDraggable.Placed -= StartFillingBar;
        _buildingDraggable.PickedUp -= StopFillingBar;
        StopFillingBar();
    }
}