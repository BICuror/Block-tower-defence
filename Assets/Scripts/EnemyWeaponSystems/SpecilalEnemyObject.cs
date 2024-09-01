using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SpecialEnemyObject : MonoBehaviour
{
    [SerializeField] private float _areaScale;
    [SerializeField] private float _animationDuration;

    [SerializeField] private AnimationCurve _appearCurve;

    private EnemyHealth _enemyHealth;
    public EnemyHealth GetEnemyHealth() => _enemyHealth;

    private void Start()
    {   
        StartCoroutine(Appear());
    }

    private IEnumerator Appear()
    {
        YieldInstruction yieldInstruction = new WaitForFixedUpdate();
        
        float elapsedTime = 0f;

        while(elapsedTime < _animationDuration)
        {
            elapsedTime += Time.deltaTime;

            float scale = _appearCurve.Evaluate(elapsedTime / _animationDuration) * _areaScale;

            transform.localScale = Vector3.one * scale;

            yield return yieldInstruction;
        }
    }

    public void SetEnemyHealth(EnemyHealth enemyHealth)
    {
        _enemyHealth = enemyHealth;
        _enemyHealth.DeathEvent.AddListener(StartDisappearing);
    }

    public void StartDisappearing(GameObject enemy)
    {
        if (gameObject.activeSelf == false) return;
        transform.SetParent(null);

        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        YieldInstruction yieldInstruction = new WaitForFixedUpdate();
        
        float elapsedTime = 0f;

        while(elapsedTime < _animationDuration)
        {
            elapsedTime += Time.deltaTime;

            float scale = _appearCurve.Evaluate(1f - (elapsedTime / _animationDuration)) * _areaScale;

            transform.localScale = Vector3.one * scale;

            yield return yieldInstruction;
        }

        Destroy(gameObject);
    }
}
