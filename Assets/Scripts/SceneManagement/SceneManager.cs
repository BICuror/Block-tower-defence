using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace CuroSceneManagement
{
    public static class SceneManager
    {
        public static UniTask LoadScene(int index)
        {
            return LoadScene(UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(index).name);
        }
        
        public static async UniTask LoadScene(string sceneName)
        {
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;

            LoadingScreen.Instance.StartPlayingLoadingIconAnimation().Forget();
            await LoadingScreen.Instance.Enable();
            
            await UniTask.WaitUntil(() => asyncOperation.progress >= 0.9f);

            DOTween.KillAll();
            
            asyncOperation.allowSceneActivation = true;
            
            await UniTask.DelayFrame(2);
            
            LoadingScreen.Instance.StopPlayingLoadingIconAnimation().Forget();
            await LoadingScreen.Instance.Disable();
        }
    }
}