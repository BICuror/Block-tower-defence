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
    
    private async UniTask AddChargeWithAnimation(Vector3 sourcePosition, float barFillDuration)
    {
        GameObject charge = Instantiate(_chargePrefab, sourcePosition, Quaternion.identity);

        charge.transform.DOMoveX(_finalPosition.position.x, _animationDuration, false).SetEase(_horizontalAnimationCurve);
        charge.transform.DOMoveZ(_finalPosition.position.z, _animationDuration, false).SetEase(_horizontalAnimationCurve);
        await charge.transform.DOMoveY(_finalPosition.position.y, _animationDuration, false).SetEase(_verticalAnimationCurve).AsyncWaitForCompletion();
        
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
}