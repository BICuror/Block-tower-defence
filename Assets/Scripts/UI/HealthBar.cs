using System.Collections;
using UnityEngine;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    #region ShakeSettings
    private float _shakeDuration = 0.3f;
    private float _shakeRotationStrength = 45f;
    private float _shakeScaleStrength = 0.04f;
    private Tween _shakeRotationTween;
    private Tween _shakeScaleTween;
    private Vector3 _defaultScale;
    #endregion

    #region CurrentHealthTweeningSettings
    private float _currentHealthChangeDuration = 0.1f;
    private Tween _currentHealthTween;
    #endregion

    #region HealthDifferenceTweeningSettings
    private float _healthDifferenceDecreaseTime = 0.2f;
    private float _timeBeforeDecreasingHealthDifference = 0.3f;
    private YieldInstruction _healthDifferenceChangeWait;
    private Tween _currentHealthDifferenceTween;
    #endregion

    private MeshRenderer _meshRenderer;

    private float _lastSetHealthDifferecne;
    private float _lastSetHealth;
    private float _currentDisplayedHealth;

    private MaterialPropertyBlock _materialPropertyBlock;

    private void Awake()
    {
        _defaultScale = transform.localScale;
        _meshRenderer = GetComponent<MeshRenderer>();

        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);

        _healthDifferenceChangeWait = new WaitForSeconds(_timeBeforeDecreasingHealthDifference);
    }

    public void SetValue(float health) 
    {
        _lastSetHealth = health;
        _currentDisplayedHealth = health;
        _lastSetHealthDifferecne = health;
        SetPropertyBlock(health, health);
    }

    private void SetPropertyBlock(float currentHealth, float healthDifference)
    {
        _materialPropertyBlock.SetFloat("Health", currentHealth);
        _materialPropertyBlock.SetFloat("HealthDifference", healthDifference);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
    
    #region Decrease
    public void DecreaseValue(float currentHealth, float healthDifference)
    {
        if (_lastSetHealthDifferecne < healthDifference) _lastSetHealthDifferecne = healthDifference;
        else _lastSetHealthDifferecne *= 0.95f;

        _lastSetHealth = currentHealth;

        ShakeSlider();

        DecreaseCurrentHealth();
        
        StopAllCoroutines();
        
        StartCoroutine(DecreaseHealthDifference());
    }

    private void DecreaseCurrentHealth()
    {
        if (_currentHealthTween != null && _currentHealthTween.IsPlaying()) _currentHealthTween.Kill();

        _currentHealthTween = DOVirtual.Float(_currentDisplayedHealth, _lastSetHealth, _currentHealthChangeDuration, SetCurrentHealth);

        void SetCurrentHealth(float currentHealth)
        {
            _currentDisplayedHealth = currentHealth;
            SetPropertyBlock(_currentDisplayedHealth, _lastSetHealthDifferecne);
        }
    }

    private IEnumerator DecreaseHealthDifference()
    {
        if (_currentHealthDifferenceTween != null && _currentHealthDifferenceTween.IsPlaying()) _currentHealthDifferenceTween.Kill();

        yield return _healthDifferenceChangeWait;

        _currentHealthDifferenceTween = DOVirtual.Float(_lastSetHealthDifferecne, _lastSetHealth, _healthDifferenceDecreaseTime, SetCurrentHealthDifference);
    }
    private void SetCurrentHealthDifference(float currentHealth)
    { 
        _lastSetHealthDifferecne = currentHealth;
        SetPropertyBlock(_currentDisplayedHealth, _lastSetHealthDifferecne);
    }
    
    #endregion

    public void IncreaseValue(float currentHealth)
    {
        if (_currentHealthTween != null && _currentHealthTween.IsPlaying()) _currentHealthTween.Kill();
        if (_currentHealthDifferenceTween != null && _currentHealthDifferenceTween.IsPlaying()) _currentHealthDifferenceTween.Kill();
        StopAllCoroutines();

        float healthDifference = currentHealth;
        if (currentHealth < _lastSetHealthDifferecne) healthDifference = _lastSetHealthDifferecne;
            
        SetPropertyBlock(currentHealth, currentHealth);
    }

    private void ShakeSlider()
    {
        CompleteAllTweens();

        _shakeRotationTween = transform.DOShakeRotation(_shakeDuration, _shakeRotationStrength);
        _shakeScaleTween = transform.DOShakeScale(_shakeDuration, _shakeScaleStrength);
    }

    private void CompleteAllTweens()
    {
        if (_shakeRotationTween != null && _shakeRotationTween.IsPlaying() == true)
        {
            _shakeRotationTween.Complete();
            _shakeScaleTween.Complete();
        }
    }

    private void OnDisable() 
    {
        CompleteAllTweens();
        StopAllCoroutines();
    }    

    private void OnDestroy() => transform.DOKill();
}
