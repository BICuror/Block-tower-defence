using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public sealed class Building : DraggableObject
{
    [Header("BuildingProcessSettings")]

    [SerializeField] private float _buildTime;
    public float BuildTime => _buildTime;
    [SerializeField] private BuildingProgressBar _buildBar;

    private bool _isBuilt = true;
    
    public UnityEvent BuildCompleted;

    [HideInInspector] public UnityEvent<Building> BuildingPlaced;
    [HideInInspector] public UnityEvent<Building> BuildingBuilt;
    [HideInInspector] public UnityEvent<Building> BuildingPickedUp;
    
    public bool IsBuilt() => _isBuilt;

    private void Start()  
    {
        Placed.AddListener(StartBuilding);

        PickedUp.AddListener(DisableBuilding);
        PickedUp.AddListener(StopBuildingProcess);
        if (_buildBar != null) PickedUp.AddListener(_buildBar.StopFillingBar);
    }

    private void DisableBuilding() 
    {
        _isBuilt = false;

        BuildingPickedUp?.Invoke(this);
    }

    private void StartBuilding()
    {
        BuildingPlaced?.Invoke(this);

        StartCoroutine(StartBuildingProcess());
        
        _buildBar?.StartFillingBar(_buildTime);
    }

    private IEnumerator StartBuildingProcess()
    {
        yield return new WaitForSeconds(_buildTime);

        CompleteBuild();
    }

    private void CompleteBuild()
    {
        _isBuilt = true;

        BuildCompleted?.Invoke();

        BuildingBuilt?.Invoke(this);
    }

    private void StopBuildingProcess() => StopAllCoroutines();
}
