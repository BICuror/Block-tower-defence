using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;
using Combat;
using System;

public class ApplyEffectOnceToEntitiesInArea : EntityObjectModifier
{
    [Inject] private WaveStateMachine _waveStateMachine;

    [Header("Links")]
    [SerializeField] private VisualEffectHandler _visualEffectHandler;
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    [SerializeField] private string _effectTypeName;
    [SerializeField] private float _duration;
    [SerializeField] private int _effectStaks;

    [SerializeField] private bool _canBeCastOutOfAttackState = false;

    [Header("Charges")] 
    [SerializeField] private bool _hasCharges;
    [ShowIf("_hasCharges")] [SerializeField] protected int MaxCharges;
    private Type _effectType;
    protected int CurrentCharges;
    
    protected void Start()
    {
        _effectType = Type.GetType(_effectTypeName);
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted += TryRefillCharge;
    }

    protected void ApplyEffect()
    {
        if (!_canBeCastOutOfAttackState && _waveStateMachine.CurrentState != WaveState.Attack) return;
        
        if (_hasCharges && CurrentCharges <= 0) return;
        
        IReadOnlyList<CombatEntity> entitiesInArea = _areaEntityDetector.GetList();

        for (int i = 0; i < entitiesInArea.Count; i++)
        {
            if (_duration > 0)
            {
                entitiesInArea[i].ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(_effectType, _effectStaks, _duration);
            }
            else
            {
                entitiesInArea[i].ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_effectType, _effectStaks);
            }
        }

        _visualEffectHandler.PlayBurstEffectAndForget();
        
        if (!_hasCharges) return;

        SetCharges(CurrentCharges - 1);
    } 
    
    private void TryRefillCharge() => SetCharges(MaxCharges);

    private void SetCharges(int value)
    {
        CurrentCharges = value;
        
        OnChargesValueChanged();
    }

    protected virtual void OnChargesValueChanged() {}
    
    protected void OnDestroy()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted -= TryRefillCharge;
        SetCharges(0);
    }
}