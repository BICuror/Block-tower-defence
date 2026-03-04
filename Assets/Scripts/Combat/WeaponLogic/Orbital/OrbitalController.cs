using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;
using Cashing;
using System;
using Combat;

public sealed class OrbitalController : MonoBehaviour
{
    [SerializeField] private List<Transform> _orbitalPositions;
    [SerializeField] private Transform _orbitalsParent;
    [SerializeField] private Orbital _orbitalPrefab;
    [SerializeField] private bool _followTerrain;
    
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Cached] private BuildingDraggable _buildingDraggable;
    [Cached] private ReachAreaScale _reachAreaScale;
    [Cached] private CombatEntity _ownerEntity;
    [Cached] private MaxEntities _maxEntities;
    
    private readonly List<Orbital> _instantiatedOrbitals = new();

    private void Start()
    {
        _maxEntities.RoundedValueChanged += UpdateOrbitalsAmount;
        _buildingDraggable.BuildCompleted += Enable;
        _buildingDraggable.PickedUp += Disable;
        
        UpdateOrbitalsAmount(_maxEntities.RoundedValue);
    }

    private void Enable()
    {
        _orbitalsParent.gameObject.SetActive(true);
        _instantiatedOrbitals.ForEach(orbital => orbital.gameObject.SetActive(true));
        PositionAllOrbitals();
    }

    private void Disable()
    {
        _orbitalsParent.gameObject.SetActive(false);
        _instantiatedOrbitals.ForEach(orbital => orbital.gameObject.SetActive(false));
    }

    private void UpdateOrbitalsAmount(int orbitalAmount)
    {
        if (orbitalAmount > _instantiatedOrbitals.Count)
        {
            int orbitalsToCreate = orbitalAmount - _instantiatedOrbitals.Count;

            for (int i = 0; i < orbitalsToCreate; i++) InstantiateAndAddOrbital();
        }
        else if (orbitalAmount < _instantiatedOrbitals.Count)
        {
            int orbitalsToRemove = _instantiatedOrbitals.Count - orbitalAmount;

            for (int i = 0; i < orbitalsToRemove; i++) RemoveOrbital();
        }
    }
    
    public void InstantiateAndAddOrbital()
    {
        Orbital orbital = Instantiate(_orbitalPrefab);
        
        orbital.SetIslandHeightMapHolder(_islandHeightMapHolder);
        orbital.SetFollowTerrainState(_followTerrain);
        orbital.transform.SetParent(_orbitalsParent);
        orbital.SetTravelPoints(_orbitalPositions);
        orbital.Initialize(_ownerEntity);
        
        _instantiatedOrbitals.Add(orbital);
        
        PositionAllOrbitals();
    }
    
    public void RemoveOrbital()
    {
        Orbital orbital = _instantiatedOrbitals[^1];
        
        _instantiatedOrbitals.Remove(orbital);
        
        PositionAllOrbitals();
        
        Destroy(orbital.gameObject);
    }

    #region OrbitalInitialPositioning

    private void PositionAllOrbitals()
    {
        _instantiatedOrbitals.ForEach(orbital => orbital.CancelMovement());
        
        List<Vector3> localOrbitalPositions = GetNewPositions();
        List<Transform> targetTransforms = GetTargetTransforms();
        
        for (int i = 0; i < _instantiatedOrbitals.Count; i++)
        {
            _instantiatedOrbitals[i].SetNextTarget(targetTransforms[i], localOrbitalPositions[i] + _orbitalsParent.transform.position);
        }
    }
    
    private List<Vector3> GetNewPositions()
    {
        List<Vector3> newPositions = new();

        float step = 360f / (_instantiatedOrbitals.Count);

        for (int i = 0; i < _instantiatedOrbitals.Count; i++)
        {
            var radians = Math.PI * (step * i) / 180.0;
            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);

            float radius = _reachAreaScale.RoundedValue;
            
            newPositions.Add(new Vector3(cos * radius, 0f, sin * radius));
        }

        return newPositions;
    }
    
    private List<Transform> GetTargetTransforms()
    {
        List<Transform> targets = new();

        float angle = 0f;
        float step = 360f / _instantiatedOrbitals.Count;

        for (int i = 0; i < _instantiatedOrbitals.Count; i++)
        {
            if (angle < 45f) targets.Add(_orbitalPositions[0]);
            else if (angle < 135f) targets.Add(_orbitalPositions[1]);
            else if (angle < 225f) targets.Add(_orbitalPositions[2]);
            else if (angle < 315f) targets.Add(_orbitalPositions[3]);
            else targets.Add(_orbitalPositions[0]);
                
            angle += step;
        }

        return targets;
    }

    #endregion

    private void OnEnable() => Enable();
    
    private void OnDisable() => Disable();
    
    private void OnDestroy()
    {
        _maxEntities.RoundedValueChanged -= UpdateOrbitalsAmount;
    }
}