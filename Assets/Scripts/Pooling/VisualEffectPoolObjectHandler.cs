using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public sealed class VisualEffectPoolObjectHandler : MonoBehaviour
{
    [SerializeField] private StopActionType _stopAction;

    [SerializeField] private VisualEffect _visualEffect;

    private YieldInstruction _rechargeInstruction;

    private void Awake()
    {
        float disableTime = _visualEffect.GetFloat("MaxLifeTime"); 

        _rechargeInstruction = new WaitForSeconds(disableTime);
    }

    public void Play()
    {
        _visualEffect.Play();
    }

    public void Stop()
    {
        _visualEffect.Stop();

        StartCoroutine(StartDisableingProcess());
    }

    private IEnumerator StartDisableingProcess()
    {
        yield return _rechargeInstruction;

        ExecuteStopAction();
    }

    private void ExecuteStopAction()
    {
        switch (_stopAction)
        {
            case StopActionType.Destroy: Destroy(gameObject); break;
            case StopActionType.Disable: gameObject.SetActive(false); break;
        }
    }

    private enum StopActionType
    {
        Destroy,
        Disable
    }
}
