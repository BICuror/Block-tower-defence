using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityModificatorsContainer : MonoBehaviour
{
    private readonly ListDictionary<EntityModificatorData, List<EntityModificator>> _appliedModificators = new();
    [SerializeField] private List<EntityModificatorData> _initialModificatorDatas;
    [SerializeField] private List<EntityModificatorData> _allAvailableModificators;
    [Inject] private EntityModificatorFactory _entityModificatorFactory;
    [Cached] private CombatEntity _ownerEntity;
        
    public List<EntityModificatorData> AvailableModificators => new List<EntityModificatorData>(_allAvailableModificators);
    public List<EntityModificatorData> AppliedModificators => _appliedModificators.GetAllKeys();

    private async void Start()
    {
        await UniTask.WaitForFixedUpdate();
        
        _initialModificatorDatas.ForEach(modificatorData =>
        {
            AddModificator(modificatorData);
        });   
    }

    public int GetModificatorsAmount(EntityModificatorData modificatorData) => _appliedModificators.Get(modificatorData).Count;
    
    public void AddModificator(EntityModificatorData modificatorData)
    {
        List<EntityModificator> modificators = _entityModificatorFactory.CreateEntityModificators(modificatorData);
        
        for (int i = 0; i < modificators.Count; i++)
        {
            modificators[i].SetEntity(_ownerEntity);
            modificators[i].SetEntityModificatorData(modificatorData);

            if (!modificators[i].CanBeApplied())
            {
                modificators.RemoveAt(i);
                i--;
                continue;
            }
                    
            modificators[i].Enable();
        }

        if (modificators.Count > 0) _appliedModificators.Add(modificatorData, modificators);
    }

    public bool Has(EntityModificatorData modificatorData)
    {
        return _appliedModificators.GetAllKeys().Contains(modificatorData);
    }
    
    public void RemoveModificator(EntityModificatorData modificatorData)
    {
        if (_appliedModificators.Contains(modificatorData))
        {
            List<EntityModificator> modificators = _appliedModificators.Remove(modificatorData);
            
            modificators.ForEach(modificator => modificator.Disable());
        }
    }

    public List<EntityModifcatorTag> GetAppliedTags()
    {
        List<EntityModifcatorTag> entityModifcatorTags = new();
        
        _appliedModificators.GetAllKeys().ForEach(modificatorData =>
        {
            for (int i = 0; i < _appliedModificators.Get(modificatorData).Count; i++)
            {
                modificatorData.Tags.ForEach(tag => entityModifcatorTags.Add(tag));
            }
        });

        return entityModifcatorTags;
    }
}