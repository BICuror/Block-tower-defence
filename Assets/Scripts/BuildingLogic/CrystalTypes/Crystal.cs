using UnityEngine;
using UnityEngine.Events;
using Zenject;

public abstract class Crystal : DraggableObject
{
    [SerializeField] private LayerSetting _townhallLayer;
    [SerializeField] private Launcher _crystalLauncher;
    [SerializeField] private VisualEffectHandler _explotionVisualEffectHandler;
    [Inject] private CrystalManager _crystalManager;

    public UnityEvent CrystalUsed;

    private void OnEnable()
    {
        _crystalManager.CrystalSet.AddListener(Destroy);
    }

    public Launcher GetLauncher() => _crystalLauncher;

    /*public override bool CanBePlacedAt(float x, float z, LayerSetting layerSetting)
    {
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(x, 10000f, z), Vector3.down, Mathf.Infinity, layerSetting.GetLayerMask());

        if (hits.Length == 0) return true;
        else if (hits.Length == 1 && hits[0].collider.gameObject == gameObject) return true;
        else if (hits.Length >= 2)
        {
            return Physics.Raycast(new Vector3(x, 10000f, z), Vector3.down, Mathf.Infinity, _townhallLayer.GetLayerMask());
        } 
        else return false;
    }*/

    public void DisableDraggableComponent() => SetDraggableState(false);
    protected void StopFromDestroying() => _crystalManager.CrystalSet.RemoveListener(Destroy);

    public abstract void Activate();

    public void Destroy() 
    {
        CreateExplotion();

        Destroy(gameObject);
    }

    private void CreateExplotion()
    {
        VisualEffectHandler explotionHandler = Instantiate(_explotionVisualEffectHandler, transform.position, Quaternion.identity);

        explotionHandler.Play();
    }
}
