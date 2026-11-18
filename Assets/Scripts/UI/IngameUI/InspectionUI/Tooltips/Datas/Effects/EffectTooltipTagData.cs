using UnityEngine;

namespace Combat.IngameUI.Inspection.Tooltips
{
    [CreateAssetMenu(fileName = "EffectTooltipTagData", menuName = "Tooltips/Effects/TagData")]

    public sealed class EffectTooltipTagData : TooltipTagData
    {
        [SerializeField] private string _associatedEffectTypeName;

        public string AssociatedEffectTypeName => _associatedEffectTypeName;
    }
}