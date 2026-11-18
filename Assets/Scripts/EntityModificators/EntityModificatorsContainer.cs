using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat.Effects;
using UnityEngine;
using Cashing;
using Zenject;

namespace Combat.Entity.Modificators
{
    public sealed class EntityModificatorsContainer : MonoBehaviour
    {
        private readonly ListDictionary<EntityModificatorConfig, Effect> _appliedEffects = new();

        [SerializeField] private List<EntityModificatorConfig> _initialEffectConfigs;

        [Inject] private EffectFactory _effectFactory;
        [Cached] private CombatEntity _ownerEntity;

        public List<EntityModificatorConfig> AppliedModificators => _appliedEffects.GetAllKeys();
        
        public event Action<EntityModificatorConfig> ModificatorApplied;
        public event Action<EntityModificatorConfig> ModificatorRemoved;

        private async void Start()
        {
            await UniTask.WaitForFixedUpdate();

            _initialEffectConfigs.ForEach(modificatorData => { AddModificator(modificatorData); });
        }

        public int GetEffectsAmount(EntityModificatorConfig modificatorConfig) => _appliedEffects.Get(modificatorConfig).Count;

        public void AddModificator(EntityModificatorConfig modificatorConfig)
        {
            if (_effectFactory.EffectCanBeApplied(modificatorConfig, _ownerEntity))
            {
                Effect effect = _effectFactory.CreateEntityModificator(modificatorConfig, _ownerEntity);

                _appliedEffects.Add(modificatorConfig, effect);

                effect.Enable().Forget();
                
                ModificatorApplied?.Invoke(modificatorConfig);
            }
        }

        public bool Has(EntityModificatorConfig modificatorConfig)
        {
            return _appliedEffects.GetAllKeys().Contains(modificatorConfig);
        }

        public void RemoveModificator(EntityModificatorConfig modificatorConfig)
        {
            if (_appliedEffects.Contains(modificatorConfig))
            {
                Effect effect = _appliedEffects.Remove(modificatorConfig);

                effect.Disable();
                
                ModificatorRemoved?.Invoke(modificatorConfig);
            }
        }

        public List<EntityModifcatorTag> GetAppliedTags()
        {
            List<EntityModifcatorTag> entityModifcatorTags = new();

            _appliedEffects.GetAllKeys().ForEach(modificatorData =>
            {
                for (int i = 0; i < _appliedEffects.Get(modificatorData).Count; i++)
                {
                    modificatorData.Tags.ForEach(tag => entityModifcatorTags.Add(tag));
                }
            });

            return entityModifcatorTags;
        }
    }
}