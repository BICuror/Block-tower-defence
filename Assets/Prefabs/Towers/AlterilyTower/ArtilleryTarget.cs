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
    
    protected override void OnInitialized()
    {
        _areaEntityDetector.AddedItem += InvokeOnAreaEntityAdded;
        
        OwnerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += TryDamageAllEnemiesInArea;
        OwnerEntity.ComponentsContainer.Get<AreaManager>().AddAreaScanerController(_areaScanerController);
        
        _areaScanerController.AreaVisualisation.SubscribeToHoverable(OwnerEntity.ComponentsContainer.Get<HoverableObject>());
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
    
    private void InvokeOnAreaEntityAdded(CombatEntity _) => AreaEntityAdded?.Invoke();

    private void OnDestroy()
    {
        _areaEntityDetector.AddedItem -= InvokeOnAreaEntityAdded;
        
        OwnerEntity.ComponentsContainer.Get<AreaManager>().RemoveAreaScanerController(_areaScanerController);
        OwnerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed -= TryDamageAllEnemiesInArea;
        _areaScanerController.AreaVisualisation.UnsubscribeFromHoverable(OwnerEntity.ComponentsContainer.Get<HoverableObject>());
    }
}