using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;

namespace Combat.UpgradeCharges
{
    public sealed class UpgradeChargeContainer : MonoBehaviour
    {
        [Inject] private SelectionManager _selectionManager;
        [SerializeField] private int _chargesPerUpgrade;

        [Header("Animation")] 
        [SerializeField] private UpgradeChargeContainerAnimator _upgradeChargeContainerAnimator;
        private List<AnimateChargesRequest> _animationRequests = new();
        private int _currentCharge;

        public int CurrentCharge => _currentCharge;

        public event Action UpgradeSelectionEnqueued;
        
        public void AddUpgradeCharge()
        {
            _currentCharge++;
    
            if (_currentCharge >= _chargesPerUpgrade)
            {
                _selectionManager.EnqueueSelection(new SelectionSettings(SelectionType.BuildingUpgrade));
                _currentCharge = 0;
                
                UpgradeSelectionEnqueued?.Invoke();
            }
        }
        
        public async UniTask AddCharge(Transform source, int chargeAmount)
        {
            AnimateChargesRequest request = new AnimateChargesRequest(source, chargeAmount);
            
            _animationRequests.Add(request);

            await UniTask.WaitUntil(() => request.Complete);
        }
        
        public async UniTask AnimateAllRequests()
        {
            for (int i = 0; i < _animationRequests.Count; i++)
            {
                AnimateChargesRequest request = _animationRequests[i];
                
                for (int chargeIndex = 0; chargeIndex < request.Amount; chargeIndex++)
                {
                    await _upgradeChargeContainerAnimator.AddChargeWithAnimation(request.Source.position);
                }
                
                request.Complete = true;
            }
            
            _animationRequests.Clear();
        }
    }
}