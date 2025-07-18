using System.Collections.Generic;
using UnityEngine;
using Cashing;
using System;
using Combat;

public sealed class OrbitalController : MonoBehaviour
{
    [SerializeField] private Transform _orbitalsParent;
    [SerializeField] private List<Transform> _orbitalPositions;
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
    
    private void PositionAllOrbitals()
    {
        _instantiatedOrbitals.ForEach(orbital => orbital.CancelMovement());
        
        List<Vector3> localOrbitalPositions = GetNewPositions();
        List<Transform> targetTransforms = GetTargetTransforms();
        
        for (int i = 0; i < _instantiatedOrbitals.Count; i++)
        {
            _instantiatedOrbitals[i].transform.position = _orbitalsParent.transform.position + localOrbitalPositions[i];
            _instantiatedOrbitals[i].SetNextTarget(targetTransforms[i]);
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
            
            /*if (Math.Abs(cos) > Math.Abs(sin))
            {
                sin = GetNormalizedValue(sin);
            }
            else if (Math.Abs(cos) < Math.Abs(sin))
            {
                cos = GetNormalizedValue(cos);
            }
            else
            {
                sin = GetNormalizedValue(sin);
                cos = GetNormalizedValue(cos);
            }*/
            
            newPositions.Add(new Vector3(cos * radius, 0f, sin * radius));
        }

        return newPositions;

        float GetNormalizedValue(float value)
        {
            if (value > 0) return 1f;
            return -1f;
        }
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
}