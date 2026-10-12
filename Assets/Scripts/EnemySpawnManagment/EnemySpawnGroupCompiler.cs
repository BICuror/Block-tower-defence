using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;

using Random = System.Random;

namespace Combat.EnemySpawnSystem
{
    public sealed class EnemySpawnGroupCompiler : MonoBehaviour
    {
        [Inject] private EnemySpawnSystemController _enemySpawnSystemController;
        [Inject] private EnemySpawnGroupContainer _enemySpawnGroupContainer;
        [Inject] private GlobalBuildingContainer _globalBuildingContainer;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private GlobalStatContainer _globalStatContainer;
        [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
        [Inject] private WaveIndexContainer _waveIndexContainer;
        [Inject] private WaveStateMachine _waveStateMachine;
        private bool _forceSpawnExistingBuildingAttackTypeGroups;
        private int _currentWaveSeed;
        private int _currentGroupSeed;

        private Random _random = new();

        private IslandData _islandData => _islandDataContainer.Data;
        public int CurrentWaveSeed => _currentWaveSeed;

        private void Start()
        {
            _globalStatContainer.Get<EnemyAmountMultiplier>().ValueChanged += _ => GenerateEnemyGroups();
        }

        public void RegenerateWaveSeed()
        {
            _currentWaveSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            _currentGroupSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        public void SetNextWaveData(bool forceExistingBuildingAttackWaves)
        {
            _enemySpawnGroupContainer.AdditionalGroups.Clear();

            _forceSpawnExistingBuildingAttackTypeGroups = forceExistingBuildingAttackWaves;
        }

        public void GenerateEnemyGroups()
        {
            GenerateMainEnemyGroups();
            GenerateAdditionalEnemyGroups();
        }

        private void GenerateAdditionalEnemyGroups()
        {
            _random = new Random(_currentGroupSeed);

            _enemySpawnGroupContainer.AdditionalGroups.ForEach(group =>
            {
                EnemySpawner randomEnemySpawner = _enemySpawnSystemController.Spawners[_random.Next(0, _enemySpawnSystemController.Spawners.Count)];

                _enemySpawnGroupContainer.EnemySpawnDatas[randomEnemySpawner].AddRange(GetEnemyGroupPartAmountModified(group.GroupParts));
            });
        }

        public void AddAdditionalEnemyGroup(AdditionalEnemyGroupDataConfig additionalEnemyGroup)
        {
            _enemySpawnGroupContainer.AdditionalGroups.Add(additionalEnemyGroup);
            GenerateEnemyGroups();
        }

        public void RemoveAdditionalEnemyGroup(AdditionalEnemyGroupDataConfig additionalEnemyGroup)
        {
            _enemySpawnGroupContainer.AdditionalGroups.Remove(additionalEnemyGroup);
            GenerateEnemyGroups();
        }

        private void GenerateMainEnemyGroups()
        {
            _enemySpawnGroupContainer.EnemySpawnDatas.Clear();

            _random = new Random(_currentWaveSeed);

            foreach (EnemySpawner spawner in _enemySpawnSystemController.Spawners)
            {
                List<EnemySpawnData> waveGroup = GetEnemyGroupPartAmountModified(FindSuitableRandomGroup().GroupParts);
                
                _enemySpawnGroupContainer.EnemySpawnDatas.Add(spawner, waveGroup);
            }
        }

        public List<EnemySpawnData> GetEnemyGroupPartAmountModified(List<EnemyGroupPart> groupParts)
        {
            return GetEnemyGroupPart(groupParts, _globalStatContainer.Get<EnemyAmountMultiplier>().Value);
        }

        public List<EnemySpawnData> GetEnemyGroupPart(List<EnemyGroupPart> groupParts, float amountMultiplier)
        {
            List<EnemySpawnData> groupEnemies = new List<EnemySpawnData>();

            for (int enemyGroupPartIndex = 0; enemyGroupPartIndex < groupParts.Count; enemyGroupPartIndex++)
            {
                EnemyGroupPart currentPart = groupParts[enemyGroupPartIndex];

                int amount = currentPart.GetEnemyAmount(_waveIndexContainer.GetCurrentWave());

                if (currentPart.ScaleByModificators) amount = Mathf.RoundToInt(amount * amountMultiplier);

                if (amount <= 0) amount = 1;
                
                groupEnemies.Add(new EnemySpawnData(currentPart.Config, amount));
            }

            return groupEnemies;
        }

        private EnemyWaveGroupConfig FindSuitableRandomGroup()
        {
            EnemyWaveGroupConfig[] waveGroups = _islandData.WavesData.WaveGroups;

            List<EnemyWaveGroupConfig> suitableGroups = new List<EnemyWaveGroupConfig>();

            for (int i = 0; i < waveGroups.Length; i++)
            {
                if (CanSpawnWaveGroup(waveGroups[i]))
                {
                    suitableGroups.Add(waveGroups[i]);
                }
            }

            return suitableGroups[_random.Next(0, suitableGroups.Count)];
        }

        private bool CanSpawnWaveGroup(EnemyWaveGroupConfig groupConfig)
        {
            WaveContent content = _waveIndexContainer.GetCurrentWaveContent();

            if (content.RequiredEnemyTier != groupConfig.WaveGroupTier) return false;

            if (!_forceSpawnExistingBuildingAttackTypeGroups) return true;

            List<BuildingAttackType> existingBuildingAttackTypes = new();

            _globalBuildingContainer.GetPlayerBuildings().ForEach(buildingEntity =>
            {
                if (!existingBuildingAttackTypes.Contains(buildingEntity.BuildingAttackType) &&
                    buildingEntity.BuildingAttackType != BuildingAttackType.None)
                {
                    existingBuildingAttackTypes.Add(buildingEntity.BuildingAttackType);
                }
            });

            return existingBuildingAttackTypes.Contains(groupConfig.BuildingAttackType);
        }
    }

    public record EnemySpawnData
    {
        public EnemyConfig Config;
        public int Amount;

        public EnemySpawnData(EnemyConfig config, int amount)
        {
            Config = config;
            Amount = amount;
        }
    }
}