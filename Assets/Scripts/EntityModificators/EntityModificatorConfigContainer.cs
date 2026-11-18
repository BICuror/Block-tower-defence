using System.Collections.Generic;
using UnityEngine;

namespace Combat.Entity.Modificators
{
    [CreateAssetMenu(fileName = "EntityModificatorConfigContainer", menuName = "EntityModificators/EntityModificatorConfigContainer")]
    
    public sealed class EntityModificatorConfigContainer : ScriptableObject
    {
        [SerializeField] private List<EntityModificatorConfig> _entityModificatorDataList;
        
        public List<EntityModificatorConfig> EntityModificatorDataList => _entityModificatorDataList;
    }
}