using Cashing;
using UnityEngine;

public sealed class OrbitalTower : MonoBehaviour
{
    [SerializeField] private Orbital _orbitalPrefab;
    [SerializeField] private int _defaultOrbiralsAmount = 3;
    [Cached] private OrbitalController _orbitalController;
    
    private void Start()
    {
        for (int i = 0; i < _defaultOrbiralsAmount; i++)
        {
            _orbitalController.InstantiateAndAddOrbital(_orbitalPrefab);
        }
    }
}