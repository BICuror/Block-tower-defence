using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine;
using Combat;
using System;

public sealed class TooltipStatContainer : MonoBehaviour
{
    [SerializeField] private StatTooltip _statTooltipPrefab;
    [SerializeField] private Transform _statTooltipContainer;
    
    [Header("CustomTooltips")]
    [SerializeField] private List<string> _customStatTypeNames;
    [SerializeField] private List<StatTooltip> _customStatTooltips;
    private List<StatTooltip> _instantiatedTooltips = new();
    
    public Action<TooltipParseTagDataContainer> TooltipOpened;
    public Action TooltipClosed;
    
    public void SetInspectedEntity(CombatEntity entity)
    {
        InitializeCustomTooltips(entity);
        InitializeStatTooltips(entity);
        SortStatTooltips();
    }

    private void InitializeStatTooltips(CombatEntity entity)
    {
        List<Stat> entityStats = entity.StatContainer.GetAllStats();

        _customStatTypeNames.ForEach(typeName => entityStats.RemoveAll(stat => stat.GetType() == Type.GetType(typeName)));
        
        entityStats.ForEach(stat =>
        {
            StatTooltip tooltip = Instantiate(_statTooltipPrefab, _statTooltipContainer);
            tooltip.SetStat(stat);
            tooltip.StatValueChanged += SortStatTooltips;
            tooltip.TooltipOpened += (value) => TooltipOpened?.Invoke(value);
            tooltip.TooltipClosed += () => TooltipClosed?.Invoke();
            
            _instantiatedTooltips.Add(tooltip);
        });
    }

    private void SortStatTooltips()
    {
        List<StatTooltip> statTooltips = _instantiatedTooltips.OrderByDescending(statTooltip => statTooltip.AssiociatedStat.IsModified).ToList();

        for (int i = 0; i < statTooltips.Count; i++)
        {
            statTooltips[i].transform.SetSiblingIndex(i + 1);
        }
    }

    private void InitializeCustomTooltips(CombatEntity entity)
    {
        _customStatTypeNames.ForEach(typeName =>
        {
            Type statType = Type.GetType(typeName);
            
            bool hasStat = entity.StatContainer.Has(statType);

            StatTooltip statTooltip = _customStatTooltips[_customStatTypeNames.IndexOf(typeName)];
            
            statTooltip.gameObject.SetActive(hasStat);
            
            if (hasStat)
            {
                statTooltip.SetStat(entity.StatContainer.Get(statType));
                statTooltip.TooltipOpened += (value) => TooltipOpened?.Invoke(value);
                statTooltip.TooltipClosed += () => TooltipClosed?.Invoke();
            }
        });
    }
}