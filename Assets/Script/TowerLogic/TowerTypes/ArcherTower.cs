using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    
    [SerializeField] private float arrowSpeed;

    [SerializeField] private int damage;

    [SerializeField] private ApplyEffectContainer _applyEffectContainer;
    
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;

    private void Start()
    {   
        GetComponent<TaskCycle>().ShouldWorkDelegate = LOL;

        _applyEffectContainer.ApplyEffectUpdated.AddListener(ModifyArrow);
    }

    private void ModifyArrow()
    {
        arrow.GetComponent<Weapon>()._effect = _applyEffectContainer.GetApplyEffects();
    }

    public bool LOL() => _enemyAreaScaner.Empty() == false;

    public void Shoot()
    {   
        arrow.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        arrow.SetActive(true);

        arrow.transform.position = transform.position;

        arrow.transform.LookAt(_enemyAreaScaner.GetFirstEnemy().transform.position);

        arrow.GetComponent<Rigidbody>().AddForce(arrow.transform.forward * arrowSpeed, ForceMode.Impulse);

        arrow.GetComponent<Weapon>().SetDamage(damage);
    }
}
