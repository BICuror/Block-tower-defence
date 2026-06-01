using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Cashing;
using Combat;

public abstract class OptionalTask : MonoBehaviour
{
    [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
    [Inject] private EnemySpawnSystem _waveStateController;
    [Inject] private SelectionManager _selectionManager;
    [Cached] protected EntityCanvas EntityCanvas;
    [Cached] protected CombatEntity OwnerEntity;
    
    [SerializeField] private int _chargesToSpawn = 2;
    private OptionalTaskRewardType _rewardType;

    [Header("UIData")]
    [SerializeField] private KeywordTooltipTagData _upgradeTooltipTagData;
    [SerializeField] private KeywordTooltipTagData _rerollTooltipTagData;
    
    protected void Start()
    {
        _waveStateController.LastWaveEnemyDied += GrantRewardsSync;
        OwnerEntity.Health.Died += OnDeath;
    }

    public void SetRewardType(OptionalTaskRewardType rewardType)
    {
        _rewardType = rewardType;
        int rewardAmount = 1;
        
        if (rewardType == OptionalTaskRewardType.UpgradeCharges) rewardAmount = _chargesToSpawn;
     
        ReplaceableDataParser dataParser = OwnerEntity.ComponentsContainer.Get<InspectableObject>().ReplaceableDataParser;
        
        dataParser.AddOrUpdateParsableData("{rewardAmount}", rewardAmount.ToString());

        KeywordTooltipTagData rewardTypeData = null;
        
        switch (rewardType)
        {
            case OptionalTaskRewardType.UpgradeCharges: rewardTypeData = _upgradeTooltipTagData; break;
            case OptionalTaskRewardType.Reroll: rewardTypeData = _rerollTooltipTagData; break;
        }
        
        dataParser.AddOrUpdateParsableData("{rewardType}", $"#{rewardTypeData.Tag}");
        EntityCanvas.AddIcon(rewardTypeData.IconSprite, true, rewardAmount);
    }
    
    protected abstract bool IsCompleted();

    private void GrantRewardsSync() => GrantRewards();
    private async UniTask GrantRewards()
    {
        if (IsCompleted())
        {
            switch (_rewardType)
            {
                case OptionalTaskRewardType.UpgradeCharges:
                {
                    await _upgradeChargeContainer.AddChargesWithAnimation(_chargesToSpawn, transform);
                } break;
                case OptionalTaskRewardType.Reroll:
                {
                    _selectionManager.AddRerolls(1);
                } break;
            }
        }
        
        OwnerEntity.Health.Die();
    }

    private void OnDeath()
    {
        _waveStateController.LastWaveEnemyDied -= GrantRewardsSync;
        OwnerEntity.Health.Died -= OnDeath;
    }
}