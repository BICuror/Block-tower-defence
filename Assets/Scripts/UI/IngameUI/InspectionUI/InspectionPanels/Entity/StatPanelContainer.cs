using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Combat;
using System;
using UnityEngine.Serialization;

public sealed class StatPanelContainer : ParserableTextContainer
{
    [SerializeField] private StatTooltipInvokingPanel _statTooltipInvokingPanelPrefab;
    [SerializeField] private Transform _statTooltipContainer;
    
    [Header("CustomTooltips")]
    [SerializeField] private List<StatTooltipInvokingPanel> _customStatTooltips;
    [SerializeField] private List<string> _customStatTypeNames;
    
    private List<StatTooltipInvokingPanel> _instantiatedTooltips = new();
    
    public Action<TooltipParseTagDataContainer> TooltipOpened;
    public Action TooltipClosed;

    private void Awake()
    {
        InitializeCustomTooltips();
    }

    public void SetInspectedEntity(CombatEntity entity)
    {
        for (int i = 0; i < _instantiatedTooltips.Count; i++)
        {
            Destroy(_instantiatedTooltips[i].gameObject);
        }
        
        _instantiatedTooltips.Clear();
        
        EnableCustomTooltips(entity);
        InitializeStatTooltips(entity);
    }

    private void InitializeStatTooltips(CombatEntity entity)
    {
        List<Stat> entityStats = entity.StatContainer.GetAllStats();

        _customStatTypeNames.ForEach(typeName => entityStats.RemoveAll(stat => stat.GetType() == Type.GetType(typeName)));
        
        entityStats.ForEach(stat =>
        {
            StatTooltipInvokingPanel tooltipInvokingPanel = Instantiate(_statTooltipInvokingPanelPrefab, _statTooltipContainer);
            _instantiatedTooltips.Add(tooltipInvokingPanel);
            
            tooltipInvokingPanel.SetStat(stat);
            tooltipInvokingPanel.CopyParsersFromContainer(this);
            
            tooltipInvokingPanel.TooltipOpened += (value) => TooltipOpened?.Invoke(value);
            tooltipInvokingPanel.TooltipClosed += () => TooltipClosed?.Invoke();
            
        });
    }
    
    private void EnableCustomTooltips(CombatEntity entity)
    {
        _customStatTypeNames.ForEach(typeName =>
        {
            Type statType = Type.GetType(typeName);
            
            bool hasStat = entity.StatContainer.Has(statType);

            StatTooltipInvokingPanel statTooltipInvokingPanel = _customStatTooltips[_customStatTypeNames.IndexOf(typeName)];
            
            statTooltipInvokingPanel.gameObject.SetActive(hasStat);
            
            if (hasStat)
            {
                statTooltipInvokingPanel.SetStat(entity.StatContainer.Get(statType));
            }
        });
    }
    
    private void InitializeCustomTooltips()
    {
        _customStatTooltips.ForEach(statTooltip =>
        {
            statTooltip.TooltipOpened += (value) => TooltipOpened?.Invoke(value);
            statTooltip.TooltipClosed += () => TooltipClosed?.Invoke();
        });
    }
}