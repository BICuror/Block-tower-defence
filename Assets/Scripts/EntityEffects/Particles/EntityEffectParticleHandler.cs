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
    
    [Header("InstantiationPosition")] 
    [SerializeField] private EffectInstantiationPosition _effectInstantiationPosition;
    [SerializeField] private float _yOffset;
    
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

        float yOffset = _yOffset;
        
        switch (_effectInstantiationPosition)
        {
            case EffectInstantiationPosition.Middle: break;
            case EffectInstantiationPosition.Top: yOffset += entity.ComponentsContainer.Get<DragAnimationObject>().MeshHeight; break;
            case EffectInstantiationPosition.Bottom: yOffset -= entity.ComponentsContainer.Get<DragAnimationObject>().MeshHeight; break;
        }
        
        transform.SetParent(targetTransform);
        transform.localPosition = new Vector3(0f, yOffset, 0);
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
    
    private enum EffectInstantiationPosition
    {
        Middle = 0,
        Top = 1,
        Bottom = 2
    }
}