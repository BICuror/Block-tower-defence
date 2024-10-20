using UnityEngine;

namespace Navigation
{
    [CreateAssetMenu(fileName = "NavigationAgentData", menuName = "NavigationAgentData", order = 0)]

    public sealed class NavigationAgentData : ScriptableObject 
    {
        [Header("MovmentSettings")]
        [Tooltip("Seconds per one block")]
        [Range(0, 5f)] [SerializeField] private float _timePerBlock = 1f;
        
        [Header("MovmentCurves")]
        [Range(0, 4f)] [SerializeField] private float _verticalCurveMultiplyer;
        [SerializeField] private AnimationCurve _verticalMovmentCurve;
        [SerializeField] private AnimationCurve _horizontalMovmentCurve;

        public float TimePerBlock => _timePerBlock;
        public float VerticalCurveMultiplyer => _verticalCurveMultiplyer;
        public AnimationCurve VerticalMovmentCurve => _verticalMovmentCurve;
        public AnimationCurve HorizontalMovmentCurve => _horizontalMovmentCurve;
    }
}