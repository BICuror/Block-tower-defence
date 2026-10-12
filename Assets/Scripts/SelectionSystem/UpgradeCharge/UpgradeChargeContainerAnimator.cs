using Cysharp.Threading.Tasks;
using Combat.IngameUI;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Combat.UpgradeCharges
{
    public sealed class UpgradeChargeContainerAnimator : MonoBehaviour
    {
        [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
        
        [Header("ChargeAnimation")]
        [SerializeField] private GameObject _chargePrefab;
        [SerializeField] private Transform _finalPosition;
        [SerializeField] private float _animationDuration;
        [SerializeField] private AnimationCurve _verticalAnimationCurve;
        [SerializeField] private AnimationCurve _horizontalAnimationCurve;

        [Header("BarAnimation")]
        [SerializeField] private UpgradeChargeBar _chargeBar;
        [SerializeField] private float _timeBetweenCharges;

        private void Awake()
        {
            _upgradeChargeContainer.UpgradeSelectionEnqueued += ResetBar;
        }

        private void ResetBar() => _chargeBar.ResetBar();
        
        public async UniTask AddChargeWithAnimation(Vector3 source)
        {
            GameObject charge = Instantiate(_chargePrefab, source, Quaternion.identity);
            
            await MoveChargeObjectToFinalPosition(charge, _finalPosition.position);
            
            Destroy(charge);
            
            await _chargeBar.SetCharges(_upgradeChargeContainer.CurrentCharge + 1, _timeBetweenCharges);

            _upgradeChargeContainer.AddUpgradeCharge();
        }
    
        private async UniTask MoveChargeObjectToFinalPosition(GameObject chargeObject, Vector3 finalPosition)
        {
            Vector3 startPosition = chargeObject.transform.position;
            
            await DOVirtual.Float(0f, 1f, _animationDuration, MoveCharge).AsyncWaitForCompletion();
            
            void MoveCharge(float progress)
            {
                Vector3 lerpedPosition = Vector3.LerpUnclamped(startPosition, finalPosition, _horizontalAnimationCurve.Evaluate(progress));
    
                lerpedPosition.y += _verticalAnimationCurve.Evaluate(progress);
                
                chargeObject.transform.position = lerpedPosition;
            }
        }
    }
    
    public sealed class AnimateChargesRequest
    {
        public Transform Source;
        public int Amount;

        public bool Complete;

        public AnimateChargesRequest(Transform source, int amount)
        {
            Source = source;
            Amount = amount;
        }
    }
}