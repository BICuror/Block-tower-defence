using UnityEngine;

public sealed class EffectPanel : MonoBehaviour
{
    private Effect _effect;

    public void SetEffect(Effect effectToSet)
    {
        _effect = effectToSet;

        gameObject.GetComponent<MeshRenderer>().material = effectToSet.UIMaterial;
    }

    public bool CompareEffect(Effect effectToCompare) => effectToCompare == _effect;
}
