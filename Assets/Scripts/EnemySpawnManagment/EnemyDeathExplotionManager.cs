using UnityEngine;

public sealed class EnemyDeathExplotionManager : MonoBehaviour
{
    [SerializeField] private VisualEffectHandler _visualEffectHandler;    

    private void Awake()
    {
        GetComponent<EnemyHealth>().DeathEvent.AddListener(PlayExplotionVFX);
    }

    public void PlayExplotionVFX(GameObject enemyObject)
    {
        _visualEffectHandler.transform.SetParent(null);

        _visualEffectHandler.transform.position = enemyObject.transform.position;

        _visualEffectHandler.Play();
    }
}
