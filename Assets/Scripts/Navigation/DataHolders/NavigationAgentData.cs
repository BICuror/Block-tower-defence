using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Navigation
{
    [CreateAssetMenu(fileName = "NavigationAgentData", menuName = "NavigationAgentData", order = 0)]

    public sealed class NavigationAgentData : ScriptableObject
    {
        [SerializeField] private NavigationMapLayerType _prefferedNavigationLayer;
        [SerializeField] [Dropdown("AllNavigationNodePickerTypes")] private string _navigationNodePickerType;
        
        [Header("DefaultMovementCurves")]
        [SerializeField] private AnimationCurve _defaultHorizontalMovementCurve;
        
        [Header("JumpMovementCurves")]
        [Range(0, 4f)] [SerializeField] private float _verticalCurveMultiplyer;
        [SerializeField] private AnimationCurve _JumpVerticalMovementCurve;
        [SerializeField] private AnimationCurve _JumpHorizontalMovementCurve;

        [HideInInspector] public List<string> AllNavigationNodePickerTypes;
        
        public NavigationMapLayerType PrefferedNavigationLayer => _prefferedNavigationLayer;
        public AnimationCurve DefaultHorizontalMovementCurve => _defaultHorizontalMovementCurve;
        public string NavgationNodePickerType => _navigationNodePickerType;
        public float VerticalCurveMultiplyer => _verticalCurveMultiplyer;
        public AnimationCurve JumpVerticalMovementCurve => _JumpVerticalMovementCurve;
        public AnimationCurve JumpHorizontalMovementCurve => _JumpHorizontalMovementCurve;
    }
}