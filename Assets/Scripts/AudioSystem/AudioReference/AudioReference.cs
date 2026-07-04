using UnityEngine.AddressableAssets;
using NaughtyAttributes;
using UnityEngine;

namespace CuroAudio
{
    public abstract class AudioReference : ScriptableObject
    {
        [Space] [Header("Reference")] [SerializeField]
        private AssetReference _audioReference;

        [Space] [Header("Volume")] [Range(0f, 1f)] [SerializeField]
        private float _volumeModifier = 1f;
        
        [Space] [Header("LifetimeDuration")]
        [SerializeField] private bool _hasLifetimeDuration;
        [ShowIf("_hasLifetimeDuration")] [SerializeField] private AudioAssetLifetimeDuration _lifetimeDuration = AudioAssetLifetimeDuration.Short;

        public AssetReference AudioFileReference => _audioReference;
        public float VolumeModifier => _volumeModifier;
        public bool HasLifetimeDuration => _hasLifetimeDuration;
        public AudioAssetLifetimeDuration LifetimeDuration => _lifetimeDuration;
    }

    public abstract class AudioReferenceWithTransition : AudioReference
    {
        [Header("Transition")]
        [SerializeField] private float _transitionDuration = 0.5f;
        
        public float TransitionDuration => _transitionDuration;
    }
}