using Combat.IngameUI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Configs;
using Zenject;

public sealed class ActiveAbilityBarSubDomain : SubDomain
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    [Argument("BarSprite")] private Sprite _sprite;
    [Argument("ShowOnlyAtAttackState")] private bool _showOnlyAtAttackState;
    [Argument("DisableOnEmptyBar")] private bool _disableOnEmptyBar;
    [Argument("InitialValue")] private float _initialValue;
    
    private EntityCanvasAbilityIcon _abilityIcon;
    
    public EntityCanvasAbilityIcon AbilityIcon => _abilityIcon;
    
    public override UniTask Enable()
    {
        _abilityIcon = Args.Owner.ComponentsContainer.Get<EntityCanvas>().AddAbilityIcon(_sprite, _initialValue);
        _waveStateMachine.StateStarted += OnWaveStateStarted;

        UpdateIconState();
        
        return UniTask.CompletedTask;
    }
    
    public override UniTask Disable()
    {
        Args.Owner.ComponentsContainer.Get<EntityCanvas>().RemoveAbilityIcon(_abilityIcon);
        _waveStateMachine.StateStarted -= OnWaveStateStarted;
        
        return UniTask.CompletedTask;
    }
    
    private void OnWaveStateStarted(WaveState state) => UpdateIconState();
    private void UpdateIconState()
    {
        if (_disableOnEmptyBar && _abilityIcon.LastSetValue == 0)
        {
            _abilityIcon.SetActiveState(false);
            return;
        }
        
        if (_showOnlyAtAttackState)
        {
            _abilityIcon.SetActiveState(_waveStateMachine.GetWaveStateController(WaveState.Attack));
        }
        else _abilityIcon.SetActiveState(true);
    }
}

public sealed class SetAbilityIconValueCommand : SubDomainCommand<ActiveAbilityBarSubDomain>, ICommand<float>
{
    public UniTask Execute(float args)
    {
        SubDomain.AbilityIcon.SetValue(args);
        
        return UniTask.CompletedTask;
    }
}