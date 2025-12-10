using UnityEngine;
using Cashing;
using System;
using Combat;

public sealed class StatModificatorPerBuildingsAmountNearby : EntityObjectModifier
{
    [SerializeField] private string _statType;
    [SerializeField] private bool _useStackableNearbyModifier;
    [SerializeField] private float _aloneModifier;
    [SerializeField] private float _nearbyModifier;
    [SerializeField] private AreaEntityDetector _buildingAreaScaner;
    [Cached] private CombatEntity _ownerEntity;
    private StatModifier _statModifier = new StatModifier();
    private Type _assignedStatType;
    
    private void Start()
    {
        _ownerEntity.StatContainer.Get(GetStatType()).AddStatModifier(_statModifier);
        
        _buildingAreaScaner.AddedItem += RecalculateDamageBoost;
        _buildingAreaScaner.RemovedItem += RecalculateDamageBoost;
        
        RecalculateDamageBoost();
    }
    
    public override bool CanBeAppliedToEntity(CombatEntity entity) => entity.StatContainer.Has<ReachAreaScale>() && entity.StatContainer.Has(GetStatType()); 

    private void RecalculateDamageBoost(CombatEntity _) => RecalculateDamageBoost();
    
    private void RecalculateDamageBoost()
    {
        if (_buildingAreaScaner.Count == 0)
        {
            _statModifier.SetMultiplier(_aloneModifier);
        }
        else
        {
            if (_useStackableNearbyModifier) _statModifier.SetMultiplier(_nearbyModifier * _buildingAreaScaner.Count);
            else _statModifier.SetMultiplier(_nearbyModifier);
        }
    }
    
    private Type GetStatType()
    {
        if (_assignedStatType == null)
        {
            _assignedStatType = Type.GetType(_statType);
        }
        
        return _assignedStatType;
    }


    private void OnDestroy()
    {
        _ownerEntity.StatContainer.Get(GetStatType()).RemoveStatModifier(_statModifier);
        
        _buildingAreaScaner.AddedItem -= RecalculateDamageBoost;
        _buildingAreaScaner.RemovedItem -= RecalculateDamageBoost;
    }
}