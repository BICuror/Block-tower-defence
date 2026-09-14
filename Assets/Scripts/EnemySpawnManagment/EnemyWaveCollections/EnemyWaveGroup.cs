using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnemyWaveGroup", menuName = "SpawnManagement/EnemyWaveGroup")]

public sealed class EnemyWaveGroup : ScriptableObject
{
    [SerializeField] private List<EnemyGroupPart> _groupParts;
    
    [SerializeField] private BuildingAttackType _buildingAttackType;
    [SerializeField] private EnemyTier _waveGroupTier;
    
    public List<EnemyGroupPart> GroupParts => _groupParts;
    public BuildingAttackType BuildingAttackType => _buildingAttackType;
    public EnemyTier WaveGroupTier => _waveGroupTier;
    
    [Button] private void ParseAll()
    {
        _groupParts.ForEach(part => part.ParseEnemyGroupWeightContainers());
    }
}

[Serializable] public sealed class EnemyGroupPart
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private bool _useDefaultWeightContainer = true;
    [SerializeField] private bool _scaleByModificators = true;
    [AllowNesting] [ShowIf("_useDefaultWeightContainer")] [SerializeField] private EnemyWaveDefaultGroupWeightContainer _defaultGroupWeightContainer;
    [AllowNesting] [HideIf("_useDefaultWeightContainer")] [SerializeField] private EnemyGroupWeightContainer _enemyGroupWeightContainer;

    [SerializeField] private float _enemyAmountScale = 1f;
    
    public EnemyData Data => _enemyData;
    public bool ScaleByModificators => _scaleByModificators;

    public int GetEnemyAmount(int waveIndex)
    {
        int amount = 0;

        if (_useDefaultWeightContainer)
        {
            amount = _defaultGroupWeightContainer.EnemyGroupWeightContainer.GetAmount(waveIndex);
        }
        else
        {
            amount = _enemyGroupWeightContainer.GetAmount(waveIndex);
        }

        amount = Mathf.RoundToInt(amount * _enemyAmountScale);
        
        return amount;
    }

    public void ParseEnemyGroupWeightContainers()
    {
        _defaultGroupWeightContainer.ParseEnemyGroupWeightContainer();
        _enemyGroupWeightContainer.ParseEnemyGroupWeightContainer();
    }
}