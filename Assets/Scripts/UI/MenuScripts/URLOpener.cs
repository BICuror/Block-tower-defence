using UnityEngine;

public sealed class URLOpener : MonoBehaviour
{
    public void OpenURL(string url) => Application.OpenURL(url);
}
