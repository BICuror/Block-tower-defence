using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public sealed class BuildingEffectStand : MonoBehaviour
    {
        [SerializeField] private DraggableEffectManagerAreaDetector _draggableEffectManagerAreaDetector;
    
        [SerializeField] private Effect[] _applyEffects;
    
        private void Awake()
        {
            _draggableEffectManagerAreaDetector.AddedItem += AddEffectToContainer;
            _draggableEffectManagerAreaDetector.RemovedItem += RemoveEffectFromContainer;
        }
    
        private void AddEffectToContainer(EntityEffectManager draggable)
        {
            for (int i = 0; i < _applyEffects.Length; i++)
            {
                draggable.ApplyEffect(_applyEffects[i]);
            }
        }
    
        private void RemoveEffectFromContainer(EntityEffectManager draggable)
        {
            for (int i = 0; i < _applyEffects.Length; i++)
            {
                draggable.TryToRemoveEffect(_applyEffects[i]);
            }
        }
    }
}