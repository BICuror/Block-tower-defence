using CuroSceneManagement;
using UnityEngine;

namespace CuroLoading
{
    public sealed class LoadingScreenInitializer
    {
        private static readonly string PATH_TO_MAIN_LOADING_CONFIG = "Loading/LoadingScreenConfig";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            LoadingScreenInitializationConfig initializationConfig = Resources.Load<LoadingScreenInitializationConfig>(PATH_TO_MAIN_LOADING_CONFIG);
        
            LoadingScreen loadingScreen = Object.Instantiate(initializationConfig.LoadingScreenPrefab).GetComponent<LoadingScreen>();
            
            loadingScreen.Initialize(initializationConfig);
            
            Object.DontDestroyOnLoad(loadingScreen.gameObject);
        }
    }
}