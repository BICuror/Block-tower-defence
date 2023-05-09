using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortireTower : MonoBehaviour
{
    [SerializeField] private GameObject _core;
    
    [SerializeField] private float _angle;

    [SerializeField] private int _damage;
    
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;

    private void Start()
    {   
        TaskCycle buildingTaskCycle = GetComponent<TaskCycle>();

        buildingTaskCycle.ShouldWorkDelegate = ShouldWorkDelegate;

        buildingTaskCycle.TaskPerformed.AddListener(Shoot);
    }

    private bool ShouldWorkDelegate() => _enemyAreaScaner.Empty() == false;

    private void Shoot()
    {
        _core.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        _core.SetActive(true);

        _core.transform.position = transform.position;

        _core.transform.rotation = Quaternion.identity;

        _core.GetComponent<Rigidbody>().velocity = BallisticVelocity(_enemyAreaScaner.GetFirstEnemy().transform.position, _angle);

        _core.GetComponent<Weapon>().SetDamage(_damage);
    }

    private Vector3 BallisticVelocity(Vector3 destination, float _angle)
    {
        Vector3 dir = destination - transform.position; // get Target Direction
        float height = dir.y; // get height difference
        dir.y = 0; // retain only the horizontal difference
        float dist = dir.magnitude; // get horizontal direction
        float a = _angle * Mathf.Deg2Rad; // Convert _angle to radians
        dir.y = dist * Mathf.Tan(a); // set dir to the elevation _angle.
        dist += height / Mathf.Tan(a); // Correction for small height differences

        // Calculate the velocity magnitude
        float velocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * dir.normalized; // Return a normalized vector.
    }
}
