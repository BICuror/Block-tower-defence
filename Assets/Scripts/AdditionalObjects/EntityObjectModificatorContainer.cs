using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityObjectModificatorContainer : MonoBehaviour
{
    [Cached] private EntityComponentCacher _ownerComponentCacher;
    [Inject] private DiContainer _diContainer;
    [Cached] private CombatEntity _ownerEntity;
    
    private List<EntityObjectModifier> _modificators = new();
    private List<GameObject> _gameObjectModificators = new();
    
    public bool CanBeAppliedToEntity(EntityObjectModifier modificatorPrefab, ArgumentsContainer argumentsContainer)
    {
        return modificatorPrefab.CanBeAppliedToEntity(_ownerEntity, argumentsContainer);
    }
    
    public EntityObjectModifier InstantiateAndAddModificator(EntityObjectModifier modificatorPrefab, ArgumentsContainer argumentsContainer)
    {
        EntityObjectModifier modificator = _diContainer.InstantiatePrefab(modificatorPrefab, transform).GetComponent<EntityObjectModifier>();
        
        _ownerEntity.InjectCachedToObjectAndChildren(modificator.gameObject);
        modificator.SetArgumentsContainer(argumentsContainer);
        AdaptObjectModifier(modificator.gameObject);
        _modificators.Add(modificator);

        return modificator;
    }

    public GameObject InstantiateAndAddModificator(GameObject modificatorPrefab)
    {
        GameObject modificator = _diContainer.InstantiatePrefab(modificatorPrefab, transform);
        
        _ownerEntity.InjectCachedToObjectAndChildren(modificator);
        AdaptObjectModifier(modificator.gameObject);
        _gameObjectModificators.Add(modificator);

        return modificator;
    }
    
    public void RemoveAndDestroyModificator(EntityObjectModifier modificator)
    {
        _modificators.Remove(modificator);

        Destroy(modificator.gameObject);
    }
    
    public void RemoveAndDestroyModificator(GameObject modificator)
    {
        _gameObjectModificators.Remove(modificator);

        Destroy(modificator);
    }
    
    public void DestroyAllModificators()
    {
        _modificators.ForEach(modificator => Destroy(modificator.gameObject));
        _modificators.Clear();
        
        _gameObjectModificators.ForEach(Destroy);
        _gameObjectModificators.Clear();
    }
    
    private void AdaptObjectModifier(GameObject objectModifier)
    {
        objectModifier.transform.SetParent(transform);
        objectModifier.transform.localPosition = Vector3.zero;
        objectModifier.transform.localRotation = Quaternion.identity;
    }
}