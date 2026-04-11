using System.Collections.Generic;
using System.Linq;

namespace CuroAudio
{
    public sealed class AudioChannelLayerContainer<T> where T : AudioReference
    {
        private Dictionary<AudioLayer, T> _activeAudioReferences = new();
        
        public bool IsEmpty => _activeAudioReferences.Count == 0;

        public T GetAudioReference(AudioLayer audioLayer) => _activeAudioReferences[audioLayer];
        
        public AudioLayer GetHighestPriorityLayer()
        {
            return GetAllPresentAudioLayers()[^1];
        }

        public List<AudioLayer> GetAllPresentAudioLayers()
        {
            List<AudioLayer> layers = _activeAudioReferences.Keys.ToList();
            
            layers.Sort();
            
            return layers;
        }
        
        public bool HasLayer(AudioLayer layer) => _activeAudioReferences.ContainsKey(layer);
        
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