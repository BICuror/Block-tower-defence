using Cysharp.Threading.Tasks;
using Combat.UpgradeCharges;
using Combat.IngameUI;
using UnityEngine;
using Zenject;
using Cashing;

namespace Combat.OptionalTasks
{
    public abstract class OptionalTask : MonoBehaviour
    {
        [Inject] private UpgradeChargeContainer _upgradeChargeContainer;
        [Inject] private SelectionManager _selectionManager;
        [Cached] protected EntityCanvas EntityCanvas;
        [Cached] protected CombatEntity OwnerEntity;
        
        [SerializeField] private int _chargesToSpawn = 2;
        private OptionalTaskRewardType _rewardType;
    
        [Header("UIData")]
        [SerializeField] private KeywordTooltipTagData _upgradeTooltipTagData;
        [SerializeField] private KeywordTooltipTagData _rerollTooltipTagData;
        
        protected abstract bool IsCompleted();
        
        public void SetReward(OptionalTaskRewardType rewardType)
        {
            _rewardType = rewardType;
            int rewardAmount = 1;
            
            if (rewardType == OptionalTaskRewardType.UpgradeCharges) rewardAmount = _chargesToSpawn;
            
            KeywordTooltipTagData rewardTypeData = null;
            
            switch (rewardType)
            {
                case OptionalTaskRewardType.UpgradeCharges: rewardTypeData = _upgradeTooltipTagData; break;
                case OptionalTaskRewardType.Reroll: rewardTypeData = _rerollTooltipTagData; break;
            }
            
            ReplaceableDataParser dataParser = OwnerEntity.ComponentsContainer.Get<InspectableObject>().ReplaceableDataParser;
            dataParser.AddOrUpdateParsableData("{rewardAmount}", rewardAmount.ToString());
            dataParser.AddOrUpdateParsableData("{rewardType}", $"#{rewardTypeData.Tag}");
            
            EntityCanvas.AddIcon(rewardTypeData.IconSprite, true, rewardAmount);
        }
    
        public async UniTask TryGrantRewards()
        {
            if (IsCompleted())
            {
                switch (_rewardType)
                {
                    case OptionalTaskRewardType.UpgradeCharges:
                    {
                        await _upgradeChargeContainer.AddCharge(transform, _chargesToSpawn);
                    } break;
                    case OptionalTaskRewardType.Reroll:
                    {
                        _selectionManager.AddRerolls(1);
                    } break;
                }
            }
            
            OwnerEntity.Health.Die();
        }
    }
}