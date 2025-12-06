using UnityEngine;
using System;
using Combat;

public sealed class ApplyEffectInArea : EntityObjectModifier
{
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    [SerializeField] private string _effectTypeName;
    [SerializeField] private int _effectStaks;
    private Type _effectType;

    private void Start()
    {
        _effectType = Type.GetType(_effectTypeName);
        
        foreach (CombatEntity combatEntity in _areaEntityDetector.GetList())
        {
            ApplyEffect(combatEntity);
        }

        _areaEntityDetector.AddedItem += ApplyEffect;
        _areaEntityDetector.RemovedItem += RemoveEffect;
    }

    private void ApplyEffect(CombatEntity entity) => entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_effectType, _effectStaks);

    private void RemoveEffect(CombatEntity entity) => entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(_effectType, _effectStaks);

    private void OnDestroy()
    {
        _areaEntityDetector.AddedItem -= ApplyEffect;
        _areaEntityDetector.RemovedItem -= RemoveEffect;
        
        foreach (CombatEntity combatEntity in _areaEntityDetector.GetList())
        {
            RemoveEffect(combatEntity);
        }
    }

    public override bool CanBeAppliedToEntity(CombatEntity entity)
    {
        return entity.StatContainer.Has<ReachAreaScale>() && entity.ComponentsContainer.Has<AreaManager>();
    }
}