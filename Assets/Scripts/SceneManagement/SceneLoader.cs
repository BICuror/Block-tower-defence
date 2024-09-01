using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public sealed class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void LoadScene()
    {
        DOTween.Clear();

        SceneManager.LoadScene(_sceneName);
    }
}
