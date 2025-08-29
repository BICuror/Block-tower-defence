using UnityEngine.AddressableAssets;
using UnityEngine;

namespace CuroAudio
{
    public abstract class AudioReference : ScriptableObject
    {
        [Space] [Header("Reference")] [SerializeField]
        private AssetReference _audioReference;

        [Space] [Header("Volume")] [Range(0f, 1f)] [SerializeField]
        private float _volumeModifier = 1f;

        public AssetReference AudioFileReference => _audioReference;
        public float VolumeModifier => _volumeModifier;
        public abstract AudioAssetLifetimeDuration LifetimeDuration { get; }
    }
}