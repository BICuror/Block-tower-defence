using System.Collections.Generic;
using UnityEngine;
using Configs;
using UnityEngine.Serialization;

namespace Combat.Tiles.Modificators
{
    [CreateAssetMenu(menuName = "Combat/Tiles/TileModificators/ModificatorConfig")]
    
    public sealed class TileModificatorConfig : UpdatableScriptableObject
    {
        [FormerlySerializedAs("_logicDomainConfig")] [SerializeField] private DomainConfig domainConfig;
        
        public DomainConfig DomainConfig => domainConfig;
        
        protected override IEnumerable<INonScriptableObjectUpdatable> GetAllNonScriptableObjectUpdatables()
        {
            return domainConfig.GetAllUpdatables();
        }
    }
}