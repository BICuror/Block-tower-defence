using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "AudioSystemConfig", menuName = "Audio/AudioSystemConfig")]
    
    public sealed class AudioSystemConfig : ScriptableObject
    {
        [SerializeField] private List<AudioReferenceSector> _audioReferenceSectors;
        
        [Space] [Header("AudioPlayers")]
        [SerializeField] private AudioSourceAudioPlayerObject _audioSourceAudioPlayerPrefab;
        
        public List<AudioReferenceSector> Sectors => _audioReferenceSectors;
        public AudioSourceAudioPlayerObject AudioSourceAudioPlayerPrefab => _audioSourceAudioPlayerPrefab;

        public bool CheckForDuplicateIDs()
        {
            bool containsDuplicateIDs = false;

            _audioReferenceSectors.ForEach(sector =>
            {
                sector.SubSectors.ForEach(subSector =>
                {
                    List<AudioReferenceEntry> audioReferenceEntries = subSector.AudioReferenceEntries;
                    
                    for (int outerScope = 0; outerScope < audioReferenceEntries.Count; outerScope++)
                    {
                        string outerScopeID = audioReferenceEntries[outerScope].SoundID;
                        
                        for (int innerScope = outerScope + 1; innerScope < audioReferenceEntries.Count; innerScope++)
                        {
                            string innerScopeID = audioReferenceEntries[innerScope].SoundID;
                            
                            if (outerScopeID == innerScopeID)
                            {
                                Debug.LogError($"Duplicate AudioReferenceEntry id: {AudioUtility.GetAudioReferenceFullID(sector.Name, subSector.Name, innerScopeID)} on indexes {outerScope} and {innerScope}");
                                containsDuplicateIDs = true;
                            }
                        }
                    }
                });
            });

            return containsDuplicateIDs;
        }
        
        public void ApplyEnumValues()
        {
            Sectors.ForEach(sector =>
            {
                sector.SubSectors.ForEach(subSector =>
                {
                    for (int i = 0; i < subSector.AudioReferenceEntries.Count; i++)
                    {
                        string enumStringValue = AudioUtility.GetAudioReferenceFullID(sector.Name, subSector.Name, subSector.AudioReferenceEntries[i].SoundID);

                        if (Enum.TryParse(typeof(AudioEnum), enumStringValue, true, out object enumValue))
                        {
                            subSector.AudioReferenceEntries[i].EnumValue = (AudioEnum)enumValue;
                        }
                        else
                        {
                            Debug.LogError($"Could not parse {enumStringValue} to AudioEnum, try regenerating AudioEnum");
                        }
                    }
                });
                
                EditorUtility.SetDirty(sector);
                AssetDatabase.SaveAssetIfDirty(sector);
            });
        }
    }
}