using CuroSceneManagement;
using UnityEngine;

namespace CuroLoading
{
    [CreateAssetMenu(fileName = "LoadingScreenConfig", menuName = "Loading/LoadingScreenConfig")]
    
    public sealed class LoadingScreenInitializationConfig : ScriptableObject
    {
        [SerializeField] private LoadingScreen _loadingScreenPrefab;
        
        [Header("Animation")]
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private float _animationDuration = 1f;
        
        public LoadingScreen LoadingScreenPrefab => _loadingScreenPrefab;
        public AnimationCurve AnimationCurve => _animationCurve;
        public float AnimationDuration => _animationDuration;
    }
}