using Cysharp.Threading.Tasks;
using System.Threading;
using NaughtyAttributes;
using UnityEngine;
using Combat;

public sealed class AOE : WeaponBase
{
    [SerializeField] private VisualEffectHandler _explotionEffect;
    [SerializeField] private LayerSetting _enemyLayerSettings;
    [SerializeField] private float _defaultRadius = 1f;

    [SerializeField] private bool _infiniteDuration;
    [HideIf("_infiniteDuration")] [SerializeField] private float _duration = 5f;
    [SerializeField] private float _secondsPerHit = 1f;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private AOEDamageMultiplier _aoeDamageMultiplier;
    private AOERadius _radius;
    
    public float AOERadius => _radius.Value * _defaultRadius + 0.5f;

    protected override void OnInitialized()
    {
        _aoeDamageMultiplier = OwnerEntity.StatContainer.Get<AOEDamageMultiplier>();
        _radius = OwnerEntity.StatContainer.Get<AOERadius>();
    }

    [Button] public void ActivateAOEDEBUG() => ActiveAOE().Forget();

    public async UniTask ActiveAOE()
    {
        UpdateAOERadius(AOERadius * 2);
        
        gameObject.SetActive(true);
        _explotionEffect.PlayBurstEffectAndForget();

        float elapsedTime = 0f;

        while (elapsedTime <= _duration || _infiniteDuration)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, AOERadius, _enemyLayerSettings.GetLayerMask());
    
            for (int i = 0; i < hitEnemies.Length; i++)
            {
                DamageEntity(_aoeDamageMultiplier.Value * GetDamageValue(), hitEnemies[i].GetComponent<CombatEntity>());
            }
            
            try
            {
                await UniTask.WaitForSeconds(_secondsPerHit, cancellationToken: _cancellationTokenSource.Token);
            }
            catch { return; }
            
            elapsedTime += _secondsPerHit;
        }

        await _explotionEffect.StopPermamentEffect();
        gameObject.SetActive(false);
    }

    public void DeactiveAOE()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
        _explotionEffect.StopPermamentEffect().Forget();
    }
    
    private void UpdateAOERadius(float explotionRaduis)
    {
        float scale = explotionRaduis;

        _explotionEffect.transform.localScale = new Vector3(scale, scale, scale);
    }

    private float GetDamageValue()
    {
        if (OwnerEntity.StatContainer.Has<Damage>()) return OwnerEntity.StatContainer.Get<Damage>().Value;
        
        return OwnerEntity.StatContainer.Get<ExplosionDamage>().Value;
    }

#if UNITY_EDITOR        
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, AOERadius);
    }
#endif
}