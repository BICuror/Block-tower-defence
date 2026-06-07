using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine;

public sealed class RebindScreen : MonoBehaviour
{
    [SerializeField] private RebindSaveLoad _rebindSaveLoad;

    private void OnDisable()
    {
        _rebindSaveLoad.Save();
    }
}