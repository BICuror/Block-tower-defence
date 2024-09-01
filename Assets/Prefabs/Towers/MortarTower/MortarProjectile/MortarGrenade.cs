using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public sealed class MortarGrenade : PlayerWeapon
{
    [SerializeField] private LayerSetting _enemyLayerSettings;

    [SerializeField] private AnimationCurve _heightCurve;
    [SerializeField] private float _maxHeight;

    [SerializeField] private float _travelTime;

    [SerializeField] private VisualEffectHandler _visualEffectHandler; 

    public UnityEvent Enabled;
    public UnityEvent ReachedEnd;

    public UnityEvent Exploded;

    private float _explotionDamage;
    public void SetExplotionDamage(float value) => _explotionDamage = value;

    private float _explotionRadius;
    public void SetExplotionRaduis(float value) => _explotionRadius = value;

    private YieldInstruction _yieldInstruction = new WaitForFixedUpdate();

    private void Awake() => HitSomething.AddListener(Explode);
    
    private void OnEnable()
    {
        Collider.enabled = true;
     
        Enabled.Invoke();
    }

    public void Launch(Vector3 finalPositon)
    {
        StartCoroutine(MoveGrenade(finalPositon));
    }

    private IEnumerator MoveGrenade(Vector3 finalPosition)
    {   
        Vector3 startPosition = transform.position;

        float time = 0f;

        while (time < _travelTime)
        {
            float evaluatedTime = time / _travelTime;

            Vector3 currentPosition = Vector3.Lerp(startPosition, finalPosition, evaluatedTime);

            currentPosition.y += _heightCurve.Evaluate(evaluatedTime) * _maxHeight;

            transform.position = currentPosition;

            time += Time.deltaTime;

            yield return _yieldInstruction;
        }

        Collider.enabled = false;

        ReachedEnd.Invoke();

        Explode();
    }

    private void Explode()
    {
        StopAllCoroutines();

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _explotionRadius, _enemyLayerSettings.GetLayerMask());

        for (int i = 0; i < hitEnemies.Length; i++)
        {
            DamageEntity(hitEnemies[i].transform.gameObject.GetComponent<EnemyHealth>(), _explotionDamage);
        }

        _visualEffectHandler.Play();

        Exploded.Invoke();
    }
}
