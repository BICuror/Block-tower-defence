using CuroSceneManagement;
using UnityEngine.UI;
using GameControls;
using UnityEngine;
using DG.Tweening;
using Zenject;

public sealed class PauseScreen : MonoBehaviour
{
    [Inject] private TimeController _timeController;
    [Inject] private GameController _gameController;
    
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _rebindButton;
    [SerializeField] private Button _restatButton;
    [SerializeField] private Button _quitButton;
    
    [SerializeField] private SettingsScreen _settingsScreen;
    [SerializeField] private RebindScreen _rebindScreen;
    
    private float _capturedTimeScale;

    private void Awake()
    {
        _continueButton.onClick.AddListener(QuitCurrentState);
        _settingsButton.onClick.AddListener(() => _settingsScreen.gameObject.SetActive(true));
        _rebindButton.onClick.AddListener(() => _rebindScreen.gameObject.SetActive(true));
        _restatButton.onClick.AddListener(Restart);
        _quitButton.onClick.AddListener(Quit);
    }

    public void QuitCurrentState()
    {
        if (gameObject.activeSelf)
        {
            if (_rebindScreen.gameObject.activeSelf) _rebindScreen.gameObject.SetActive(false);
            else if (_settingsScreen.gameObject.activeSelf) _settingsScreen.gameObject.SetActive(false);
            else Disable();
        }
        else Enable();
    }

    private void Restart()
    {
        CleanUp();
        SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void Quit()
    {
        CleanUp();
        SceneManager.LoadScene("Menu");
    }

    private void CleanUp()
    {
        _gameController.Dispose();
    }
    
    private void Enable()
    {
        gameObject.SetActive(true);
        _gameController.Disable();
        _timeController.Pause();
    }
    
    private void Disable()
    {
        gameObject.SetActive(false);
        _gameController.Enable();
        _timeController.Resume();
    }
}