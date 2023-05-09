using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyEffectStand : MonoBehaviour
{
    [SerializeField] private Effect[] _applyEffects;

    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;

    private void Start()
    {
        _enemyAreaScaner.EnemyEnteredArea.AddListener(AddEffect);

        _enemyAreaScaner.EnemyExitedArea.AddListener(RemoveEffect);
    }

    private void AddEffect(EnemyHealth enemy)
    {
        for (int i = 0; i < _applyEffects.Length; i++)
        {
            enemy.gameObject.GetComponent<EntityEffectManager>().ApplyEffect(_applyEffects[i]);
        }
    }
    
    private void RemoveEffect(EnemyHealth enemy)
    {
        for (int i = 0; i < _applyEffects.Length; i++)
        {
            enemy.gameObject.GetComponent<EntityEffectManager>().RemoveEffect(_applyEffects[i]);
        }
    }
    
    private void OnDestroy() => RemoveAllEffects();

    public void RemoveAllEffects()
    {
        List<EnemyHealth> enemies = _enemyAreaScaner.GetAllEnemies();

        for (int i = 0; i < enemies.Count; i++)
        {
            RemoveEffect(enemies[i]);
        }
    }
}
