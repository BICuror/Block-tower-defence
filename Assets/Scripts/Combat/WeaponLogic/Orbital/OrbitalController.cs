using System.Collections.Generic;
using UnityEngine;
using Cashing;
using System;
using Combat;

public sealed class OrbitalController : MonoBehaviour
{
    [SerializeField] private Orbital _orbitalPrefab;
    [SerializeField] private GameObject _orbitalsParent;
    [SerializeField] private List<Transform> _orbitalPositions;
    [Cached] private ReachAreaScale _reachAreaScale;
    [Cached] private BuildingDraggable _buildingDraggable;
    [Cached] private CombatEntity _ownerEntity;
    private readonly List<Orbital> _instantiatedOrbitals = new();

    private void Start()
    {
        _buildingDraggable.BuildCompleted += Enable;
        _buildingDraggable.PickedUp += Disable;

        InstantiateAndAddOrbital(_orbitalPrefab);
        InstantiateAndAddOrbital(_orbitalPrefab);
        InstantiateAndAddOrbital(_orbitalPrefab);
        InstantiateAndAddOrbital(_orbitalPrefab);
    }

    private void Enable()
    {
        _orbitalsParent.SetActive(true);
        _instantiatedOrbitals.ForEach(orbital => orbital.gameObject.SetActive(true));
        PositionAllOrbitals();
    }

    private void Disable()
    {
        _orbitalsParent.SetActive(false);
        _instantiatedOrbitals.ForEach(orbital => orbital.gameObject.SetActive(false));
    }
    
    public void InstantiateAndAddOrbital(Orbital orbitalPrefab)
    {
        Orbital orbital = Instantiate(orbitalPrefab);
        orbital.SetTravelPoints(_orbitalPositions);
        orbital.Initialize(_ownerEntity);
        _instantiatedOrbitals.Add(orbital);
        
        PositionAllOrbitals();
    }
    
    private void PositionAllOrbitals()
    {
        List<Vector3> localOrbitalPositions = GetNewPositions();
        List<Transform> targetTransforms = GetTargetTransforms();
        
        for (int i = 0; i < _instantiatedOrbitals.Count; i++)
        {
            _instantiatedOrbitals[i].transform.position = _orbitalsParent.transform.position + localOrbitalPositions[i];
            _instantiatedOrbitals[i].SetNextTarget(targetTransforms[i]);
            
            Debug.Log($"Launched orbital {i} at {_orbitalPositions.IndexOf(targetTransforms[i])}");
        }
    }
    
    private List<Vector3> GetNewPositions()
    {
        List<Vector3> newPositions = new();

        float step = 360f / _instantiatedOrbitals.Count;

        for (int i = 0; i < _instantiatedOrbitals.Count; i++)
        {
            var radians = Math.PI * (step * i) / 180.0;
            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);

            float radius = _reachAreaScale.RoundedValue;

            if (Math.Abs(cos) > Math.Abs(sin))
            {
                if (sin > 0) sin = 1f;
                else sin = -1f;
            }
            else if (Math.Abs(cos) < Math.Abs(sin))
            {
                if (cos > 0) cos = 1f;
                else cos = -1f;
            }
            else
            {
                if (sin > 0) sin = 1f;
                else sin = -1f;
                if (cos > 0) cos = 1f;
                else cos = -1f;
            }
            
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
            if (angle < 90f) targets.Add(_orbitalPositions[0]);
            else if (angle < 180f) targets.Add(_orbitalPositions[1]);
            else if (angle < 270f) targets.Add(_orbitalPositions[2]);
            else targets.Add(_orbitalPositions[3]);
                
            angle += step;
        }

        return targets;
    }
    
    public void RemoveOrbital(Orbital orbitalToRemove)
    { 
        _instantiatedOrbitals.Remove(orbitalToRemove);
    }
}