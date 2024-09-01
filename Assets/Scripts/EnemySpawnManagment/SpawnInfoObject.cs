using UnityEngine;
using TMPro;
using DG.Tweening;

public sealed class SpawnInfoObject : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    
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
        transform.DOScale(Vector3.zero, _animationDuration).OnComplete(DestroyYourself);
    }

    private void DestroyYourself() => Destroy(gameObject);

    public void SetEnemiyData(EnemyData enemyData)
    {
        _meshFilter.sharedMesh = enemyData.GetMesh();
    }

    public void SetAmount(int amount)
    {
        _amountText.text = amount.ToString();
        _amountText.gameObject.SetActive(true);
    }
}
