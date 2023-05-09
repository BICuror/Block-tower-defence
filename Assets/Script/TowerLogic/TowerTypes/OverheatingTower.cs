using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheatingTower : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    
    [SerializeField] private float arrowSpeed;

    [SerializeField] private int damage;

    [SerializeField] private ApplyEffectContainer _applyEffectContainer;
    
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;

    [SerializeField] private OverheatManager _overheatingManager;

    private void Start()
    {   
        GetComponent<TaskCycle>().ShouldWorkDelegate = LOL;

        _applyEffectContainer.ApplyEffectUpdated.AddListener(ModifyArrow);
    }

    private void ModifyArrow()
    {
        arrow.GetComponent<Weapon>()._effect = _applyEffectContainer.GetApplyEffects();
    }

    public bool LOL() => _enemyAreaScaner.Empty() == false &&  _overheatingManager.IsOverheated() == false;

    public void Shoot()
    {
        _overheatingManager.AddOverheat(20f);

        arrow.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        arrow.SetActive(true);

        arrow.transform.position = transform.position;

        arrow.transform.LookAt(_enemyAreaScaner.GetFirstEnemy().transform.position);

        arrow.GetComponent<Rigidbody>().AddForce(arrow.transform.forward * arrowSpeed, ForceMode.Impulse);

        arrow.GetComponent<Weapon>().SetDamage(damage);
    }
}
