using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Cashing;
using Combat;

public sealed class BloodCollector : OptionalTask
{
    [Cached] private AreaEntityDetector _areaEntityDetector;

    [Header("CollectionAnimation")]
    [SerializeField] private AnimationCurve _horizontalAnimationCurve;
    [SerializeField] private AnimationCurve _verticalAnimationCurve;
    [SerializeField] private GameObject _bloodDropPrefab;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _animationDuration;
    
    [Header("VFX")]
    [SerializeField] private Transform _bloodFountanTransform;
    [SerializeField] private float _startingScale = 0.1f;
    [SerializeField] private float _finalScale = 0.3f;
    
    [Header("UI")]
    [SerializeField] private EntityCanvasBar _barPrefab;
    [SerializeField] private Sprite _completedIcon;
    [SerializeField] private Sprite _barSprite;
    
    private EntityCanvasIcon _killsLeftIcon;
    private int _entitiesKilledInArea;
    private EntityCanvasBar _bar;
    private int _requiredKills;
    
    private void Start()
    {
        base.Start();
        
        _bar = EntityCanvas.AddBar(_barSprite, 0f, _barPrefab);
        
        _areaEntityDetector.RemovedItem += OnEntityRemoved;
    }

    protected override bool IsCompleted() => _entitiesKilledInArea >= _requiredKills;

    public void SetRequiredKills(int amount)
    {
        _requiredKills = amount;
        _killsLeftIcon = EntityCanvas.AddIcon(_barSprite, true, _requiredKills);
        OwnerEntity.ComponentsContainer.Get<InspectableObject>().ReplaceableDataParser.AddOrUpdateParsableData("{MinimalRequiredEnemiesToKill}", amount.ToString());
    }

    private void OnEntityRemoved(CombatEntity entity)
    {
        if (!entity.Health.IsAlive())
        {
            IncreaseKilledEntities(entity.transform.position).Forget();
        }
    }

    private async UniTask IncreaseKilledEntities(Vector3 entityDeathPosition)
    {
        AnimateBloodDrop(entityDeathPosition).Forget();
        
        if (_entitiesKilledInArea < _requiredKills)
        {
            _entitiesKilledInArea++;
            
            float progress = _entitiesKilledInArea / (float)_requiredKills;
            float vfxScale = Mathf.Lerp(_startingScale, _finalScale, progress);
            
            _bloodFountanTransform.DOKill();
            _bloodFountanTransform.DOScale(new Vector3(vfxScale, vfxScale, vfxScale), 0.5f).From(_bloodFountanTransform.localScale).SetLink(_bloodFountanTransform.gameObject);
            
            if (_entitiesKilledInArea >= _requiredKills)
            {
                await _bar.SetValue(1f);
                
                EntityCanvas.RemoveBar(_bar);
                EntityCanvas.RemoveIcon(_killsLeftIcon);
                EntityCanvas.AddIcon(_completedIcon);
            }
            else
            {
                _killsLeftIcon.SetValue(_requiredKills - _entitiesKilledInArea);
                await _bar.SetValue(progress);
            }
        }
    }
    
    private async UniTask AnimateBloodDrop(Vector3 startPosition)
    {
        GameObject bloodDrop = Instantiate(_bloodDropPrefab, startPosition, Quaternion.identity);
        
        Vector3 endPosition = _targetTransform.position;
        
        await DOVirtual.Float(0f, 1f, _animationDuration, MoveCharge).SetLink(gameObject).AsyncWaitForCompletion();
        
        void MoveCharge(float progress)
        {
            Vector3 lerpedPosition = Vector3.LerpUnclamped(startPosition, endPosition, _horizontalAnimationCurve.Evaluate(progress));

            lerpedPosition.y += _verticalAnimationCurve.Evaluate(progress);
            
            bloodDrop.transform.position = lerpedPosition;
        }
        
        Destroy(bloodDrop.gameObject);
    }
}