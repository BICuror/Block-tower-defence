using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class BloodCollector : MonoBehaviour
{
    [Cached] private AreaEntityDetector _areaEntityDetector;
    [Cached] private InspectableObject _inspectableObject;
    [Cached] private EntityCanvas _entityCanvas;
    [Cached] private CombatEntity _ownerEntity;
    
    [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
    [Inject] private EnemySpawnSystem _waveStateController;
    [Inject] private ItemFactory _itemFactory;

    [SerializeField] private EntityCanvasBar _barPrefab;
    [SerializeField] private Sprite _barSprite;
    [SerializeField] private int _chargesToSpawn = 2;
    
    private int _entitiesKilledInArea;
    private EntityCanvasBar _bar;
    private int _requiredKills;
    
    private void Start()
    {
        _bar = _entityCanvas.AddBar(_barSprite, 0f, _barPrefab);
        
        _waveStateController.LastWaveEnemyDied += CreateItemAsync;
        _areaEntityDetector.RemovedItem += OnEntityRemoved;
        _ownerEntity.Health.Died += Unsubscribe;
    }

    public void SetRequiredKills(int amount)
    {
        _requiredKills = amount;
        _ownerEntity.ComponentsContainer.Get<InspectableObject>().ReplaceableDataParser.AddOrUpdateParsableData("{MinimalRequiredEnemiesToKill}", amount.ToString());
    }

    private void CreateItemAsync() => CreateItem().Forget();
    private async UniTask CreateItem()
    {
        Unsubscribe();
        
        if (_entitiesKilledInArea >= _requiredKills) await _upgradeChargeContainer.AddChargesWithAnimation(_chargesToSpawn, transform);
        
        _ownerEntity.Health.Die();
    }

    private void OnEntityRemoved(CombatEntity entity)
    {
        if (!entity.Health.IsAlive())
        {
            _entitiesKilledInArea++;
            _bar.SetValue(_entitiesKilledInArea / (float)_requiredKills);
        }
    }

    private void Unsubscribe()
    {
        _waveStateController.LastWaveEnemyDied -= CreateItemAsync;
        _ownerEntity.Health.Died -= Unsubscribe;
    }
}
