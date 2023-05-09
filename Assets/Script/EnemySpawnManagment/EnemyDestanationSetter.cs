using UnityEngine;
using UnityEngine.AI;

public sealed class EnemyDestanationSetter : MonoBehaviour
{
    [SerializeField] private Transform _townhall;

    public void SetTargetToEnemy(GameObject enemy)
    {
        enemy.GetComponent<NavMeshAgent>().SetDestination(_townhall.position);
    }
}
