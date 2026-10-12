using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;
using Combat.Effects;
using UnityEngine.Serialization;

public sealed class EntityModificatorsContainer : MonoBehaviour
{
    private readonly ListDictionary<EntityModificatorData, Effect> _appliedEffects = new();
    [FormerlySerializedAs("_initialModificatorDatas")] [SerializeField] private List<EntityModificatorData> _initialEffectConfigs;
    [Inject] private EffectFactory _effectFactory;
    [Cached] private CombatEntity _ownerEntity;
        
    public List<EntityModificatorData> AppliedModificators => _appliedEffects.GetAllKeys();

    private async void Start()
    {
        await UniTask.WaitForFixedUpdate();
        
        _initialEffectConfigs.ForEach(modificatorData =>
        {
            AddEffect(modificatorData);
        });   
    }

    public int GetEffectsAmount(EntityModificatorData modificatorData) => _appliedEffects.Get(modificatorData).Count;
    
    public void AddEffect(EntityModificatorData modificatorData)
    {
        if (_effectFactory.EffectCanBeApplied(modificatorData, _ownerEntity))
        {
            Effect effect = _effectFactory.CreateEntityModificator(modificatorData, _ownerEntity);

            _appliedEffects.Add(modificatorData, effect);
            
            effect.Enable().Forget();
        }
    }

    public bool Has(EntityModificatorData modificatorData)
    {
        return _appliedEffects.GetAllKeys().Contains(modificatorData);
    }
    
    public void RemoveModificator(EntityModificatorData modificatorData)
    {
        if (_appliedEffects.Contains(modificatorData))
        {
            Effect effect = _appliedEffects.Remove(modificatorData);

            effect.Disable();
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