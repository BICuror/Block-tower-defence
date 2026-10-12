using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System;

namespace Combat.IngameUI
{
    public sealed class EntityCanvasAbilityIcon : ProgressBarBase
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [Header("Scale")]
        [SerializeField] private float _defaultIconScale = 1f;
        [SerializeField] private float _depletedIconScale;
    
        private float _fillDuration = 1f;
        private bool _hasActiveWaveState;
        private float _previousValue;
        
        public bool IsActive => gameObject.activeSelf;
        public float LastSetValue => _previousValue;
        protected override string ProgressFieldName => "BuildProgress";
    
        public event Action StateUpdated;
        
        public void Initialize(Sprite sprite, float value = 0)
        {
            Initialize();
            _spriteRenderer.sprite = sprite;
            _previousValue = value;
    
            SetValue(value);
        }
        
        public void SetFillDuration(float fillDuration) => _fillDuration = fillDuration;
    
        public void SetValue(float value)
        {
            FillBar(_previousValue, value, _fillDuration).Forget();
            _previousValue = value;
        }
    
        public void SetActiveState(bool state)
        {
            if (gameObject.activeSelf == state) return;
            
            transform.DOKill();
            
            if (state)
            {
                gameObject.SetActive(true);
                transform.DOScale(_defaultIconScale, 0.5f).SetLink(gameObject);
            }
            else
            {
                transform.DOScale(_depletedIconScale, 0.5f).SetLink(gameObject).OnComplete(() => gameObject.SetActive(false));
            }
            
            StateUpdated?.Invoke();
        }
    }
}