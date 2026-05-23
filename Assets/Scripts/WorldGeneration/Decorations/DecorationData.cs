using System;
using NaughtyAttributes;
using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "DecorationData", menuName = "Generation/DecorationData")]

    public sealed class DecorationData :  ScriptableObject
    {
        [SerializeField] private DecorationObject[] _prefabs;
        public DecorationObject[] Prefabs => _prefabs;

        [SerializeField] private int _amount;
        public int Amount => _amount;

        [Header("Scale")] 
        [SerializeField] private bool _hasYScale;
        [ShowIf("_hasYScale")] [SerializeField] private float _minYScale, _maxYScale;
        [SerializeField] private float _minScale, _maxScale;
        public bool HasYScale => _hasYScale;
        public float MinYScale => _minYScale;
        public float MaxYScale => _maxYScale;
        public float MinScale => _minScale;
        public float MaxScale => _maxScale;

        [Header("Position")] [Space] 
        [Range(0f, 1f)] [SerializeField] private float _placementOffset;
        [SerializeField] private float _decorationScale = 1f;
        public float PlacementOffset => _placementOffset;
        public float DecorationScale => _decorationScale;
        
        [Header("RandomRotation")] [Space] 
        [SerializeField] private bool _lockToRightAngleRotation;
        [SerializeField] private bool _rotateXAxis;
        [SerializeField] private bool _rotateYAxis = true;
        [SerializeField] private bool _rotateZAxis;
        public bool LockToRightAngleRotation => _lockToRightAngleRotation;
        public bool RotateXAxis => _rotateXAxis;
        public bool RotateYAxis => _rotateYAxis;
        public bool RotateZAxis => _rotateZAxis;
    }
}
