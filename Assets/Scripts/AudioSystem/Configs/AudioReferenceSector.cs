using System.Collections.Generic;
using UnityEngine;
using System;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "AudioReferenceSector", menuName = "Audio/AudioReferenceSector")]
    public sealed class AudioReferenceSector : ScriptableObject
    {
        [SerializeField] private string _generatedEnumName;
        [SerializeField] private string _sectorName;
        [SerializeField] private List<AudioReferenceSubSector> _subSectors;
        
        public string GeneratedEnumName => _generatedEnumName;
        public string Name => _sectorName;
        public List<AudioReferenceSubSector> SubSectors => _subSectors;
    }
    
    [Serializable] public sealed class AudioReferenceSubSector
    {
        [Header("Main")]
        [SerializeField] private string _subSectorName;
        [Header("AudioReferences")]
        [SerializeField] private List<AudioReferenceEntry> _audioReferenceEntries;
        
        public string Name => _subSectorName;
        public List<AudioReferenceEntry> AudioReferenceEntries => _audioReferenceEntries;
    }

    [Serializable] public class AudioReferenceEntry
    {
        [SerializeField] private AudioReference _audioReference;
        [SerializeField] private string _soundID;
        public AudioEnum EnumValue;
        
        public AudioReference AudioReference => _audioReference;
        public string SoundID => _soundID;
    }
}