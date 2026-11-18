using ScriptableLogic.ArgumentItem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Configs;
using System;
using Combat;

using Object = UnityEngine.Object;

namespace ScriptableLogic.SubDomains
{
    public sealed class CustomAreaSubDomain : SubDomain
    {
        [Argument("UseReachAreaScale")] private bool _useReachAreaScale;
        [ArgumentItem("AreaScale")] private ValueArgumentItem _areaScale;
        [Argument("AreaPrefab")] private GameObject _areaPrefab;
        
        private AreaEntityDetector _areaEntityDetector;
        private AreaScanerController _areaScanerController;
        
        public AreaEntityDetector AreaEntityDetector => _areaEntityDetector;
        
        public event Action<CombatEntity> EntityAdded;
        public event Action<CombatEntity> EntityRemoved;
        
        public override UniTask Enable()
        {
            _areaEntityDetector = Object.Instantiate(_areaPrefab, Args.Owner.transform).GetComponent<AreaEntityDetector>();
            _areaScanerController = _areaEntityDetector.GetComponent<AreaScanerController>();
            _areaScanerController.SetAutoScale(_useReachAreaScale);

            if (!_useReachAreaScale)
            {
                if (_areaScale.UseStatValue)
                {
                    _areaScale.GetStat().RoundedValueChanged += SetAreaScale;
                }
                
                SetAreaScale(_areaScale.GetRoundedValue());
            }
            
            _areaEntityDetector.AddedItem += OnEntityAdded;
            _areaEntityDetector.RemovedItem += OnEntityRemoved;
            
            return UniTask.CompletedTask;
        }
        
        private void SetAreaScale(int visualisationScale)
        {
            _areaScanerController.SetScale(visualisationScale);
        }
        
        private void OnEntityAdded(CombatEntity entity) => EntityAdded?.Invoke(entity);
        private void OnEntityRemoved(CombatEntity entity) => EntityRemoved?.Invoke(entity);

        public override UniTask Disable()
        {
            if (!_useReachAreaScale && _areaScale.UseStatValue)
            {
                _areaScale.GetStat().RoundedValueChanged -= SetAreaScale;
            }
            
            _areaEntityDetector.AddedItem -= OnEntityAdded;
            _areaEntityDetector.RemovedItem -= OnEntityRemoved;
            
            Object.Destroy(_areaEntityDetector);
            
            return UniTask.CompletedTask;
        }
    }

    public sealed class CustomAreaVisualisationSubDomain : SubDomain
    {
        [ArgumentItem("AreaScale")] private ValueArgumentItem _areaScale = new();
        [Argument("AreaVisualisationPrefab")] private GameObject _areaVisualisationPrefab;
        
        private AreaVisualisation _areaVisualisation;
        
        public override UniTask Enable()
        {
            _areaVisualisation = Object.Instantiate(_areaVisualisationPrefab, Args.Owner.transform).GetComponent<AreaVisualisation>();

            if (_areaScale.UseStatValue)
            {
                _areaScale.GetStat().RoundedValueChanged += SetVisualisationScale;
            }

            SetVisualisationScale(_areaScale.GetRoundedValue());
            
            return UniTask.CompletedTask;
        }

        private void SetVisualisationScale(int visualisationScale)
        {
            _areaVisualisation.SetDefaultScale(visualisationScale);
        }

        public override UniTask Disable()
        {
            if (_areaScale.UseStatValue)
            {
                _areaScale.GetStat().RoundedValueChanged -= SetVisualisationScale;
            }
            
            Object.Destroy(_areaVisualisation.gameObject);
            
            return UniTask.CompletedTask;
        }
    }
}