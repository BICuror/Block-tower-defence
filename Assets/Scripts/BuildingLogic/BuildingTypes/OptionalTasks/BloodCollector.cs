
using WorldGeneration;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;
using CuroLocalization;

public sealed class BloodCollector : MonoBehaviour
{
    [Cached] private AreaEntityDetector _areaEntityDetector;
    [Cached] private EntityCanvas _entityCanvas;
    [Cached] private CombatEntity _ownerEntity;
    [Cached] private Inspectable _inspectable;
    
    [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
    [Inject] private EnemySpawnSystem _waveStateController;
    [Inject] private SpawnerRotator _spawnerRotator;
    [Inject] private ItemFactory _itemFactory;

    [SerializeField] private Sprite _barSprite;
    [SerializeField] private EntityCanvasBar _barPrefab;
    [SerializeField] private int _chargesToSpawn = 2;
    
    private int _entitiesKilledInArea;
    private EntityCanvasBar _bar;
    private int _requiredKills;
    
    private void Start()
    {
         _bar = _entityCanvas.AddBar(_barSprite, 0f, _barPrefab);
        
        _areaEntityDetector.RemovedItem += OnEntityRemoved;
        _waveStateController.LastWaveEnemyDied += CreateItem;
        _spawnerRotator.RotateSpawner(transform);
        _ownerEntity.Health.Died += Unsubscribe;
    }

    public void SetRequiredKills(int amount)
    {
        _requiredKills = amount;
        _inspectable.SetInspectableData(_inspectable.Name, _inspectable.Description.Replace("*", amount.ToString()));
    }

    private async void CreateItem()
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
        _waveStateController.LastWaveEnemyDied -= CreateItem;
        _ownerEntity.Health.Died -= Unsubscribe;
    }
}
