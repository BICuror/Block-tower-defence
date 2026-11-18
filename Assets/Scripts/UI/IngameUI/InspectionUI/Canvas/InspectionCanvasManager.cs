using System.Collections.Generic;
using Combat.Entity.Modificators;
using GameControls.Controllers;
using Cysharp.Threading.Tasks;
using System.Threading;
using Combat.Items;
using UnityEngine;
using Zenject;
using System;
using UnityEngine.Serialization;

namespace Combat.IngameUI.Inspection.InspectionCanvas
{
    public sealed class InspectionCanvasManager : MonoBehaviour
    {
        private static InspectionCanvasManager _instance;
        public static InspectionCanvasManager Instance => _instance;
        
        [Inject] private DragController _dragController;
        
        [Header("ActivityCondition")]
        [SerializeField] [Range(-1f, 1f)] private float _directionDotProductThreshold = -0.2f;
        [SerializeField] private float _wrongDirectionMaxDistance = 100;
        [SerializeField] private float _maxAlwaysValidPopupDistance;
        
        [Header("Links")]
        [SerializeField] private Transform _uiRoot; 
        [SerializeField] private EntityInspectionCanvas _entityInspectionCanvasPrefab;
        [SerializeField] private CrystalInspectionCanvas _crystalInspectionCanvasPrefab;
        [SerializeField] private EntityModificatorInspectionCanvas _entityModificatorInspectionCanvasPrefab; 
        [SerializeField] private EntityModificatorInspectionCanvasPreview _entityModificatorPreviewCanvasPrefab;
        [SerializeField] private RerollInspectionCanvas _rerollPrevieTooltipPrefab;
        
        private CancellationTokenSource _activeSinglePopupCancelationTokenSource = new();
        private readonly ListDictionary<UILayer, InspectionCanvasBase> _layers = new(); 
        private bool _hoveredOverNonIdleTooltip;
        private UILayer _currentActiveLayer;
        
        public bool NonIdleTooltipsOpened => _layers.Contains(UILayer.Single);
        public bool HoveredOverNonIdleTooltip => _hoveredOverNonIdleTooltip;
    
        public event Action<InspectionCanvasBase> OpenedTooltip;
        
        private void Awake()
        {
            _instance = this;
            
            _dragController.PickedObject += _ => DisableActiveSinglePopup();
            _dragController.PickedObject += _ => SetActiveLayer(UILayer.None).Forget();
            _dragController.DroppedObject += _ => SetActiveLayer(UILayer.Group).Forget();
        }
        
        private async UniTask SetActiveLayer(UILayer layer)
        {
            _currentActiveLayer = layer;
    
            if (layer == UILayer.Single)
            {
                await UpdateTooltipStates(UILayer.Group);
                await UpdateTooltipStates(UILayer.Single);
            }
            else
            {
                if (_layers.Contains(UILayer.Single))
                {
                    List<UniTask> destroymentTasks = new();
    
                    List<InspectionCanvasBase> panels = new List<InspectionCanvasBase>(_layers.Get(UILayer.Single));
                    
                    panels.ForEach(panel =>
                    {
                        if (panel) destroymentTasks.Add(DestroyElement(panel));
                    });
       
                    await UniTask.WhenAll(destroymentTasks);
                }
                
                await UpdateTooltipStates(UILayer.Group);
            }
        }
        
        public async UniTask OpenEntityTooltip(CombatEntity entity)
        {
            EntityInspectionCanvas entityInspectionCanvas = Instantiate(_entityInspectionCanvasPrefab, _uiRoot);
               
            entityInspectionCanvas.Initialize(entity);
    
            await OpenTooltip(entityInspectionCanvas);
        }
    
        public async UniTask OpenCrystalTooltip(Item item)
        {
            CrystalInspectionCanvas crystalInspectionCanvas = Instantiate(_crystalInspectionCanvasPrefab, _uiRoot);
               
            crystalInspectionCanvas.Initialize(item);
    
            await OpenTooltip(crystalInspectionCanvas);
        }
        
        public async UniTask OpenEffectTooltip(EntityModificatorConfig entityModificatorConfig, BuildingEntity targetEntity, Transform target)
        {
            EntityModificatorInspectionCanvas entityModificatorInspectionCanvas = Instantiate(_entityModificatorInspectionCanvasPrefab, _uiRoot);
            
            entityModificatorInspectionCanvas.Initialize(entityModificatorConfig, targetEntity, target);
    
            await OpenTooltip(entityModificatorInspectionCanvas);
        }
    
