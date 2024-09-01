using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class EnemyAreaScaner : MonoBehaviour
{   
    private List<EnemyHealth> _enemyList = new List<EnemyHealth>();

    public UnityEvent<EnemyHealth> EnemyEnteredArea;
    public UnityEvent<EnemyHealth> EnemyExitedArea;
    
    public bool Empty() => _enemyList.Count == 0;

    public EnemyHealth GetRandomEnemy() => _enemyList[Random.Range(0, _enemyList.Count)];

    public EnemyHealth GetFirstEnemy() => _enemyList[0];
    
    public List<EnemyHealth> GetAllEnemies() => _enemyList;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
        {
            _enemyList.Add(enemyHealth);

            enemyHealth.EnemyDeathEvent.AddListener(RemoveFromList);

            EnemyEnteredArea?.Invoke(enemyHealth);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
        {
            _enemyList.Remove(enemyHealth);

            enemyHealth.EnemyDeathEvent.RemoveListener(RemoveFromList);
            
            EnemyExitedArea?.Invoke(enemyHealth);
        }
    }

    private void RemoveFromList(EnemyHealth enemyHealth)
    {
        enemyHealth.EnemyDeathEvent.RemoveListener(RemoveFromList);

        _enemyList.Remove(enemyHealth);

        EnemyExitedArea?.Invoke(enemyHealth);
    }
}
