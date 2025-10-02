using Cysharp.Threading.Tasks;
using UnityEngine;
using Combat;

public sealed class AOE : WeaponBase
{
    [SerializeField] private VisualEffectHandler _explotionEffect;
    [SerializeField] private LayerSetting _enemyLayerSettings;
    [SerializeField] private float _defaultRadius = 1f;

    [SerializeField] private float _duration = 5f;
    [SerializeField] private float _secondsPerHit = 1f;
    
    private AOEDamageMultiplier _aoeDamageMultiplier;
    private AOERadius _radius;

    protected override void OnInitialized()
    {
        _aoeDamageMultiplier = OwnerEntity.StatContainer.Get<AOEDamageMultiplier>();
        _radius = OwnerEntity.StatContainer.Get<AOERadius>();
    }

    public async UniTask ActiveAOE()
    {
        UpdateAOERadius(_radius.Value);
        _explotionEffect.Play();

        float elapsedTime = 0f;

        while (elapsedTime <= _duration)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _radius.Value * _defaultRadius, _enemyLayerSettings.GetLayerMask());
    
            for (int i = 0; i < hitEnemies.Length; i++)
            {
                DamageEntity(_aoeDamageMultiplier.Value * GetDamageValue(), hitEnemies[i].GetComponent<CombatEntity>());
            }
            
            try
            {
                await UniTask.WaitForSeconds(_secondsPerHit, cancellationToken: destroyCancellationToken);
            }
            catch { return; }
            
            elapsedTime += _secondsPerHit;
        }

        await _explotionEffect.StopAsync();
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
        Gizmos.DrawSphere(transform.position, _defaultRadius);
    }
#endif
}