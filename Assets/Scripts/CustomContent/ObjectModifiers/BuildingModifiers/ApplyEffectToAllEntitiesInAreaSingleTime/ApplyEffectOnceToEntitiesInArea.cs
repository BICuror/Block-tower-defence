using System.Collections.Generic;
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
    
    private float _duration;
    private int _effectStacks;
    private bool _hasCharges;
    private Type _effectType;
    
    protected int MaxCharges;
    
    protected int CurrentCharges;
    
    protected void Start()
    {
        _effectType = Type.GetType(Args.GetArgument<string>("EffectTypeName"));
        _duration = Args.GetArgument<float>("EffectDuration");
        _effectStacks = Args.GetArgument<int>("EffectStacks");
        
        _hasCharges = Args.GetArgument<bool>("HasCharges");
        if (_hasCharges) MaxCharges = Args.GetArgument<int>("Chargers");
        
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted += TryRefillCharge;
    }

    protected void ApplyEffect()
    {
        if (_waveStateMachine.CurrentState != WaveState.Attack) return;
        
        if (_hasCharges && CurrentCharges <= 0) return;
        
        IReadOnlyList<CombatEntity> entitiesInArea = _areaEntityDetector.GetList();

        for (int i = 0; i < entitiesInArea.Count; i++)
        {
            if (_duration > 0)
            {
                entitiesInArea[i].ComponentsContainer.Get<EntityEffectManager>().TryApplyTemporaryEffect(_effectType, _effectStacks, _duration);
            }
            else
            {
                entitiesInArea[i].ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_effectType, _effectStacks);
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