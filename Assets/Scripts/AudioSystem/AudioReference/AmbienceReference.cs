using UnityEngine;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "AmbienceReference", menuName = "Audio/References/AmbienceReference")]
    public sealed class AmbienceReference : AudioReferenceWithTransition
    {
        [Space] [Header("LifetimeDuration")]
        [SerializeField] private bool _hasLifetimeDuration;
        [SerializeField] private AudioAssetLifetimeDuration _lifetimeDuration = AudioAssetLifetimeDuration.NoLifetime;

        public override AudioAssetLifetimeDuration LifetimeDuration => _lifetimeDuration;
    }
}