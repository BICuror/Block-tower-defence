using UnityEngine;
using Cashing;

public class AreaManager : MonoBehaviour
{
    [Header("AreaSettings")]
    protected float Height = 100f;
    [SerializeField] private GameObject _reachAreaCollider;
    [Cached] protected ReachAreaScale _reachAreaScale;
    
    private void Start()
    {
        _reachAreaScale.ValueChanged += _ => UpdateScale();
        UpdateScale();
    }

    private void UpdateScale()
    {
        _reachAreaCollider.transform.localScale = GetScale();
    }

    public virtual Vector3 GetScale()
    {
        float scale = _reachAreaScale.Value * 2f + 0.95f;

        return new Vector3(scale, Height, scale);
    }
}