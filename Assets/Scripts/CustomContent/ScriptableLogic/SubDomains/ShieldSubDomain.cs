using Combat.IngameUI.CombatEntityCanvas;
using ScriptableLogic.SubDomains;
using Cysharp.Threading.Tasks;
using Combat.ValueModifiers;
using Combat.Stats;
using UnityEngine;
using CuroAudio;
using Configs;
using System;
using Combat;

namespace ScriptableLogic.Commands.ValueModifiers
{
    public sealed class ResetShieldCommand : SubDomainCommand<ShieldSubDomain>, ICommand
    {
        public UniTask Execute()
        {
            SubDomain.Reinitialize();
            
            return UniTask.CompletedTask;
        }
    }
    
    public sealed class MaxHealthShieldSubDomain : ShieldSubDomain
    {
        [Argument("ShieldScaleFromMaxHealth")] private float _shieldScaleFromMaxHealth;

        public override float ShieldMaxHealth => Args.Owner.StatContainer.Get<MaxHealth>().Value * _shieldScaleFromMaxHealth;
    }
    
    public sealed class FlatShieldSubDomain : ShieldSubDomain
    {
        [Argument("ShieldMaxHealth")] private float _shieldMaxHealth;

        public override float ShieldMaxHealth => _shieldMaxHealth;
    }
    
    public abstract class ShieldSubDomain : SubDomain
    {
        [Argument("Icon")] private Sprite _iconSprite;
        [Argument("CanvasBarPrefab")] private GameObject _canvasBarPrefab;
        
        private ShieldValueModificator _shieldValueModificator;
        private EntityCanvasIcon _shieldIcon;
        private EntityCanvasBar _shieldBar;

        public abstract float ShieldMaxHealth { get; }
        
        public override UniTask Enable()
        {
            _shieldValueModificator = new(ShieldMaxHealth);
            _shieldValueModificator.ShieldHealthUpdated += OnShieldHealthUpdated;
            
            Args.Owner.ValueModifierContainer.DamageReceiverContainer.Add(_shieldValueModificator);
    
            Initialize();
            
            return UniTask.CompletedTask;
        }
        
        public override UniTask Disable()
        {
            TryRemoveIcon(0f);
            TryRemoveBar(0f);
            Args.Owner.ValueModifierContainer.DamageReceiverContainer.Remove(_shieldValueModificator);
            _shieldValueModificator.ShieldHealthUpdated -= OnShieldHealthUpdated;
            
            return UniTask.CompletedTask;
        }

        public void Reinitialize()
        {
            TryRemoveBar(0f);
            Initialize();
        }
        
        private void Initialize()
        {
            if (!_shieldIcon) _shieldIcon = Args.Owner.ComponentsContainer.Get<EntityCanvas>().AddIcon(_iconSprite);
            
            _shieldValueModificator.ResetShieldHealth();
        }
    
        #region UI
    
        private void OnShieldHealthUpdated(float shieldHealthPercent)
        {
            TryRemoveIcon(shieldHealthPercent);
            TryAddBar(shieldHealthPercent);
            TryRemoveBar(shieldHealthPercent);
            UpdateShieldBar(shieldHealthPercent);
        }
        
        private void TryRemoveIcon(float shieldHealthPercent)
        {
            if (!_shieldIcon || shieldHealthPercent == 1f) return;
            
            Args.Owner.ComponentsContainer.Get<EntityCanvas>().RemoveIcon(_shieldIcon);
        }
    
        private void TryRemoveBar(float shieldHealthPercent)
        {
            if (!_shieldBar || shieldHealthPercent != 0f) return;
      
            Args.Owner.ComponentsContainer.Get<EntityCanvas>().RemoveBar(_shieldBar);
        }
    
        private void TryAddBar(float shieldHealthPercent)
        {
            if (_shieldBar || shieldHealthPercent == 1f) return;
    
            _shieldBar = Args.Owner.ComponentsContainer.Get<EntityCanvas>().AddBar(_iconSprite, shieldHealthPercent, _canvasBarPrefab.GetComponent<EntityCanvasBar>());
        }
    
        private void UpdateShieldBar(float shieldHealthPercent)
        {
            if (!_shieldBar) return;
            
            _shieldBar.SetValue(shieldHealthPercent).Forget();
        }
    
        #endregion
        
        private sealed class ShieldValueModificator : ValueModifier
        {
            private float _currentShieldHealth;
            private float _maxShieldHealth;
            
            public event Action<float> ShieldHealthUpdated; 
            
            public ShieldValueModificator(float maxShieldHealth)
            {
                _currentShieldHealth = maxShieldHealth;
                _maxShieldHealth = maxShieldHealth;
            }
    
            public void ResetShieldHealth()
            {
                _currentShieldHealth = _maxShieldHealth;
                ShieldHealthUpdated?.Invoke(1f);
            }
            
            public override bool AllowValuePass(CombatEntity otherEntity, float value)
            {
                if (_currentShieldHealth > 0f)
                {
                    _currentShieldHealth -= value;
    
                    if (_currentShieldHealth <= 0f)
                    {
                        _currentShieldHealth = 0f;
                        AudioSystem.PlaySFX(AudioEnum.sound_combat_shield_break, OwnerEntity.transform.position);
                    }
                    
                    ShieldHealthUpdated?.Invoke(_currentShieldHealth / _maxShieldHealth);
    
                    DamageNumberDisplayManager.Instance.DisplayDamageNumber(value, OwnerEntity.transform.position, DamageVisualsType.Shielded);
                    
                    return false;
                }
                
                return true;
            }
        }
    }
}