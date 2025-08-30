using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "SFXReference", menuName = "Audio/References/SFXReference")]
    public sealed class SFXReference : AudioReference
    {
        [Header("RandomSFX")]
        [SerializeField] private bool _useRandomSFX;
        [ShowIf("_useRandomSFX")] [SerializeField] private List<AssetReference> _randomSFXReferences;
        
        [Space] [Header("Pitch")]
        [SerializeField] private bool _useRandomPitch;
        [ShowIf("_useRandomPitch")] [SerializeField] private float _pitchMagnitude = 0.05f;
        
        [Space] [Header("LifetimeDuration")]
        [SerializeField] private AudioAssetLifetimeDuration _lifetimeDuration = AudioAssetLifetimeDuration.Short;
        
        public bool UseRandomSFX => _useRandomSFX;
        public List<AssetReference> RandomSFXReferences => _randomSFXReferences;
        
        public bool UseRandomPitch => _useRandomPitch;
        public float PitchMagnitude => _pitchMagnitude;
        
        public override AudioAssetLifetimeDuration LifetimeDuration => _lifetimeDuration;
    }
}