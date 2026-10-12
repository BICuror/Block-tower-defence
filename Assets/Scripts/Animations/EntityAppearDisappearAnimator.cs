using UnityEngine;
using DG.Tweening;
using Cashing;

namespace Combat.Animation
{
    public sealed class EntityAppearDisappearAnimator : MonoBehaviour
    {
        [Cached] private CombatEntity _ownerEntity;
        [SerializeField] private float _animationDuration;
        
        private void Awake()
        {
            Vector3 initialScale = transform.localScale;

            transform.DOScale(initialScale, _animationDuration).From(Vector3.zero).SetLink(gameObject);

            _ownerEntity.Health.Died += AnimateDeath;
        }

        private void AnimateDeath()
        {
            _ownerEntity.GetComponent<Collider>().enabled = false;
            
            _ownerEntity.Health.Died -= AnimateDeath;
            
            transform.DOKill();
            transform.DOScale(Vector3.zero, _animationDuration).SetLink(gameObject).OnComplete(() => Destroy(_ownerEntity.gameObject));
        }
    }
}