using UnityEngine;
using Cashing;
using Combat;

public sealed class EnemyAreaHeal : MonoBehaviour
{
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;
    [Cached] private DraggableObject _draggableObject;

    private void Start()
    {
        _enemyAreaScaner.AddedItem += ApplyHealEffect;
        _enemyAreaScaner.RemovedItem += RemoveHealEffect;
    }
    
    private void ApplyHealEffect(CombatEntity entity) => entity.ComponentsContainer.Get<EntityEffectManager>().TryApplyEffect(typeof(HealEffect), 1);
    
    private void RemoveHealEffect(CombatEntity entity) => entity.ComponentsContainer.Get<EntityEffectManager>().RemoveEffect(typeof(HealEffect), 1);

    private void OnDestroy()
    {
        _enemyAreaScaner.AddedItem -= ApplyHealEffect;
        _enemyAreaScaner.RemovedItem -= RemoveHealEffect;
    }
}
