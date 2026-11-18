using Combat.Entity.Modificators;
using System.Linq;
using System;

namespace Combat.Entity.Sets
{
    public sealed class EntitySet
    {
        private EntityModificatorsContainer _entityModificatorsContainer;
        private EntitySetConfig _entitySetConfig;
        private int _currentStacks;
        
        public EntitySetConfig EntitySetConfig => _entitySetConfig;
        public int CurrentStacks => _currentStacks;

        public event Action StacksAmountChanged;
        
        public EntitySet(EntitySetConfig config, EntityModificatorsContainer container)
        {
            _entitySetConfig = config;
            _entityModificatorsContainer = container;
        }

        public void AddStack()
        {
            _currentStacks++;
            
            if (TryGetThresholdConfig(out EntitySetThresholdConfig thresholdConfig))
            {
                _entityModificatorsContainer.AddModificator(thresholdConfig.EntityModificator);
            }
        }

        public void RemoveStack()
        {
            _currentStacks--;

            if (TryGetThresholdConfig(out EntitySetThresholdConfig thresholdConfig))
            {
                _entityModificatorsContainer.RemoveModificator(thresholdConfig.EntityModificator);
            }
        }

        private bool TryGetThresholdConfig(out EntitySetThresholdConfig entitySetThresholdConfig)
        {
            entitySetThresholdConfig = _entitySetConfig.ThresholdConfigs.FirstOrDefault(config => config.LevelThreshold == _currentStacks);
            
            return entitySetThresholdConfig != null;
        }
    }

    public enum EntitySetType
    {
        BuildTime = 1,
        BigHitter = 2,
        Poison = 3,
        Fire = 4,
        Summoner = 5,
        Healer = 6,
        Executor = 7,
        Reach = 8,
        Control = 9,
        Mark = 10,
        MaxHealth = 11,
        QuickAttacks = 12,
        Sacrifice = 13,
    }
}