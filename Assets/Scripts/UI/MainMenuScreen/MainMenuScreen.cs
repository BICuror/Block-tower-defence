using CuroSceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using WorldGeneration;

public sealed class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private IslandDataContainer _islandDataContainer;
    [SerializeField] private IslandData _defaultIslandData;
    [SerializeField] private IslandData _tutorialIslandData;
    
    [SerializeField] private Button _playGameButton;
    [SerializeField] private Button _startTutorialButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _rebindButton;
    [SerializeField] private Button _feedbackButton;
    [SerializeField] private Button _quitButton;
    
    [SerializeField] private SettingsScreen _settingsScreen;
    [SerializeField] private RebindScreen _rebindScreen;
    
    private void Awake()
    {
        _playGameButton.onClick.AddListener(LoadPlayScene);
        _startTutorialButton.onClick.AddListener(LoadTutorialScene);
        _settingsButton.onClick.AddListener(() => _settingsScreen.gameObject.SetActive(true));
        _rebindButton.onClick.AddListener(() => _rebindScreen.gameObject.SetActive(true));
        _feedbackButton.onClick.AddListener(OpenFeedback);
        _quitButton.onClick.AddListener(Quit);
    }

    private void LoadPlayScene()
    {
        _islandDataContainer.SetData(_defaultIslandData);
        
        SceneManager.LoadScene("PlayScene").Forget();
    }
    
    private void LoadTutorialScene()
    {
        _islandDataContainer.SetData(_tutorialIslandData);
        
        SceneManager.LoadScene("PlayScene").Forget();
    }
    
    private void OpenFeedback() {}
    private void Quit() => Application.Quit();
}