using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine.VFX;
using UnityEngine;
using Combat;

public sealed class EntityEffectParticleHandler : MonoBehaviour
{
    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private bool _hasIntensity = true;
    [ShowIf("_hasIntensity")] [SerializeField] private string _intensityFieldName = "Intensity";
    
    public void UpdateEffectStrength(float strength)
    {
        if (_hasIntensity) _visualEffect.SetFloat(_intensityFieldName, strength);
    }

    public void AdaptToEntity(CombatEntity entity)
    {
        Transform targetTransform = entity.transform;
        
        if (entity.ComponentsContainer.Has<DragAnimationObject>())
        {
            targetTransform = entity.ComponentsContainer.Get<DragAnimationObject>().transform;
        }
        
        transform.SetParent(targetTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        _visualEffect.Play();
    }

    public async UniTask Remove()
    {
        transform.SetParent(null);
        _visualEffect.Stop();

        try
        {
            await UniTask.WaitForSeconds(_visualEffect.GetFloat("MaxLifeTime") * 2, cancellationToken: destroyCancellationToken);
        }
        catch (Exception e) { TaskUtility.LogAsync(e); }
        
        gameObject.SetActive(false);
    }
}