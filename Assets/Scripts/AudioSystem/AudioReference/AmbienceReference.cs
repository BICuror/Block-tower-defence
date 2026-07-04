using UnityEngine;

namespace CuroAudio
{
    [CreateAssetMenu(fileName = "AmbienceReference", menuName = "Audio/References/AmbienceReference")]
    public sealed class AmbienceReference : AudioReferenceWithTransition
    {
        [Header("AmbienceSettings")]
        [SerializeField] private bool _playFromRandomPoint = true;
        
        public bool PlayFromRandomPoint => _playFromRandomPoint;
    }
}