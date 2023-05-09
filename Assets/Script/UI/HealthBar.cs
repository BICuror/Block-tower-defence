using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Material _healthBarMaterial;

    private Material _currentHealthBarMaterial;

    [SerializeField] private float _decreaseAcceleration = 2f;

    [SerializeField] private float _startDecreaseSpeed = 0.01f;

    private float _currentDecreaseSpeed;

    private void OnEnable() => CreateAndSetNewMaterial();
    
    private void CreateAndSetNewMaterial()
    {
        _currentHealthBarMaterial = new Material(_healthBarMaterial);

        gameObject.GetComponent<MeshRenderer>().material = _currentHealthBarMaterial;
    }

    public void UpdateValue(float healthPercent)
    {
        if (gameObject.activeSelf == false) return;

        float health = _currentHealthBarMaterial.GetFloat("Health");

        _currentHealthBarMaterial.SetFloat("Health", healthPercent);

        StartDecreasingHealthDifference(health);
    }

    private void StartDecreasingHealthDifference(float health)
    {
        StopAllCoroutines();

        _currentDecreaseSpeed = _startDecreaseSpeed;

        StartCoroutine(SliderDecrease(health));
    }

    private IEnumerator SliderDecrease(float healthDifference)
    {
        yield return new WaitForFixedUpdate();

        _currentDecreaseSpeed *= _decreaseAcceleration;
        
        healthDifference -= _currentDecreaseSpeed;
        
        _currentHealthBarMaterial.SetFloat("HealthDifference", healthDifference);

        if (healthDifference > _currentHealthBarMaterial.GetFloat("Health")) 
        {
            StartCoroutine(SliderDecrease(healthDifference));
        }
        else
        {
            _currentHealthBarMaterial.SetFloat("HealthDifference", _currentHealthBarMaterial.GetFloat("Health"));
        }
    }
}
