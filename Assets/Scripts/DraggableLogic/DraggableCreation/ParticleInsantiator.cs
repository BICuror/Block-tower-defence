using UnityEngine;

public sealed class ParticleInsantiator : MonoBehaviour
{
    [SerializeField] private VisualEffectHandler _particlePrefab;

    private ObjectPool<VisualEffectHandler> _effectPool;

    private void Awake()
    {
        _effectPool = new ObjectPool<VisualEffectHandler>(_particlePrefab, 2);
    } 

    public void InstantiateParticle(GameObject placedObject)
    {
        VisualEffectHandler visualEffect = _effectPool.GetNextPooledObject();

        visualEffect.transform.position = placedObject.transform.position;

        visualEffect.transform.rotation = placedObject.transform.rotation;
    
        visualEffect.Play();
    }
}
