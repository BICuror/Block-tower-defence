using UnityEngine;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "MusicReference", menuName = "Audio/References/MusicReference")]
    public sealed class MusicReference : AudioReference
    {
        [Header("Transition")]
        [SerializeField] private float _transitionDuration = 0.5f;
        
        public float TransitionDuration => _transitionDuration;
        public override AudioAssetLifetimeDuration LifetimeDuration => AudioAssetLifetimeDuration.NoLifetime;
    }
}