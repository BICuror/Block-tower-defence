using System.Collections.Generic;

namespace Combat.EnemySpawnSystem
{
    public sealed class EnemySpawnGroupContainer
    {
        public Dictionary<EnemySpawner, List<EnemySpawnData>> EnemySpawnDatas = new();
        public List<AdditionalEnemyGroupDataConfig> AdditionalGroups = new();
        
        public List<EnemySpawnData> GetEnemySpawnDataForSpawner(EnemySpawner spawner) => EnemySpawnDatas[spawner];

        public int GetTotalSpawnCount()
        {
            int count = 0;

            foreach (EnemySpawner spawner in EnemySpawnDatas.Keys)
            {
                count += GetEnemySpawnCount(spawner);
            }
            
            return count;
        }
        
        public int GetEnemySpawnCount(EnemySpawner spawner)
        {
            int count = 0;

            foreach (EnemySpawnData enemySpawnData in EnemySpawnDatas[spawner])
            {
                count += enemySpawnData.Amount;
            }
            
            return count;
        }
        
        public List<EnemyConfig> GetAllEnemyConfigs()
        {
            List<EnemyConfig> result = new();

            AdditionalGroups.ForEach(group => group.GroupParts.ForEach(groupPart => result.Add(groupPart.Config)));

            foreach (List<EnemySpawnData> enemySpawnDatas in EnemySpawnDatas.Values)
            {
                foreach (EnemySpawnData enemySpawnData in enemySpawnDatas)
                {
                    result.Add(enemySpawnData.Config);
                }
            }

            return result;
        }
    }
}