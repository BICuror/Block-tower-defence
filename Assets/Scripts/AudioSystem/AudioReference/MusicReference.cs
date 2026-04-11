using UnityEngine;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "MusicReference", menuName = "Audio/References/MusicReference")]
    public sealed class MusicReference : AudioReferenceWithTransition
    {
        [Space] [Header("LifetimeDuration")]
        [SerializeField] private AudioAssetLifetimeDuration _lifetimeDuration = AudioAssetLifetimeDuration.NoLifetime;
        
        public override AudioAssetLifetimeDuration LifetimeDuration => _lifetimeDuration;
    }
}