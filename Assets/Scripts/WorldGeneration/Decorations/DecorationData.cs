using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "DecorationData", menuName = "Generation/DecorationData")]

    public sealed class DecorationData :  ScriptableObject
    {
        [SerializeField] private MeshRenderer[] _prefabs;
        public MeshRenderer[] Prefabs => _prefabs;

        [SerializeField] private int _amount;
        public int Amount => _amount;

        [SerializeField] private float _minScale, _maxScale;
        public float MinScale => _minScale;
        public float MaxScale => _maxScale;
            
        [Header("Position")][Space] 
        [Range(0f, 1f)] [SerializeField] private float _placementOffset;
        public float PlacmentOffset => _placementOffset;

        [Header("RandomRotation")][Space] 
        [SerializeField] private Vector3 _maxRotation;
        public Vector3 MaxRotation => _maxRotation;
    }
}
