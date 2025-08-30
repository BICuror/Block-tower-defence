using UnityEngine;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "MusicReference", menuName = "Audio/References/MusicReference")]
    public sealed class MusicReference : AudioReference
    {
        [Header("Transition")]
        [SerializeField] private float _transitionDuration = 0.5f;
        
        [Space] [Header("LifetimeDuration")]
        [SerializeField] private AudioAssetLifetimeDuration _lifetimeDuration = AudioAssetLifetimeDuration.NoLifetime;
        
        public float TransitionDuration => _transitionDuration;
        public override AudioAssetLifetimeDuration LifetimeDuration => _lifetimeDuration;
    }
}