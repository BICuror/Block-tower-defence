using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;
using Cysharp.Threading.Tasks;

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
            AddEffect(modificatorData);
        });   
    }

    public int GetModificatorsAmount(EntityModificatorData modificatorData) => _appliedModificators.Get(modificatorData).Count;
    
    public void AddEffect(EntityModificatorData modificatorData)
    {
        List<EntityModificator> modificators = _entityModificatorFactory.CreateEntityModificators(modificatorData);

        for (int i = 0; i < modificators.Count; i++)
        {
            modificators[i].SetEntity(_ownerEntity);

            if (!modificators[i].CanBeApplied())
            {
                modificators.RemoveAt(i);
                i--;
                continue;
            }
                    
            modificators[i].Enable();
        }
        

        _appliedModificators.Add(modificatorData, modificators);
    }

    public bool Has(EntityModificatorData modificatorData)
    {
        return _appliedModificators.GetAllKeys().Contains(modificatorData);
    }
    
    public void RemoveEffect(EntityModificatorData modificatorData)
    {
        if (_appliedModificators.Contains(modificatorData))
        {
            List<EntityModificator> modificators = _appliedModificators.Remove(modificatorData);
            
            modificators.ForEach(modificator => modificator.Disable());
        }
    }
}