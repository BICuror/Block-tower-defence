using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortireTower : MonoBehaviour
{
    [SerializeField] private GameObject core;
    
    [SerializeField] private float angle;

    [SerializeField] private int damage;
    
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;

    private void Start()
    {   
        GetComponent<TaskCycle>().ShouldWorkDelegate = LOL;
    }

    public bool LOL() => _enemyAreaScaner.Empty() == false;

    public void Shoot()
    {
        core.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        core.SetActive(true);

        core.transform.position = transform.position;

        core.transform.rotation = Quaternion.identity;

        core.GetComponent<Rigidbody>().velocity = BallisticVelocity(_enemyAreaScaner.GetFirstEnemy().transform.position, angle);

        core.GetComponent<Weapon>().SetDamage(damage);
    }

    private Vector3 BallisticVelocity(Vector3 destination, float angle)
    {
        Vector3 dir = destination - transform.position; // get Target Direction
        float height = dir.y; // get height difference
        dir.y = 0; // retain only the horizontal difference
        float dist = dir.magnitude; // get horizontal direction
        float a = angle * Mathf.Deg2Rad; // Convert angle to radians
        dir.y = dist * Mathf.Tan(a); // set dir to the elevation angle.
        dist += height / Mathf.Tan(a); // Correction for small height differences

        // Calculate the velocity magnitude
        float velocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * dir.normalized; // Return a normalized vector.
    }
}
