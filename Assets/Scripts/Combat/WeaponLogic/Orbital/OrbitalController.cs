using System.Collections.Generic;
using UnityEngine;
using Cashing;
using System;
using Combat;
using WorldGeneration;
using Zenject;

public sealed class OrbitalController : MonoBehaviour
{
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [SerializeField] private Transform _orbitalsParent;
    [SerializeField] private List<Transform> _orbitalPositions;
    [SerializeField] private bool _followTerrain;
    [Cached] private ReachAreaScale _reachAreaScale;
    [Cached] private BuildingDraggable _buildingDraggable;
    [Cached] private CombatEntity _ownerEntity;
    private readonly List<Orbital> _instantiatedOrbitals = new();

    private void Start()
    {
        _buildingDraggable.BuildCompleted += Enable;
        _buildingDraggable.PickedUp += Disable;
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
    
    public void InstantiateAndAddOrbital(Orbital orbitalPrefab)
    {
        Orbital orbital = Instantiate(orbitalPrefab);
        
        orbital.SetIslandHeightMapHolder(_islandHeightMapHolder);
        orbital.SetFollowTerrainState(_followTerrain);
        orbital.transform.SetParent(_orbitalsParent);
        orbital.SetTravelPoints(_orbitalPositions);
        orbital.Initialize(_ownerEntity);
        
        _instantiatedOrbitals.Add(orbital);
        
        PositionAllOrbitals();
    }
    
    public void RemoveOrbital(Orbital orbitalToRemove)
    { 
        _instantiatedOrbitals.Remove(orbitalToRemove);
        
        PositionAllOrbitals();
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

}