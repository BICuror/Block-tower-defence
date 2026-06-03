using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;
using Cashing;
using Combat;
using System;

public class ApplyEffectOnceToEntitiesInArea : EntityObjectModifier
{
    [Inject] private WaveStateMachine _waveStateMachine;

    [Header("UI")] 
    [SerializeField] private Sprite _iconSprite;
    private EntityCanvasIcon _entityCanvasIcon;
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
    [ShowIf("_hasCharges")] [SerializeField] private int _maxCharges;
    private Type _effectType;
    private int _charges;
    
    protected void Start()
    {
        _effectType = Type.GetType(_effectTypeName);

        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted += TryRefillCharge;
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
        if (!_hasCharges) return;

        if (_charges < 0 && _entityCanvasIcon)
        {
            _canvas.RemoveIcon(_entityCanvasIcon);
            _entityCanvasIcon = null;
            return;
        }

        if (_charges > 0 && !_entityCanvasIcon)
        {
            _entityCanvasIcon = _canvas.AddIcon(_iconSprite, true, _charges);
            return;
        }
        
        _entityCanvasIcon.SetValue(_charges);
    }
    
    private void OnDestroy()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted -= TryRefillCharge;
        _charges = 0;
        UpdateIconState();
    }
}