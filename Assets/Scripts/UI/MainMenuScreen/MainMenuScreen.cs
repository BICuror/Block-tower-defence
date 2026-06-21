using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public sealed class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private Button _playGameButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _rebindButton;
    [SerializeField] private Button _feedbackButton;
    [SerializeField] private Button _quitButton;
    
    [SerializeField] private SettingsScreen _settingsScreen;
    [SerializeField] private RebindScreen _rebindScreen;
    
    private void Awake()
    {
        _playGameButton.onClick.AddListener(LoadPlayScene);
        _settingsButton.onClick.AddListener(() => _settingsScreen.gameObject.SetActive(true));
        _rebindButton.onClick.AddListener(() => _rebindScreen.gameObject.SetActive(true));
        _feedbackButton.onClick.AddListener(OpenFeedback);
        _quitButton.onClick.AddListener(Quit);
    }

    private void LoadPlayScene() => SceneManager.LoadScene(1);
    private void OpenFeedback() {}
    private void Quit() => Application.Quit();
}