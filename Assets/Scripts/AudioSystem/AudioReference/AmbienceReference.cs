using UnityEngine;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "AmbienceReference", menuName = "Audio/References/AmbienceReference")]
    public sealed class AmbienceReference : AudioReference
    {
        [Header("Transition")]
        [SerializeField] private float _transitionDuration = 0.5f;

        public float TransitionDuration => _transitionDuration;
        public override AudioAssetLifetimeDuration LifetimeDuration => AudioAssetLifetimeDuration.NoLifetime;
    }
}