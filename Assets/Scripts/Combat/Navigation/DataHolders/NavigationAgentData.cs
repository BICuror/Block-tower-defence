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
        
        [Header("MovmentCurves")]
        [Range(0, 4f)] [SerializeField] private float _verticalCurveMultiplyer;
        [SerializeField] private AnimationCurve _verticalMovmentCurve;
        [SerializeField] private AnimationCurve _horizontalMovmentCurve;

        [HideInInspector] public List<string> AllNavigationNodePickerTypes;
        
        public NavigationMapLayerType PrefferedNavigationLayer => _prefferedNavigationLayer;
        public string NavgationNodePickerType => _navigationNodePickerType;
        public float VerticalCurveMultiplyer => _verticalCurveMultiplyer;
        public AnimationCurve VerticalMovmentCurve => _verticalMovmentCurve;
        public AnimationCurve HorizontalMovmentCurve => _horizontalMovmentCurve;
    }
}