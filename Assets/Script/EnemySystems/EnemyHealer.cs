using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealer : MonoBehaviour
{
    [SerializeField] private float _healSpeed;

    [SerializeField] private EnemyAreaScaner _scaner;

    private void Start()
    {
        TaskCycle cycle = GetComponent<TaskCycle>();

        cycle.ShouldWorkDelegate = LOL; 
    }

    private bool LOL() => _scaner.Empty() == false;

    public void Heal()
    {  
        _scaner.GetRandomEnemy().Heal(_healSpeed);
    }
}
