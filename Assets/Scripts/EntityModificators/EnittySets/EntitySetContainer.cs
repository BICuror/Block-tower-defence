using System.Collections.Generic;
using System.Linq;
using Combat.Entity.Modificators;
using UnityEngine;
using Cashing;

namespace Combat.Entity.Sets
{
    public sealed class EntitySetContainer : MonoBehaviour
    {
        [SerializeField] private EntitySetConfigContainer _entitySetConfigContainer;
        [Cached] private EntityModificatorsContainer _entityModificatorsContainer;
        private Dictionary<EntitySetType, EntitySet> _containedSets = new();
        
        private void Start()
        {
            _entityModificatorsContainer.ModificatorApplied += AddEntitySetProgress;
            _entityModificatorsContainer.ModificatorRemoved += RemoveEntitySetProgress;
        }

        public IReadOnlyList<EntitySetType> GetAllContainedEntitySetTypes() => _containedSets.Keys.ToList();

        public int GetEnitySetStacks(EntitySetType entitySetType)
        {
            if (_containedSets.TryGetValue(entitySetType, out EntitySet set)) return set.CurrentStacks;
           
            return 0;
        }
        
        public EntitySet GetEntitySet(EntitySetType entitySetType) => _containedSets[entitySetType];

        private void AddEntitySetProgress(EntityModificatorConfig config)
        {
            foreach (EntitySetType setType in config.EntitySetTypes)
            {
                if (!_containedSets.ContainsKey(setType))
                {
                    _containedSets.Add(setType, new EntitySet(_entitySetConfigContainer.EntitySetConfigs[setType], _entityModificatorsContainer));
                }
                
                _containedSets[setType].AddStack();
            }
        }

        private void RemoveEntitySetProgress(EntityModificatorConfig config)
        {
            foreach (EntitySetType setType in config.EntitySetTypes)
            {
                _containedSets[setType].RemoveStack();
                
                if (_containedSets[setType].CurrentStacks == 0) _containedSets.Remove(setType);
            }
        }
        
        private void OnDestroy()
        {
            _entityModificatorsContainer.ModificatorApplied -= AddEntitySetProgress;
            _entityModificatorsContainer.ModificatorRemoved -= RemoveEntitySetProgress;
        }
    }
}