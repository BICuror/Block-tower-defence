using System.Collections.Generic;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;

public sealed class EntityObjectModificatorContainer : MonoBehaviour
{
    [Inject] private DiContainer _diContainer;
    [Cached] private EntityComponentCacher _ownerComponentCacher;
    [Cached] private EntityHealth _entityHealth;
    [Cached] private CombatEntity _ownerEntity;
    
    private List<EntityObjectModifier> _modificators = new();
    private List<GameObject> _gameObjectModificators = new();

    private void Start()
    {
        _entityHealth.EntityDied += _ => DestroyAllModificators();
    }

    public bool CanBeAppliedToEntity(EntityObjectModifier modificatorPrefab)
    {
        return modificatorPrefab.CanBeAppliedToEntity(_ownerEntity);
    }
    
    public EntityObjectModifier InstantiateAndAddModificator(EntityObjectModifier modificatorPrefab)
    {
        EntityObjectModifier modificator = _diContainer.InstantiatePrefab(modificatorPrefab, transform).GetComponent<EntityObjectModifier>();
        
        AdaptObjectModifier(modificator.gameObject);
        _modificators.Add(modificator);

        return modificator;
    }

    public GameObject InstantiateAndAddModificator(GameObject modificatorPrefab)
    {
        GameObject modificator = _diContainer.InstantiatePrefab(modificatorPrefab, transform);
        
        AdaptObjectModifier(modificator.gameObject);
        _gameObjectModificators.Add(modificator);

        return modificator;
    }
    
    public void RemoveAndDestroyModificator(EntityObjectModifier modificator)
    {
        _modificators.Remove(modificator);

        Destroy(modificator);
    }
    
    public void RemoveAndDestroyModificator(GameObject modificator)
    {
        _gameObjectModificators.Remove(modificator);

        Destroy(modificator);
    }
    
    private void AdaptObjectModifier(GameObject objectModifier)
    {
        objectModifier.transform.SetParent(transform);
        objectModifier.transform.localPosition = Vector3.zero;
        objectModifier.transform.localRotation = Quaternion.identity;
        
        _ownerComponentCacher.InjectCachedToObjectAndChildren(objectModifier.gameObject);
    }

    private void DestroyAllModificators()
    {
        _modificators.ForEach(modificator => Destroy(modificator.gameObject));
        _modificators.Clear();
        
        _gameObjectModificators.ForEach(Destroy);
        _gameObjectModificators.Clear();
    }
}