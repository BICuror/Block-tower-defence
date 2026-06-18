using UnityEngine;
using Cashing;
using System;
using Combat;

public sealed class StatModificatorPerBuildingsAmountNearby : EntityObjectModifier
{
    [SerializeField] private AreaEntityDetector _areaScaner;
    
    [Cached] private EntityCanvas _entityCanvas;
    [Cached] private CombatEntity _ownerEntity;
    
    private Type _statType;
    
    private float _flatChangeWhenNoEntitiesInArea;
    private float _multChangeWhenNoEntitiesInArea;
    private float _flatChangePerEntity;
    private float _flatChangeMax;
    private float _multChangePerEntity;
    private float _multChangeMax;
    
    private bool _showIconWhenNoEntitiesInArea;
    private bool _showIconWhenEntitiesInArea;
    private bool _displayEntitiesInAreaAmount;
    
    private Sprite _hasEntitiesInAreaIcon;
    private Sprite _noEntitiesInAreaIcon;
    
    private EntityCanvasIcon _entityCanvasIcon;
    private StatModifier _statModifier = new();
    
    private void Start()
    {
        _statType = Type.GetType(Args.GetArgument<string>("StatTypeName"));
        
        _hasEntitiesInAreaIcon = Args.GetArgumentWithDefaultValue<Sprite>("HasEntitiesInAreaIcon", null);
        _noEntitiesInAreaIcon = Args.GetArgumentWithDefaultValue<Sprite>("NoEntitiesInAreaIcon", null);

        _showIconWhenNoEntitiesInArea = _noEntitiesInAreaIcon;
        _showIconWhenEntitiesInArea = _hasEntitiesInAreaIcon;
        
        _displayEntitiesInAreaAmount = Args.GetArgumentWithDefaultValue<bool>("DisplayValueWithIcon", false);
        
        _flatChangeWhenNoEntitiesInArea = Args.GetArgumentWithDefaultValue<float>("FlatChangeWhenNoEntitiesInArea", 0);
        _multChangeWhenNoEntitiesInArea = Args.GetArgumentWithDefaultValue<float>("MultChangeWhenNoEntitiesInArea", 0);
        
        _flatChangePerEntity = Args.GetArgumentWithDefaultValue<float>("FlatChangePerEntity", 0);
        _flatChangeMax = Args.GetArgumentWithDefaultValue<float>("FlatChangeMax", 0);
        
        _multChangePerEntity = Args.GetArgumentWithDefaultValue<float>("MultChangePerEntity", 0);
        _multChangeMax = Args.GetArgumentWithDefaultValue<float>("MultChangeMax", 0);
        
        _ownerEntity.StatContainer.Get(_statType).AddStatModifier(_statModifier);
        _areaScaner.RemovedItem += RecalculateDamageBoost;
        _areaScaner.AddedItem += RecalculateDamageBoost;
        RecalculateDamageBoost();
    }
    
    public override bool CanBeAppliedToEntity(CombatEntity entity, ArgumentsContainer argumentsContainer) => entity.StatContainer.Has(Type.GetType(argumentsContainer.GetArgument<string>("StatTypeName"))); 

    private void RecalculateDamageBoost(CombatEntity _) => RecalculateDamageBoost();
    
    private void RecalculateDamageBoost()
    {
        int entitiesInArea = _areaScaner.Count;
        
        if (entitiesInArea == 0)
        {
            _statModifier.SetFlat(_flatChangeWhenNoEntitiesInArea);
            _statModifier.SetMultiplier(_multChangeWhenNoEntitiesInArea);
        }
        else
        {
            float flatChange = Mathf.Clamp(entitiesInArea * _flatChangePerEntity, -_flatChangeMax, _flatChangeMax);
            _statModifier.SetFlat(flatChange);

            float multChange = Mathf.Clamp(entitiesInArea * _multChangePerEntity, -_multChangeMax, _multChangeMax);
            _statModifier.SetMultiplier(multChange);
        }

        UpdateIconUI();
    }

    private void UpdateIconUI()
    {
        bool hasEntitiesInArea = !_areaScaner.IsEmpty;
        
        bool shouldBeEnabled = (hasEntitiesInArea && _showIconWhenEntitiesInArea) || 
                               (!hasEntitiesInArea && _showIconWhenNoEntitiesInArea);

        if (shouldBeEnabled && !_entityCanvasIcon) _entityCanvasIcon = _entityCanvas.AddIcon(_hasEntitiesInAreaIcon, _displayEntitiesInAreaAmount);
        else if (!shouldBeEnabled && _entityCanvasIcon) _entityCanvas.RemoveIcon(_entityCanvasIcon);
        
        if (!_entityCanvasIcon) return;
        
        if (hasEntitiesInArea) _entityCanvasIcon.SetIcon(_hasEntitiesInAreaIcon);
        else _entityCanvasIcon.SetIcon(_noEntitiesInAreaIcon);
        
        if (_displayEntitiesInAreaAmount && _entityCanvasIcon) _entityCanvasIcon.SetValue(_areaScaner.Count);
    }
    
    private void OnDestroy()
    {
        _ownerEntity.StatContainer.Get(_statType).RemoveStatModifier(_statModifier);
        
        _areaScaner.AddedItem -= RecalculateDamageBoost;
        _areaScaner.RemovedItem -= RecalculateDamageBoost;
        
        if (_entityCanvasIcon) _entityCanvas.RemoveIcon(_entityCanvasIcon);
    }
}