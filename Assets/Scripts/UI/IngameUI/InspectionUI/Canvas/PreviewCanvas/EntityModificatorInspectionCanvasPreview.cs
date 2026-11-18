using Combat.Entity.Modificators;
using CuroLocalization;
using UnityEngine.UI;
using UnityEngine;

namespace Combat.IngameUI.Inspection
{
    public sealed class EntityModificatorInspectionCanvasPreview : InspectionCanvasBase
    {
        [SerializeField] private StaticTextLocalizer _previewName;
        [SerializeField] private Image _previewImage;
        private EntityModificatorConfig _entityModificatorConfig;
        
        public void Initialize(EntityModificatorConfig entityModificatorConfig, Transform target)
        {
            _entityModificatorConfig = entityModificatorConfig;   
            
            InitializeInspectionPanelBase(target.GetComponent<InspectableObject>());
            
            _previewImage.sprite = entityModificatorConfig.Icon;
        }
        
        protected override void UpdateAllParsableText()
        {
            _previewName.SetKey(_entityModificatorConfig.GetNameLocalizationKey(), ParseTextByDefault);
        }
    }
}