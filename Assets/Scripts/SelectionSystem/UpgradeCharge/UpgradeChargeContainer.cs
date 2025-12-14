using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

public sealed class UpgradeChargeContainer : MonoBehaviour
{
    [Inject] private SelectionManager _selectionManager;
    [SerializeField] private GameObject _chargePrefab;
    [SerializeField] private int _chargesPerUpgrade;
    [SerializeField] private Transform _finalPosition;
    [SerializeField] private float _animationDuration;
    [SerializeField] private AnimationCurve _verticalAnimationCurve;
    [SerializeField] private AnimationCurve _horizontalAnimationCurve;
    [SerializeField] private UpgradeChargeBar _chargeBar;
    [SerializeField] private float _timeBetweenCharges;
    private int _currentCharge;

    private bool _chargeAddProcessIsActive;
    
    public async UniTask AddChargesWithAnimation(int charges, Transform source)
    {
        await UniTask.WaitUntil(() => _chargeAddProcessIsActive == false);
        
        _chargeAddProcessIsActive = true;
        
        while (charges > 0)
        {
            AddChargeWithAnimation(source.position, _timeBetweenCharges).Forget();
            await UniTask.WaitForSeconds(_timeBetweenCharges);
            charges--;
        }
        
        _chargeAddProcessIsActive = false;
    }
    
    private async UniTask AddChargeWithAnimation(Vector3 sourcePosition, float barFillDuration)
    {
        GameObject charge = Instantiate(_chargePrefab, sourcePosition, Quaternion.identity);

        await MoveChargeObjectToFinalPosition(charge, _finalPosition.position);
        
        Destroy(charge);
        
        _currentCharge++;

        await _chargeBar.SetCharges(_currentCharge, barFillDuration * 0.75f);
        
        if (_currentCharge >= _chargesPerUpgrade)
        {
            _selectionManager.EnqueueSelection(new SelectionSettings(SelectionType.BuildingUpgrade));
            _selectionManager.TryStartQueuedSelection();
            _chargeBar.ResetBar();
            _currentCharge -= 5;
        }
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