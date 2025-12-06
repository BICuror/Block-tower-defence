using System.Collections.Generic;
using UnityEngine;
using System;
using Combat;

public sealed class ArtilleryTarget : WeaponBase
{
    [SerializeField] private AreaEntityDetector _areaEntityDetector;
    [SerializeField] private AreaScanerController _areaScanerController;
    
    public AreaEntityDetector AreaEntityDetector => _areaEntityDetector;
    
    public event Action AreaEntityAdded;
    
    public void Initialize(CombatEntity ownerEntity)
    {
        base.Initialize(ownerEntity);

        _areaEntityDetector.AddedItem += InvokeOnAreaEntityAdded;
        
        OwnerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += TryDamageAllEnemiesInArea;
        OwnerEntity.StatContainer.Get<ReachAreaScale>().ValueChanged += UpdateAreaScanerScale;
        
        UpdateAreaScanerScale(0f);
    }

    private void TryDamageAllEnemiesInArea()
    {
        float damage = OwnerEntity.StatContainer.Get<Damage>().Value;

        IReadOnlyList<CombatEntity> enemiesInArea = new List<CombatEntity>(_areaEntityDetector.GetList());
        
        for (int i = 0; i < enemiesInArea.Count; i++)
        {
            DamageEntity(damage, enemiesInArea[i]);
        }
    }

    private void UpdateAreaScanerScale(float value)
    {
        _areaScanerController.SetScale(OwnerEntity.StatContainer.Get<ReachAreaScale>().RoundedValue);
    }
    
    private void InvokeOnAreaEntityAdded(CombatEntity _) => AreaEntityAdded?.Invoke();

    private void OnDestroy()
    {
        _areaEntityDetector.AddedItem -= InvokeOnAreaEntityAdded;
        OwnerEntity.StatContainer.Get<ReachAreaScale>().ValueChanged -= UpdateAreaScanerScale;
        OwnerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed -= TryDamageAllEnemiesInArea;
    }
}