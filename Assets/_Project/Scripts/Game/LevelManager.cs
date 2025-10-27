using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Events")]
    public UnityEvent onLevelStart;
    public UnityEvent onLevelRestart;
    public UnityEvent onLevelComplete;
    public UnityEvent onPlayerHited;

    void Awake()
    {
        // Singleton pattern (scene-only)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {


        StartLevel();
    }


    public void StartLevel()
    {
        onLevelStart?.Invoke();


        FadePanel.Instance.FadeOut();

    }

    public void CompleteLevel()
    {

        Debug.Log("[LevelManager] Level complete!");
        FadePanel.Instance.FadeIn(() => Bootstrapper.Instance.LoadNextScene());

        onLevelComplete?.Invoke();
    }

    public void RestartLevel()
    {
        Debug.Log("[LevelManager] Restarting level...");
        DialogueManager.Instance.ForceStopDialogue();
        onLevelRestart?.Invoke();
        FadePanel.Instance.FadeIn(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
    }

    public void PlayerHited()
    {
        Debug.Log("[LevelManager] Player died.");
        onPlayerHited?.Invoke();
    }

    public void GoToNextLevel(string nextSceneName)
    {
        Debug.Log($"[LevelManager] Loading next level: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}
