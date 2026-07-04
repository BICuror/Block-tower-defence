using Cysharp.Threading.Tasks;
using UnityEngine;
using Tutorial;
using Zenject;

public sealed class TutorialContentController : MonoBehaviour
{
    [Inject] private WaveStateMachine _waveStateMachine;
    
    [SerializeField] private TutorialStep _roadGenerationThresholdStep;

    private void Awake()
    {
        IdleStateController idleStateController = _waveStateMachine.GetWaveStateController(WaveState.Idle) as IdleStateController;

        idleStateController.OnPreEnemyGroupGeneraton = AwaitTutorialStepCompletion;
    }

    private UniTask AwaitTutorialStepCompletion() => UniTask.WaitUntil(() => _roadGenerationThresholdStep.IsComplete);
}