using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [Header("AreaSettings")]
    protected float _height = 100f;
    [SerializeField] protected ReachAreaScale _reachAreaScale;
    [SerializeField] private GameObject _reachAreaCollider;

    private void Start() 
    {   
        _reachAreaScale = this.GetStat<ReachAreaScale>();
        UpdateScale();
    }

    public virtual void UpdateScale()
    {
        _reachAreaCollider.transform.localScale = GetScale();
    }

    public virtual Vector3 GetScale()
    {
        float scale = _reachAreaScale.Value * 2f + 0.95f;

        return new Vector3(scale, _height, scale);
    }
}
