using Combat;
using UnityEngine;

public sealed class EnemyDeathExplotionManager : MonoBehaviour
{
    [SerializeField] private VisualEffectHandler _visualEffectHandler;    

    private void Awake()
    {
        GetComponent<EntityHealth>().EntityDied += PlayExplotionVFX;
    }

    public void PlayExplotionVFX(CombatEntity enemyObject)
    {
        _visualEffectHandler.transform.SetParent(null);

        _visualEffectHandler.transform.position = enemyObject.transform.position;

        _visualEffectHandler.Play();
    }
}
