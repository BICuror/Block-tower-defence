using System.Collections.Generic;
using System;

namespace CuroAudio
{
    public sealed class AudioChannelLayerContainer<T> where T : AudioReference
    {
        private Dictionary<AudioLayer, T> _activeAudioReferences = new();
        
        public bool IsEmpty => _activeAudioReferences.Count == 0;
        
        public T GetHighestPriorityAudioReference()
        {
            int amount = Enum.GetValues(typeof(AudioLayer)).Length;

            for (int i = 1; i <= amount; i++)
            {
                AudioLayer layer = (AudioLayer)(amount - i);
                
                if (_activeAudioReferences.TryGetValue(layer, out T reference))
                {
                    return reference;
                }
            }

            throw new Exception($"Could not find active audio reference");
        }
        
        public void RemoveLayer(AudioLayer layer)
        {
            _activeAudioReferences.Remove(layer);
        }

        public void SetActiveReferenceToLayer(T reference, AudioLayer layer)
        {
            _activeAudioReferences[layer] = reference;
        }
    }
}