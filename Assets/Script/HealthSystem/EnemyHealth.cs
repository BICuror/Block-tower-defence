using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : EntityHealth
{
    public UnityEvent<EnemyHealth> DeathEvent; 

    [SerializeField] private float _damageDecreaseSpeed = 0.1f;

    private void OnCollisionStay(Collision other) 
    {
        if (other.gameObject.TryGetComponent(out BuildingHealth buildingHealth))
        {
            GetHurt(_damageDecreaseSpeed);
            buildingHealth.GetHurt(_damageDecreaseSpeed);
        }    
    }

    private void OnDestroy() => DeathEvent?.Invoke(this);
}
