using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cashing;
using Combat;
using System;
using Cysharp.Threading.Tasks;

public class ApplyEffectOnceToEntitiesInArea : EntityObjectModifier
{
    [Inject] private WaveStateMachine _waveStateMachine;

    [Header("UI")] 
    [SerializeField] private Sprite _iconSprite;
    private EntityCanvasAbilityIcon _abilityIcon;
    [Cached] private EntityCanvas _canvas;
    
    [Header("Links")]
    [SerializeField] private VisualEffectHandler _visualEffectHandler;
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    [SerializeField] private string _effectTypeName;
    [SerializeField] private float _duration;
    [SerializeField] private int _effectStaks;

    [SerializeField] private bool _canBeCastOutOfAttackState = false;

    [Header("Charges")] 
    [SerializeField] private bool _hasCharges;
    [SerializeField] private int _maxCharges;
    private Type _effectType;
    private int _charges;
    
    protected void Start()
    {
        _effectType = Type.GetType(_effectTypeName);
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted += TryRefillCharge;
        _abilityIcon = _canvas.AddAbilityIcon(_iconSprite, (float)_charges / _maxCharges);
    }

    protected void ApplyEffect()
    {
        if (!_canBeCastOutOfAttackState && _waveStateMachine.CurrentState != WaveState.Attack) return;
        
        if (_hasCharges && _charges <= 0) return;
        
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
        
        _charges--;
        UpdateIconState();
    } 
    
    private void TryRefillCharge()
    {
        _charges = _maxCharges;
        UpdateIconState();
    }

    private void UpdateIconState()
    {
        _abilityIcon.SetValue((float)_charges / _maxCharges).Forget();
    }
    
    private void OnDestroy()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted -= TryRefillCharge;
        _charges = 0;
        _canvas.RemoveAbilityIcon(_abilityIcon);
        UpdateIconState();
    }
}