        private async UniTask OpenTooltip(InspectionCanvasBase canvasBase)
        {
            _layers.Add(UILayer.Single, canvasBase);
            
            SetActiveLayer(UILayer.Single).Forget();
            
            OpenedTooltip?.Invoke(canvasBase);
            
            await KeepElementActiveWhileNeeded(canvasBase);
        }
    
        public EntityModificatorInspectionCanvasPreview OpenEffectPreviewTooltip(EntityModificatorConfig entityModificatorConfig, Transform target)
        {
            EntityModificatorInspectionCanvasPreview entityModificatorPreviewCanvas = Instantiate(_entityModificatorPreviewCanvasPrefab, _uiRoot);
            entityModificatorPreviewCanvas.Initialize(entityModificatorConfig, target);
    
            _layers.Add(UILayer.Group, entityModificatorPreviewCanvas);
            UpdateTooltipStates(UILayer.Group).Forget();
            
            return entityModificatorPreviewCanvas;
        }
        
        public InspectionCanvasBase OpenRerollPreview(Transform target)
        {
            RerollInspectionCanvas rerollPrevieTooltip = Instantiate(_rerollPrevieTooltipPrefab, _uiRoot);
            rerollPrevieTooltip.Initialize(target);
    
            _layers.Add(UILayer.Group, rerollPrevieTooltip);
            UpdateTooltipStates(UILayer.Group).Forget();
            
            return rerollPrevieTooltip;
        }
        
        public async UniTask DestroyElement(InspectionCanvasBase canvas)
        {
            if (_layers.Contains(UILayer.Single)) _layers.Remove(UILayer.Single, canvas);
            if (_layers.Contains(UILayer.Group)) _layers.Remove(UILayer.Group, canvas);
            await canvas.Disable();
            Destroy(canvas.gameObject);
        }
        
        public void DisableActiveSinglePopup()
        {
            _activeSinglePopupCancelationTokenSource.Cancel();
            _activeSinglePopupCancelationTokenSource = new();
            _hoveredOverNonIdleTooltip = false;
        }
        
        private async UniTask UpdateTooltipStates(UILayer layer)
        {
            if (!_layers.Contains(layer)) return;
    
            _layers.Get(layer).ForEach(async (element) => 
            {
                await SetElementState(element, _currentActiveLayer == layer || layer == UILayer.Group);
            });
        }
    
        private async UniTask SetElementState(InspectionCanvasBase canvas, bool state)
        {
            if (canvas.IsActive == state) return;
    
            if (state) await canvas.Enable();
            else await canvas.Disable();
        }
        
        private async UniTask KeepElementActiveWhileNeeded(InspectionCanvasBase canvas)
        {
            Vector2 initialPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y); 
            Vector2 currentPosition = initialPosition;
            float distance = 0f;
            
            _hoveredOverNonIdleTooltip = canvas.IsHoveredOver;
            bool hasHoveredOverNonIdleTooltip = false;
            
            DisableActiveSinglePopup();
            
            do
            {
                try
                {
                    await UniTask.NextFrame(cancellationToken: _activeSinglePopupCancelationTokenSource.Token, cancelImmediately: true);
                }
                catch (Exception e)
                {
                    e.LogAsync();
                    break;
                }
             
                currentPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    
                distance = Vector2.Distance(initialPosition, currentPosition);
                
                _hoveredOverNonIdleTooltip = canvas.IsHoveredOver;
    
                if (ShouldDisableOnExitingPopup()) break;
            } 
            while (_hoveredOverNonIdleTooltip || IsValidPopup());
    
            SetActiveLayer(UILayer.Group).Forget();
            
            return;
            
            bool IsValidPopup() => Vector2.Dot(currentPosition - initialPosition, Vector2.up) > _directionDotProductThreshold || distance < _wrongDirectionMaxDistance;
    
            bool ShouldDisableOnExitingPopup()
            {
                if (_hoveredOverNonIdleTooltip) hasHoveredOverNonIdleTooltip = true;
    
                return hasHoveredOverNonIdleTooltip && !_hoveredOverNonIdleTooltip && distance > _maxAlwaysValidPopupDistance;
            }
        }
    }
    
    public enum UILayer
    {
        None,
        Group,
        Single
    }
}