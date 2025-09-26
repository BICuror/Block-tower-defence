using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;
using System;

public sealed class ApplyEffectToAllEnemiesOnActivation : EntityObjectModifier
{
    [Cached] private CombatEntity _ownerEntity;
    [Inject] private WaveStateMachine _waveStateMachine;
    [SerializeField] private VisualEffectHandler _visualEffectHandler;
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    [SerializeField] private string _effectTypeName;
    [SerializeField] private int _effectStaks;
    [SerializeField] private bool _hasCharges;
    [ShowIf("_hasCharges")] [SerializeField] private int _maxCharges;
    private Type _effectType;
    private int _charges;
    
    private void Start()
    {
        _effectType = Type.GetType(_effectTypeName);

        _ownerEntity.Activated += ApplyEffect;

        _waveStateMachine.StateStarted += TryRefillCharge;
    }

    private void TryRefillCharge(WaveState waveState)
    {
        if (_waveStateMachine.CurrentState == WaveState.Attack)
        {
            _charges = _maxCharges;
        }
    }

    private void ApplyEffect()
    {
        if (_hasCharges && _charges <= 0) return;
        
        IReadOnlyList<CombatEntity> entitiesInArea = _areaEntityDetector.GetList();

        for (int i = 0; i < entitiesInArea.Count; i++)
        {
            entitiesInArea[i].ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_effectType, _effectStaks);
        }

        _visualEffectHandler.Play();
        
        _charges--;
    } 

    private void OnDestroy()
    {
        _ownerEntity.Activated -= ApplyEffect;
        
        _waveStateMachine.StateStarted -= TryRefillCharge;
    }
}