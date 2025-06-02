using UnityEngine;
using Cashing;

[RequireComponent(typeof(MeshRenderer))]

public class BuildingProgressBar : ProgressBarBase
{
    [Cached] private BuildingDraggable _buildingDraggable;
    [Cached] private BuildTime _buildTime;
    private float _previousValue;

    protected override string ProgressFieldName => "BuildProgress";

    protected void Start()
    {
        base.Start();

        _buildingDraggable.BuildProgressStarted += ResetFillingBar;
        _buildingDraggable.BuildProcessUpdated += StartFillingBar;
        _buildingDraggable.PickedUp += StopFillingBar;
        
        gameObject.SetActive(false);
    }

    private void ResetFillingBar()
    {
        _previousValue = 0;
        gameObject.SetActive(true);
        Shake();
    }
    
    private void StartFillingBar(float newValue)
    {
        FillBar(_previousValue, newValue, Time.fixedDeltaTime);
        _previousValue = newValue;
    }

    private void StopFillingBar()
    {
        StopBarFill();
        gameObject.SetActive(false);
    }

    protected override void OnFillComplete()
    {
        if (_previousValue == 1f) StopFillingBar();
    }
    
    protected void OnDestroy()
    {
        base.OnDestroy();
        _buildingDraggable.Placed -= ResetFillingBar;
        _buildingDraggable.BuildProcessUpdated -= StartFillingBar;
        _buildingDraggable.PickedUp -= StopFillingBar;
        StopFillingBar();
    }
}