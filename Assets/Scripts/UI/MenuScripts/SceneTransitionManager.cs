using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public sealed class SceneTransitionManager : MonoBehaviour
{
    [Header("FadeAnimationSettings")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration;

    private string _nextSceneName;

    public void Awake()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.DOFade(0f, _fadeDuration).OnComplete(DisableCanvasGroupObject);
    }

    private void DisableCanvasGroupObject() => _canvasGroup.gameObject.SetActive(false);

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        _nextSceneName = sceneName;

        _canvasGroup.gameObject.SetActive(true);

        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, _fadeDuration).OnComplete(StartLoadingLevel);
    }

    private void StartLoadingLevel() => StartCoroutine(LoadLevelAsync());

    private IEnumerator LoadLevelAsync()
    {
        YieldInstruction yieldInstruction = new WaitForFixedUpdate();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_nextSceneName);

        while (loadOperation.isDone == false)
        {
            yield return null;
        } 
    }

}
