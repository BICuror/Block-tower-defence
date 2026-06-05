using Cysharp.Threading.Tasks;
using System.Threading;
using Zenject;
using System;
using Combat;

public sealed class ApplyEffectOnActivation : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    private CancellationTokenSource _cancellationTokenSource = new();
    private EntityCanvasAbilityIcon _abilityIcon;
    
    private int _maxActivationStacks;
    private float _effectDuration;
    private int _effectStacks;
    private Type _effectType;
    
    private int _currentActivationStacks;
    private bool _isActive;

    public override bool CanBeApplied() => Entity.StatContainer.Has<TaskRechargeDuration>();
    
    public override void Enable()
    {
        _effectType = Type.GetType(Args.GetArgument<string>("EffectType"));
        _maxActivationStacks = Args.GetArgument<int>("ActivationStacks");
        _effectDuration = Args.GetArgument<float>("Duration");
        _effectStacks = Args.GetArgument<int>("Stacks");
        
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted += EnableAbility;
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateStarted += DisableAbility;
        Entity.Activated += Activate;
        
        _abilityIcon = AddAbilityIcon(0);
    }

    private void EnableAbility()
    {
        _currentActivationStacks = _maxActivationStacks;
        _isActive = false;
        
        UpdateAbilityIcon();
    }

    private void DisableAbility()
    {
        _currentActivationStacks = 0;
        UpdateAbilityIcon();
    }
    
    private void Activate()
    {
        if (_isActive || _waveStateMachine.CurrentState != WaveState.Attack || _currentActivationStacks <= 0) return;
        
        _isActive = true;

        _currentActivationStacks--;
        UpdateAbilityIcon();
        
        Entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(_effectType, _effectStacks);
        StartCountdownToDeactivation().Forget();
    }
    
    private async UniTask StartCountdownToDeactivation()
    { 
        await UniTask.WaitForSeconds(_effectDuration, cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow();
        
        Deactivate();
    }
    
    private void Deactivate()
    {
        _isActive = false;
        
        Entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(_effectType, _effectStacks);
    }
    
    private void UpdateAbilityIcon()
    {
        _abilityIcon.SetValue((float)_currentActivationStacks / _maxActivationStacks).Forget();
    }

    public override void Disable()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).EnteredStateCompleted -= EnableAbility;
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateStarted -= DisableAbility;
        Entity.Activated -= Activate;
        
        _cancellationTokenSource.Cancel();
        
        RemoveAbilityIcon(_abilityIcon);
    }
}