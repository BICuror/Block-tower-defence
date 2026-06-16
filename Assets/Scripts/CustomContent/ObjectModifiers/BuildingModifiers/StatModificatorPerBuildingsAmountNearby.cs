using NaughtyAttributes;
using UnityEngine;
using Cashing;
using System;
using Combat;

public sealed class StatModificatorPerBuildingsAmountNearby : EntityObjectModifier
{
    [SerializeField] private bool _useStackableNearbyModifier;
    [SerializeField] private string _statType;
    [SerializeField] private float _nearbyModifier;
    [SerializeField] private float _aloneModifier;
    [SerializeField] private AreaEntityDetector _buildingAreaScaner;
    [Header("UI")] [SerializeField] private bool _showIconWhenAlone;
    [ShowIf("_showIconWhenAlone")] [SerializeField] private Sprite _aloneIcon;
    [SerializeField] private bool _showIconWhenNotAlone;
    [ShowIf("_showIconWhenNotAlone")] [SerializeField] private Sprite _notAloneIcon;
    [Cached] private EntityCanvas _entityCanvas;
    [Cached] private CombatEntity _ownerEntity;
    private EntityCanvasIcon _entityCanvasIcon;
    private StatModifier _statModifier = new();
    
    private void Start()
    {
        _ownerEntity.StatContainer.Get(GetStatType()).AddStatModifier(_statModifier);
        
        _buildingAreaScaner.AddedItem += RecalculateDamageBoost;
        _buildingAreaScaner.RemovedItem += RecalculateDamageBoost;
        
        RecalculateDamageBoost();
    }
    
    private Type GetStatType() => Type.GetType(_statType);
    
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

        UpdateIconUI();
    }

    private void UpdateIconUI()
    {
        bool isAlone = _buildingAreaScaner.Count == 0;
        
        bool shouldBeEnabled = (isAlone && _showIconWhenAlone) || (!isAlone && _showIconWhenNotAlone);

        if (shouldBeEnabled && !_entityCanvasIcon) _entityCanvasIcon = _entityCanvas.AddIcon(_aloneIcon, _useStackableNearbyModifier);
        else if (!shouldBeEnabled && _entityCanvasIcon) _entityCanvas.RemoveIcon(_entityCanvasIcon);
        
        if (!_entityCanvasIcon.gameObject) return;
        
        if (isAlone) _entityCanvasIcon.SetIcon(_aloneIcon);
        else _entityCanvasIcon.SetIcon(_notAloneIcon);
        
        if (_useStackableNearbyModifier && _entityCanvasIcon) _entityCanvasIcon.SetValue(_buildingAreaScaner.Count);
    }
    
    private void OnDestroy()
    {
        _ownerEntity.StatContainer.Get(GetStatType()).RemoveStatModifier(_statModifier);
        
        _buildingAreaScaner.AddedItem -= RecalculateDamageBoost;
        _buildingAreaScaner.RemovedItem -= RecalculateDamageBoost;
        
        if (_entityCanvasIcon) _entityCanvas.RemoveIcon(_entityCanvasIcon);
    }
}