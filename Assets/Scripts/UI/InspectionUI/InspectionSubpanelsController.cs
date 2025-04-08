using UnityEngine;
using System;
using Combat;

public sealed class InspectionSubpanelsController : MonoBehaviour
{
    [SerializeField] private Transform _subpanelsContainer;
    
    [SerializeField] private InspectionStatSubpanel _statSubpanelPrefab;
    [SerializeField] private InspectionStatDetailsSubpanel _statdetailsSubpanelPrefab;
    [SerializeField] private InspectionEffectSubPanel _effectSubpanelPrefab;
    [SerializeField] private InspectionKeywordSubpanel _keywordSubpanelPrefab;

    public CombatEntity _inspectedEntity;

    public void SetTooltipParser(TooltipParseTagDataContainer tooltipParseTagDataContainer)
    {
        DestoryAllSubpanels();

        tooltipParseTagDataContainer.StatTagDatas.ForEach(tagData =>
        {
            InspectionStatSubpanel statSubpanel = Instantiate(_statSubpanelPrefab, _subpanelsContainer);
            statSubpanel.Initialize(tagData);

            if (_inspectedEntity != null)
            {
                if (_inspectedEntity.StatContainer.Has(Type.GetType(tagData.AssociatedStatTypeName)))
                {
                    Stat stat = _inspectedEntity.StatContainer.Get(Type.GetType(tagData.AssociatedStatTypeName));
                    InspectionStatDetailsSubpanel statDetailsSubpanel = Instantiate(_statdetailsSubpanelPrefab, _subpanelsContainer);
                    statDetailsSubpanel.Initialize(stat, tagData);
                }
            }
        });

        tooltipParseTagDataContainer.EffectTagDatas.ForEach(tagData =>
        {
            InspectionEffectSubPanel effectSubpanel = Instantiate(_effectSubpanelPrefab, _subpanelsContainer);
            effectSubpanel.Initialize(tagData);
        });

        tooltipParseTagDataContainer.KeywordTagDatas.ForEach(tagData =>
        {
            InspectionKeywordSubpanel keywordSubpanel = Instantiate(_keywordSubpanelPrefab, _subpanelsContainer);
            keywordSubpanel.Initialize(tagData);
        });
    }
    
    private void DestoryAllSubpanels()
    {
        for (int i = 0; i < _subpanelsContainer.childCount; i++)
        {
            Destroy(_subpanelsContainer.GetChild(_subpanelsContainer.childCount - i - 1).gameObject);
        }
    }
}