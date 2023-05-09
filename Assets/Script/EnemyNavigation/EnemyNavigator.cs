using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigator : MonoBehaviour
{
    public void SetDestination(Vector3 destination)
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(destination);
    }
}
