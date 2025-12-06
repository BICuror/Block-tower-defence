using UnityEngine;
using DG.Tweening;
using Combat;
using TMPro;

public sealed class SpawnInfoObject : MonoBehaviour
{
    [SerializeField] private EnemyBootstrap _enemyBootstrap;
    
    [SerializeField] private TextMeshPro _amountText;

    [Header("Scale")]
    [SerializeField] private float _finalScale;
    [SerializeField] private float _animationDuration;

    public void Appear()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(new Vector3(_finalScale, _finalScale, _finalScale), _animationDuration);
    }

    public void Disappear()
    { 
        transform.DOScale(Vector3.zero, _animationDuration).OnComplete(() => Destroy(gameObject));
    }

    public void SetEnemiyData(EnemyData enemyData)
    {
        _enemyBootstrap.SetEnemyData(enemyData, false, false);
    }

    public void SetAmount(int amount)
    {
        _amountText.text = amount.ToString();
        _amountText.gameObject.SetActive(true);
    }
}