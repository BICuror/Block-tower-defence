using Cashing;
using Combat;
using UnityEngine;

public sealed class EnemyDeathExplotionManager : MonoBehaviour
{
    [Cached] private EntityHealth _entityHealth;
    [SerializeField] private VisualEffectHandler _visualEffectHandler;    

    private void Start()
    {
        _entityHealth.EntityDied += PlayExplotionVFX;
    }

    private void PlayExplotionVFX(CombatEntity enemyObject)
    {
        _visualEffectHandler.transform.SetParent(null);

        _visualEffectHandler.transform.position = enemyObject.transform.position;

        _visualEffectHandler.PlayAndStop();
    }
}
