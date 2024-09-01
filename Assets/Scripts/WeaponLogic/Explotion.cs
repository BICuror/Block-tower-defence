using UnityEngine;
using UnityEngine.Events;

public class Explotion : Weapon<EntityHealth>
{
    [SerializeField] private LayerSetting _enemyLayerSettings;

    [SerializeField] private float _explotionRadius;
    public void SetExplotionRaduis(float value) => _explotionRadius = value;

    [SerializeField] private float _explotionDamage;
    public void SetExplotionDamage(float value) => _explotionDamage = value;

    [SerializeField] private VisualEffectHandler _explotionEffect;

    public void Explode()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _explotionRadius, _enemyLayerSettings.GetLayerMask());

        for (int i = 0; i < hitEnemies.Length; i++)
        {
            DamageEntity(hitEnemies[i].transform.gameObject.GetComponent<EnemyHealth>(), _explotionDamage);
        }
        
        Instantiate(_explotionEffect, transform.position, Quaternion.identity).Play();

        Destroy(gameObject);
    }

    public void AwaitExplode(float time)
    {
        Invoke("Explode", time);
    }
}
