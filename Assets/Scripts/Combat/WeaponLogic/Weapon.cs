using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

namespace Combat
{
    public abstract class Weapon : WeaponBase
    {
        [SerializeField] protected Collider Collider; 
        [SerializeField] protected Rigidbody Rigidbody;
        
        private CancellationTokenSource _cancellationTokenSource = new();
        private float _lifetime;
        
        public void Initialize(CombatEntity ownerEntity, float lifetime)
        {
            OwnerEntity = ownerEntity;
            _lifetime = lifetime;
            OnInitialized();
        }
        
        #region StateManagements

        protected void OnEnable()
        {
            StopLifetimeTrack();
            SetState(true);
            
            if (_lifetime > 0) StartLifetimeTrack().Forget();
        }

        protected void OnDisable() => StopLifetimeTrack();
        
        private async UniTask StartLifetimeTrack()
        {      
            _cancellationTokenSource = new();

            try
            {
                await UniTask.WaitForSeconds(_lifetime, cancellationToken: _cancellationTokenSource.Token, cancelImmediately: true);
            }
            catch (Exception e)
            {
                e.LogAsync(); 
                return;
            }
            
            SetState(false);
        }

        private void StopLifetimeTrack()
        {
            if (_cancellationTokenSource == null) return;
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        
        protected void SetState(bool state)
        {
            Collider.enabled = state;
            gameObject.SetActive(state);
        }
        #endregion
    }
}