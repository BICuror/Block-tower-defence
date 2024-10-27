using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon<T> : MonoBehaviour where T: EntityHealth
{
    [SerializeField] protected Collider Collider;
    [SerializeField] protected Rigidbody Rigidbody;
    public Rigidbody GetRigidbody() => Rigidbody;

    [SerializeField] private List<Effect> _effect;

    private float _contactDamage;

    public UnityEvent HitSomething;
    public UnityEvent HitEntity;
    public UnityEvent KilledEntity;

    private void OnEnable()
    {
        Collider.enabled = true;

        CancelInvoke();
        Invoke("Disable", 5f);
    }

    private void Disable() => gameObject.SetActive(false);

    private void OnTriggerEnter(Collider other)
    {
        Collider.enabled = false;

        if (other.gameObject.TryGetComponent(out T entityHealth))
        {
            DamageEntity(entityHealth, _contactDamage);
        
            HitEntity.Invoke();
        }

        HitSomething.Invoke();
    }

    protected virtual bool IsSutableTarget(T target) => true;

    protected void DamageEntity(EntityHealth entityHealth, float damageAmount)
    {
        entityHealth.GetHurt(damageAmount);

        if (entityHealth.IsAlive())
        {
            if (_effect.Count > 0)
            {
                EntityEffectManager effectManager = entityHealth.gameObject.GetComponent<EntityEffectManager>();

                for (int i = 0; i < _effect.Count; i++)
                {
                    if (entityHealth.GetCurrentHealth() > 0) effectManager.ApplyEffect(_effect[i]);
                }
            }
        }
        else
        {
            KilledEntity.Invoke();
        }
    }

    public void SetEffects(List<Effect> effects) => _effect = effects;

    public void SetContactDamage(float newDamage) => _contactDamage = newDamage;
}
