using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernoTower : MonoBehaviour
{
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;

    [SerializeField] private TaskCycle _taskCycle;

    [SerializeField] private float _beginningSpeed;

    [SerializeField] private float _accseleration;

    [SerializeField] private ApplyEffectContainer _applyEffectContainer;

    [SerializeField] private LineRenderer _lineRenderer;

    private float _currentSpeed;

    private EnemyHealth _currentEnemy;

    private void Start()
    {
        _taskCycle.ShouldWorkDelegate = ShouldWork;

        _enemyAreaScaner.EnemyEnteredArea.AddListener(TrySetEnemy);

        _enemyAreaScaner.EnemyExitedArea.AddListener(TryRemoveEnemy);
    }  

    private void TrySetEnemy(EnemyHealth enemy)
    {
        if (_currentEnemy == null)
        {
            _currentEnemy = enemy;

            _currentSpeed = _beginningSpeed;
        }
    }

    private void TryRemoveEnemy(EnemyHealth enemy)
    {
        if (_currentEnemy == enemy)
        {
            _currentEnemy = null;

            _lineRenderer.SetPositions(new Vector3[1]{transform.position});

            if (_enemyAreaScaner.Empty() == false) TrySetEnemy(_enemyAreaScaner.GetFirstEnemy());
        }
    }

    private bool ShouldWork() => _enemyAreaScaner.Empty() == false;

    public void Beam()
    {   
        _currentSpeed *= _accseleration;

        _currentEnemy.GetHurt(_currentSpeed);

        for (int i = 0; i < _applyEffectContainer.GetApplyEffects().Count; i++)
        {
            _currentEnemy.gameObject.GetComponent<EntityEffectManager>().ApplyEffect(_applyEffectContainer.GetApplyEffects()[i]);
        }

        _lineRenderer.SetPositions(new Vector3[2]{transform.position, _currentEnemy.transform.position});
    } 
}
