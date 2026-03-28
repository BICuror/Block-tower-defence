using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityModificatorDataContainer", menuName = "EntityModificators/EntityModificatorDataContainer")]

public sealed class EntityModificatorDataContainer : ScriptableObject
{
    [SerializeField] private List<EntityModificatorData> _entityModificatorDataList;
    
    public List<EntityModificatorData> EntityModificatorDataList => _entityModificatorDataList;
}