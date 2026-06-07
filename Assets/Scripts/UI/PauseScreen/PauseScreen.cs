using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
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
            else gameObject.SetActive(false);
        }
        else gameObject.SetActive(true);
    }

    private void Restart()
    {
        Destroy(gameObject);
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Quit() => Application.Quit();
    
    private void OnEnable()
    {
        _gameController.Disable();
        _timeController.Pause();
    }
    
    private void OnDisable()
    {
        _gameController.Enable();
        _timeController.Resume();
    }
}