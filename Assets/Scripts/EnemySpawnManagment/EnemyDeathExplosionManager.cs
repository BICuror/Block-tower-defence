using UnityEngine;
using Cashing;
using Combat;

public sealed class EnemyDeathExplosionManager : MonoBehaviour
{
    [Cached] private EntityHealth _entityHealth;
    [SerializeField] private VisualEffectHandler _visualEffectHandler;    

    private void Start()
    {
        _entityHealth.EntityDied += PlayExplosionVFX;
    }

    private void PlayExplosionVFX(CombatEntity enemyObject)
    {
        _visualEffectHandler.PlayBurstEffectAndForget();
    }
}