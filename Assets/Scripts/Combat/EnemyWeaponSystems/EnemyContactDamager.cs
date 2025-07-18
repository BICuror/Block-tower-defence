using UnityEngine;
using Cashing;
using Combat;

public sealed class EnemyContactDamager : MonoBehaviour
{
    [Cached] private ContactDamage _contactDamage;
    [Cached] private CombatEntity _ownerEntity;
        
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<BuildingEntity>(out BuildingEntity buildingEntity))
        {
            buildingEntity.Health.ReceiveEnemyDamage(_contactDamage.Value, _ownerEntity);
            
            _ownerEntity.Health.Die();
        }
    }
}