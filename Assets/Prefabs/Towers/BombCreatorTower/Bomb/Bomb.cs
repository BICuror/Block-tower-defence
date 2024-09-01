using UnityEngine;
using UnityEngine.Events;

public sealed class Bomb : DraggableObject
{  
    [SerializeField] private VisualEffectHandler _visualEffectHandler; 
    [SerializeField] private LayerSetting _enemyLayerSettings;

    [Header("BombStats")]
    [SerializeField] private float _explotionRadius;
    [SerializeField] private float _explotionDamage;
    [SerializeField] private float _preparationTime; 
    public void SetExplotionDamage(float value) => _explotionDamage = value;

    public UnityEvent Exploded;

    private void Start() => Placed.AddListener(PrepeareToExplode);

    private void PrepeareToExplode()
    {
        SetDraggableState(false);

        Invoke("Explode", _preparationTime);
    }

    private void Explode()
    {
        Collider[] hitEntities = Physics.OverlapSphere(transform.position, _explotionRadius, _enemyLayerSettings.GetLayerMask());

        for (int i = 0; i < hitEntities.Length; i++)
        {
            hitEntities[i].transform.gameObject.GetComponent<EntityHealth>().GetHurt(_explotionDamage);
        }

        _visualEffectHandler.Play();
        
        Exploded.Invoke();
    }
}
