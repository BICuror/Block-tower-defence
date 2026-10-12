using Combat.Enemies;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Combat.EnemySpawnSystem
{
    public sealed class SpawnInfoObject : MonoBehaviour
    {
        [SerializeField] private EnemyBootstrap _enemyBootstrap;

        [SerializeField] private TextMeshPro _amountText;

        [Header("Scale")] [SerializeField] private float _finalScale;
        [SerializeField] private float _animationDuration;

        public void Appear()
        {
            gameObject.SetActive(true);
            transform.DOScale(new Vector3(_finalScale, _finalScale, _finalScale), _animationDuration);
        }

        public void Disappear()
        {
            transform.DOScale(Vector3.zero, _animationDuration).OnComplete(() => gameObject.SetActive(false));
        }

        public void SetEnemiyData(EnemyConfig enemyConfig)
        {
            _enemyBootstrap.SetEnemyConfig(enemyConfig, false, false);
        }

        public void SetAmount(int amount)
        {
            _amountText.text = amount.ToString();
            _amountText.gameObject.SetActive(true);
        }
    }
}