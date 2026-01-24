using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TrailRendererReseter : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;

    private async void OnEnable()
    {
        _trailRenderer.enabled = false;
        await UniTask.WaitForFixedUpdate();
        _trailRenderer.Clear();
        _trailRenderer.enabled = true;
    }

    private void OnDisable()
    {
        _trailRenderer.Clear();
    }
}