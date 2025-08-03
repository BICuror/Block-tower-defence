using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Combat;
using System;
using System.Threading.Tasks;

public sealed class InspectionTooltipManager : MonoBehaviour
{
    private static InspectionTooltipManager _instance;
    public static InspectionTooltipManager Instance => _instance;
    
    [Header("ActivityCondition")]
    [SerializeField] [Range(-1f, 1f)] private float _directionDotProductThreshold = -0.2f;
    [SerializeField] private float _wrongDirectionMaxDistance = 100;
    
    [SerializeField] private Transform _uiRoot;
    [SerializeField] private DragController _dragController;
    [SerializeField] private EntityTooltip _entityTooltipPrefab;
    [SerializeField] private CrystalInspectionTooltip _crystalInspectionTooltipPrefab;
    [SerializeField] private EffectInspectionTooltip _effectInspectionTooltipPrefab;
    [SerializeField] private EffectInspectionTooltipPreview _effectPreviewTooltipPrefab;
    
    private CancellationTokenSource _activeSinglePopupCancelationTokenSource = new();
    private readonly ListDictionary<UILayer, PointFollowingCanvasUIElement> _layers = new(); 
    private UILayer _currentActiveLayer;
    
    public bool NonIdleTooltipsOpened => _layers.Contains(UILayer.Single);
    
    private void Awake()
    {
        _instance = this;
        
        _dragController.PickedObject.AddListener(_ =>
        {
            DisableActiveSinglePopup();
            SetActiveLayer(UILayer.None);
        });
        _dragController.DroppedObject.AddListener(_ => SetActiveLayer(UILayer.Group));
    }
    
    public async UniTask SetActiveLayer(UILayer layer)
    {
        Debug.Log("SetLayer " + layer);
        
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

                List<PointFollowingCanvasUIElement> elements = new List<PointFollowingCanvasUIElement>(_layers.Get(UILayer.Single));
                
                elements.ForEach(element =>
                {
                    if (element) destroymentTasks.Add(DestroyElement(element));
                });
   
                await UniTask.WhenAll(destroymentTasks);
            }
            
            await UpdateTooltipStates(UILayer.Group);
        }
    }
    
    public async UniTask OpenEntityTooltip(CombatEntity entity)
    {
        EntityTooltip entityTooltip = Instantiate(_entityTooltipPrefab, _uiRoot);
           
        await entityTooltip.Initialize(entity);
        
        _layers.Add(UILayer.Single, entityTooltip);
        
        SetActiveLayer(UILayer.Single);
        
        await KeepElementActiveWhileNeeded(entityTooltip);
    }

    public async UniTask OpenCrystalTooltip(Item item)
    {
        CrystalInspectionTooltip crystalInspectionTooltip = Instantiate(_crystalInspectionTooltipPrefab, _uiRoot);
           
        await crystalInspectionTooltip.Initialize(item);
        
        _layers.Add(UILayer.Single, crystalInspectionTooltip);
        
        SetActiveLayer(UILayer.Single);
        
        await KeepElementActiveWhileNeeded(crystalInspectionTooltip);
    }
    
    public async UniTask OpenEffectTooltip(SelectionOptionObject selectionOptionObject)
    {
        EffectInspectionTooltip effectInspectionTooltip = Instantiate(_effectInspectionTooltipPrefab, _uiRoot);
        
        await effectInspectionTooltip.SetSelectionOptionObject(selectionOptionObject);
        
        _layers.Add(UILayer.Single, effectInspectionTooltip);
        
        SetActiveLayer(UILayer.Single);
        
        await KeepElementActiveWhileNeeded(effectInspectionTooltip);
    }

    public async UniTask<EffectInspectionTooltipPreview> OpenEffectPreviewTooltip(SelectionOptionObject selectionOptionObject)
    {
        EffectInspectionTooltipPreview effectPreviewTooltip = Instantiate(_effectPreviewTooltipPrefab, _uiRoot);
        await effectPreviewTooltip.Initialilize(selectionOptionObject);

        _layers.Add(UILayer.Group, effectPreviewTooltip);
        UpdateTooltipStates(UILayer.Group);
        
        return effectPreviewTooltip;
    }

    public async UniTask DestroyElement(PointFollowingCanvasUIElement element)
    {
        if (_layers.Contains(UILayer.Single)) _layers.Remove(UILayer.Single, element);
        if (_layers.Contains(UILayer.Group)) _layers.Remove(UILayer.Group, element);
        await element.Disable();
        Destroy(element.gameObject);
    }
    
    public void DisableActiveSinglePopup()
    {
        _activeSinglePopupCancelationTokenSource.Cancel();
        _activeSinglePopupCancelationTokenSource = new();
    }
    
    private async UniTask UpdateTooltipStates(UILayer layer)
    {
        if (!_layers.Contains(layer)) return;

        _layers.Get(layer).ForEach(async (element) => 
        {
            await SetElementState(element, _currentActiveLayer == layer);
        });
    }

    private async UniTask SetElementState(PointFollowingCanvasUIElement element, bool state)
    {
        if (element.IsActive == state) return;

        if (state) await element.Enable();
        else await element.Disable();
    }
    
    private async UniTask KeepElementActiveWhileNeeded(PointFollowingCanvasUIElement element)
    {
        DisableActiveSinglePopup();
        element.PointerEntered += OnEnteringElement;
        element.PointerExited += OnExitingElement;
        bool hasBeenEntered = false;
        
        try
        {
            Vector2 initialPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 currentPosition;
            float distance;
            bool isValid;

            do
            {
                await UniTask.WaitForFixedUpdate(cancellationToken: _activeSinglePopupCancelationTokenSource.Token);

                currentPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                distance = Vector2.Distance(initialPosition, currentPosition);

                isValid = hasBeenEntered || (Vector2.Dot(currentPosition - initialPosition, Vector2.up) > _directionDotProductThreshold || distance < _wrongDirectionMaxDistance);
            } 
            while (isValid);
        }
        catch (Exception e)
        {
            e.LogAsync();
        }

        SetActiveLayer(UILayer.Group);
        
        return;
        
        void OnEnteringElement()
        {
            element.PointerExited -= OnEnteringElement;
            hasBeenEntered = true;
        }
        
        void OnExitingElement()
        {
            element.PointerExited -= OnExitingElement;
            DisableActiveSinglePopup();
        }
    }
}

public enum UILayer
{
    None,
    Group,
    Single
}