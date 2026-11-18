using TMPEffects.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.Entity.Sets
{
    [CreateAssetMenu(fileName = "EntitySetConfigContainer", menuName = "EntitySets/EntitySetConfigContainer")]
    
    public sealed class EntitySetConfigContainer : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<EntitySetType, EntitySetConfig> _entitySetConfigs;
        
        public IReadOnlyDictionary<EntitySetType, EntitySetConfig> EntitySetConfigs => _entitySetConfigs;
    }
}