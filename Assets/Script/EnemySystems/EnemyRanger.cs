using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanger : MonoBehaviour
{
    [SerializeField] private float _damage;

    [SerializeField] private BuildingsAreaScaner _scaner;

    private void Start()
    {
        TaskCycle cycle = GetComponent<TaskCycle>();

        cycle.ShouldWorkDelegate = LOL; 
    }

    private bool LOL() => _scaner.Empty() == false;

    public void Damage()
    {  
        _scaner.GetRandomBulding().gameObject.GetComponent<BuildingHealth>().GetHurt(_damage);
    }
}
