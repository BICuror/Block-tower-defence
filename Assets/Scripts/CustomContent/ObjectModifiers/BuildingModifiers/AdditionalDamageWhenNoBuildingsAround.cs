using UnityEngine;
using Cashing;
using Combat;

public sealed class AdditionalDamageWhenNoBuildingsAround : MonoBehaviour
{
    [SerializeField] private AreaScanerController _areaScanerController;
    [SerializeField] private BuildingAreaScaner _buildingAreaScaner;
    [Cached] private CombatEntity _combatEntity;
    private StatModifier _statModifier = new StatModifier();
    
    private void Start()
    {
        _combatEntity.ComponentsContainer.Get<AreaManager>().AddAreaScanerController(_areaScanerController);

        _combatEntity.StatContainer.Get<Damage>().AddStatModifier(_statModifier);
        
        _buildingAreaScaner.AddedItem += RecalculateDamageBoost;
        _buildingAreaScaner.RemovedItem += RecalculateDamageBoost;
    }

    private void RecalculateDamageBoost(BuildingEntity _)
    {
        if (_buildingAreaScaner.Count == 0)
        {
            _statModifier.SetMultiplier(0.6f);
        }
        else
        {
            _statModifier.SetMultiplier(0f);
        }
    }

    private void OnDestroy()
    {
        _combatEntity.StatContainer.Get<Damage>().RemoveStatModifier(_statModifier);
        
        _buildingAreaScaner.AddedItem -= RecalculateDamageBoost;
        _buildingAreaScaner.RemovedItem -= RecalculateDamageBoost;
    }
}