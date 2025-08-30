using UnityEngine;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "AmbienceReference", menuName = "Audio/References/AmbienceReference")]
    public sealed class AmbienceReference : AudioReference
    {
        [Header("Transition")]
        [SerializeField] private float _transitionDuration = 0.5f;
        
        [Space] [Header("LifetimeDuration")]
        [SerializeField] private bool _hasLifetimeDuration;
        [SerializeField] private AudioAssetLifetimeDuration _lifetimeDuration = AudioAssetLifetimeDuration.NoLifetime;

        public float TransitionDuration => _transitionDuration;
        public override AudioAssetLifetimeDuration LifetimeDuration => _lifetimeDuration;
    }
}