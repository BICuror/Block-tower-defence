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
    
    private List<GameObject> _modificators = new();

    private void Start()
    {
        _entityHealth.EntityDied += _ => DestroyAllModificators();
    }

    public T InstantiateAndAddModificator<T>(GameObject prefab)
    {
        GameObject modificator = _diContainer.InstantiatePrefab(prefab, transform);
        
        AddModificator(modificator);
        
        return modificator.GetComponent<T>();
    }
    
    public void AddModificator(GameObject additionalObject)
    {
        additionalObject.transform.SetParent(transform);
        additionalObject.transform.localPosition = Vector3.zero;
        additionalObject.transform.localRotation = Quaternion.identity;
        
        _ownerComponentCacher.InjectCachedToObjectAndChildren(additionalObject);
        
        _modificators.Add(additionalObject);
    }

    public void DestroyModificator(GameObject additionalObject)
    {
        _modificators.Remove(additionalObject);

        Destroy(additionalObject);
    }

    private void DestroyAllModificators()
    {
        _modificators.ForEach(modificator => Destroy(modificator.gameObject));
        _modificators.Clear();
    }
}