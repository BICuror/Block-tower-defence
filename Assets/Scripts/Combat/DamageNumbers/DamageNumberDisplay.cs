using TMPEffects.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using System;
using TMPro;

using Random = UnityEngine.Random;

public sealed class DamageNumberDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text _damageNumberText;
    
    [SerializeField] private SerializedDictionary<DamageVisualsType, DamageTypeVisualsContainer> _damageTypeVisualContainers;
    
    [Header("Animation")]
    [SerializeField] private AnimationCurve _alphaAnimationCurve;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private float _animationDuration;
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private Vector3 _endPoint;
    [SerializeField] private float _yVariety = 0.5f;
    [SerializeField] private float _xVariety = 1;
    
    public async UniTask DisplayDamageNumber(float damage, Vector3 position, DamageVisualsType damageVisualsType)
    {
        int roundedDamageValue = Mathf.RoundToInt(damage);

        if (roundedDamageValue == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        SetVisualData(roundedDamageValue, damageVisualsType);
        
        transform.position = position;
        gameObject.SetActive(true);

        _damageNumberText.DOFade(0f, _animationDuration).From(1f).SetEase(_alphaAnimationCurve);

        Vector3 endValue = _endPoint + new Vector3(Random.Range(-_xVariety, _xVariety), 0f, Random.Range(0f, _yVariety));
        
        await _damageNumberText.transform.DOLocalMove(endValue, _animationDuration).SetEase(_animationCurve).From(_startPoint).AsyncWaitForCompletion();
        
        gameObject.SetActive(false);
    }

    private void SetVisualData(int damageValue, DamageVisualsType damageVisualsType)
    {
        _damageNumberText.color = _damageTypeVisualContainers[damageVisualsType].DamageColor;
        _damageNumberText.text = damageValue.ToString();
    }
    
    [Serializable] private sealed class DamageTypeVisualsContainer
    {
        public Color DamageColor;
    }
}

public enum DamageVisualsType
{
    Damage,
    Shielded,
    Fire,
    Poison,
    NonLethal,
}