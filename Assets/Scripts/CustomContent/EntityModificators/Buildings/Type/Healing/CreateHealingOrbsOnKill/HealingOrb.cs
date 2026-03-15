using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;
using System;
using Combat;

public sealed class HealingOrb : MonoBehaviour
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    [Header("Stats")] 
    [Range(0f, 1f)] [SerializeField] private float _healPercent = 0.05f;
    [SerializeField] private float _healPulseAmplitude = 1f;
    [SerializeField] private int _healPulsesAmount = 3;
        
    [Header("Links")]
    [SerializeField] private VisualEffectHandler _visualEffectHandler;
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    [SerializeField] private DraggableObject _draggableObject;
    private CancellationTokenSource _cancellationTokenSource = new();
    private int _healPulsesLeft;
    private bool _isUsed;
    
    private void Awake()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted += DestroyOrbSync;

        _healPulsesLeft = _healPulsesAmount;
        _draggableObject.PickedUp += StopHealingProcess;
        _draggableObject.Placed += StartHealing;
        
        if (_waveStateMachine.CurrentState != WaveState.Attack) DestroyOrb().Forget();
    }

    private void StartHealing() => StartHealingProcess().Forget();

    private async UniTask StartHealingProcess()
    {
        _isUsed = true;
        
        while (_healPulsesLeft > 0)
        {
            try
            {
                await UniTask.WaitForSeconds(_healPulseAmplitude, cancellationToken: _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                e.LogAsync();
                _isUsed = false;
                return;
            }
            
            _healPulsesLeft--;
            HealAllBuildingsInArea();
            await _visualEffectHandler.PlayBurstEffect();
        }
        
        _isUsed = false;
        DestroyOrb().Forget();
    }

    private void HealAllBuildingsInArea()
    {
        foreach (CombatEntity combatEntity in _areaEntityDetector.GetList())
        {
            combatEntity.Health.ReceivePercentHeal(_healPercent);
        }
    }
    
    private void StopHealingProcess()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    private void DestroyOrbSync() => DestroyOrb().Forget();
    private async UniTask DestroyOrb()
    {
        await UniTask.WaitUntil(() => _draggableObject.IsPlaced && !_isUsed);
        
        Destroy(gameObject);
    } 

    private void OnDestroy()
    {
        _waveStateMachine.GetWaveStateController(WaveState.Attack).QuitStateCompleted -= DestroyOrbSync;
        
        StopHealingProcess();
    }
}