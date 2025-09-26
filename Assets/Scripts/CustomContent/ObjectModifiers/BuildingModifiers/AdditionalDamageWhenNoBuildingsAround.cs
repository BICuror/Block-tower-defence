using UnityEngine;
using Cashing;
using Combat;

public sealed class AdditionalDamageWhenNoBuildingsAround : EntityObjectModifier
{
    [SerializeField] private AreaEntityDetector _buildingAreaScaner;
    [Cached] private Damage _damage;
    private StatModifier _statModifier = new StatModifier();
    
    private void Start()
    {

        _damage.AddStatModifier(_statModifier);
        
        _buildingAreaScaner.AddedItem += RecalculateDamageBoost;
        _buildingAreaScaner.RemovedItem += RecalculateDamageBoost;
    }

    private void RecalculateDamageBoost(CombatEntity _)
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
        _damage.RemoveStatModifier(_statModifier);
        
        _buildingAreaScaner.AddedItem -= RecalculateDamageBoost;
        _buildingAreaScaner.RemovedItem -= RecalculateDamageBoost;
    }
}