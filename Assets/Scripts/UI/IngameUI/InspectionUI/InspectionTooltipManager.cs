using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Combat;
using System;

public sealed class InspectionTooltipManager : MonoBehaviour
{
    private static InspectionTooltipManager _instance;
    public static InspectionTooltipManager Instance => _instance;
    
    [Header("ActivityCondition")]
    [SerializeField] [Range(-1f, 1f)] private float _directionDotProductThreshold = -0.2f;
    [SerializeField] private float _wrongDirectionMaxDistance = 100;
    [SerializeField] private float _maxAlwaysValidPopupDistance;
    
    [Header("Links")]
    [SerializeField] private Transform _uiRoot;
    [SerializeField] private DragController _dragController;
    [SerializeField] private EntityTooltip _entityTooltipPrefab;
    [SerializeField] private CrystalInspectionTooltip _crystalInspectionTooltipPrefab;
    [SerializeField] private EffectInspectionTooltip _effectInspectionTooltipPrefab;
    [SerializeField] private EffectInspectionTooltipPreview _effectPreviewTooltipPrefab;
    
    private CancellationTokenSource _activeSinglePopupCancelationTokenSource = new();
    private readonly ListDictionary<UILayer, InspectionPanelBase> _layers = new(); 
    private bool _hoveredOverNonIdleTooltip;
    private UILayer _currentActiveLayer;
    
    public bool NonIdleTooltipsOpened => _layers.Contains(UILayer.Single);
    public bool HoveredOverNonIdleTooltip => _hoveredOverNonIdleTooltip;
    
    private void Awake()
    {
        _instance = this;
        
        _dragController.PickedObject.AddListener(_ =>
        {
            DisableActiveSinglePopup();
            SetActiveLayer(UILayer.None).Forget();
        });
        _dragController.DroppedObject.AddListener(_ => SetActiveLayer(UILayer.Group).Forget());
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

                List<InspectionPanelBase> panels = new List<InspectionPanelBase>(_layers.Get(UILayer.Single));
                
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
        EntityTooltip entityTooltip = Instantiate(_entityTooltipPrefab, _uiRoot);
           
        entityTooltip.Initialize(entity);
        
        _layers.Add(UILayer.Single, entityTooltip);
        
        SetActiveLayer(UILayer.Single).Forget();
        
        await KeepElementActiveWhileNeeded(entityTooltip);
    }

    public async UniTask OpenCrystalTooltip(Item item)
    {
        CrystalInspectionTooltip crystalInspectionTooltip = Instantiate(_crystalInspectionTooltipPrefab, _uiRoot);
           
        await crystalInspectionTooltip.Initialize(item);
        
        _layers.Add(UILayer.Single, crystalInspectionTooltip);
        
        SetActiveLayer(UILayer.Single).Forget();
        
        await KeepElementActiveWhileNeeded(crystalInspectionTooltip);
    }
    
    public async UniTask OpenEffectTooltip(EntityModificatorData entityModificatorData, Transform target)
    {
        EffectInspectionTooltip effectInspectionTooltip = Instantiate(_effectInspectionTooltipPrefab, _uiRoot);
        effectInspectionTooltip.Initialize(entityModificatorData, target);
        
        _layers.Add(UILayer.Single, effectInspectionTooltip);
        
        SetActiveLayer(UILayer.Single).Forget();
        
        await KeepElementActiveWhileNeeded(effectInspectionTooltip);
    }

    public EffectInspectionTooltipPreview OpenEffectPreviewTooltip(EntityModificatorData entityModificatorData, Transform target)
    {
        EffectInspectionTooltipPreview effectPreviewTooltip = Instantiate(_effectPreviewTooltipPrefab, _uiRoot);
        effectPreviewTooltip.Initialize(entityModificatorData, target);

        _layers.Add(UILayer.Group, effectPreviewTooltip);
        UpdateTooltipStates(UILayer.Group).Forget();
        
        return effectPreviewTooltip;
    }

    public async UniTask DestroyElement(InspectionPanelBase panel)
    {
        if (_layers.Contains(UILayer.Single)) _layers.Remove(UILayer.Single, panel);
        if (_layers.Contains(UILayer.Group)) _layers.Remove(UILayer.Group, panel);
        await panel.Disable();
        Destroy(panel.gameObject);
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
            await SetElementState(element, _currentActiveLayer == layer);
        });
    }

    private async UniTask SetElementState(InspectionPanelBase panel, bool state)
    {
        if (panel.IsActive == state) return;

        if (state) await panel.Enable();
        else await panel.Disable();
    }
    
    private async UniTask KeepElementActiveWhileNeeded(InspectionPanelBase panel)
    {
        Vector2 initialPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y); 
        Vector2 currentPosition = initialPosition;
        float distance = 0f;
        
        _hoveredOverNonIdleTooltip = panel.IsHoveredOver;
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
            
            _hoveredOverNonIdleTooltip = panel.IsHoveredOver;

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