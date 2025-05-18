using Cysharp.Threading.Tasks;
using UnityEngine.VFX;
using UnityEngine;

public sealed class VisualEffectHandler : MonoBehaviour
{
    [SerializeField] private StopActionType _stopAction;
    [SerializeField] private VisualEffect _visualEffect;

    private Transform _initialParent;
    private float _disableTime;

    private void Awake()
    {
        _disableTime = _visualEffect.GetFloat("MaxLifeTime"); 
    }
    
    public void PlayAndForget() => Play().Forget();

    public async UniTask Play()
    {
        SetNullParent();
        
        _visualEffect.gameObject.SetActive(true);

        await UniTask.WaitForSeconds(_disableTime);
        
        switch(_stopAction)
        {
            case StopActionType.Disable: Disable(); break;
            case StopActionType.Destroy: Destroy(); break;
            case StopActionType.DisableEffect: DisableEffect(); break;
            case StopActionType.None: break;
        }    
    }

    private void DisableEffect()
    {
        _visualEffect.gameObject.SetActive(false);
        
        transform.SetParent(_initialParent);
    }

    private void Disable()
    {
        _visualEffect.gameObject.SetActive(false);

        gameObject.SetActive(false);
        
        transform.SetParent(_initialParent);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void SetNullParent()
    {
        if (_initialParent == null) _initialParent = transform.parent;
        
        transform.SetParent(null);
    }
    
    private enum StopActionType
    {
        Disable, 
        Destroy,
        DisableEffect,
        None
    }
}