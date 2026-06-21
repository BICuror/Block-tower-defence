using NaughtyAttributes;
using UnityEngine;
using System;
using Combat;

public sealed class ApplyEffectInArea : EntityObjectModifier
{
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    [SerializeField] private bool _applyDataFromInspector;
    [ShowIf("_applyDataFromInspector")] [SerializeField] private int _effectStacks;
    [ShowIf("_applyDataFromInspector")] [SerializeField] private string _effectTypeName;
    
    private Type _effectType;
    
    private void Start() => Enable();

    public void Enable()
    {
        if (!_applyDataFromInspector)
        {
            _effectType = Type.GetType(Args.GetArgument<string>("EffectTypeName"));
            _effectStacks = Args.GetArgument<int>("EffectStacks");
        }
        else _effectType = Type.GetType(_effectTypeName);
        
        _areaEntityDetector.AddedItem += ApplyEffect;
        _areaEntityDetector.RemovedItem += RemoveEffect;
                
        foreach (CombatEntity combatEntity in _areaEntityDetector.GetList())
        {
            ApplyEffect(combatEntity);
        }
    }
    
    public void Disable()
    {
        _areaEntityDetector.AddedItem -= ApplyEffect;
        _areaEntityDetector.RemovedItem -= RemoveEffect;
        
        foreach (CombatEntity combatEntity in _areaEntityDetector.GetList())
        {
            RemoveEffect(combatEntity);
        }
    }
    
    private void ApplyEffect(CombatEntity entity) => entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_effectType, _effectStacks);

    private void RemoveEffect(CombatEntity entity) => entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(_effectType, _effectStacks);

    private void OnDestroy() => Disable();
    
    public override bool CanBeAppliedToEntity(CombatEntity entity, ArgumentsContainer argumentsContainer)
    {
        return entity.StatContainer.Has<ReachAreaScale>() && entity.ComponentsContainer.Has<AreaManager>();
    }
}