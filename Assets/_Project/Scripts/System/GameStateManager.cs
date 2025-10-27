using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Boot,
    MainMenu,
    Loading,
    Playing,
    Paused,
    LevelComplete,
    GameOver
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Current State Info")]
    [SerializeField] private GameState currentState = GameState.Boot;
    public GameState CurrentState => currentState;

    // State change event
    public event Action<GameState, GameState> OnStateChanged;

    // Generic game event system
    private readonly Dictionary<string, Action> eventTable = new Dictionary<string, Action>();

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ------------------------
    // STATE MANAGEMENT
    // ------------------------
    public void SetState(GameState newState)
    {
        if (currentState == newState) return;

        var oldState = currentState;
        currentState = newState;

        Debug.Log($"[GameStateManager] {oldState} → {newState}");
        OnStateChanged?.Invoke(oldState, newState);

        HandleBuiltInStateLogic(newState);
    }

    private void HandleBuiltInStateLogic(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.LevelComplete:
                // Could trigger SoundManager.Instance.Play("level_complete");
                break;
            case GameState.GameOver:
                // Example: show game over UI
                break;
        }
    }

    // ------------------------
    // EVENT MANAGEMENT
    // ------------------------
    public void RegisterEvent(string eventName, Action callback)
    {
        if (!eventTable.ContainsKey(eventName))
            eventTable[eventName] = callback;
        else
            eventTable[eventName] += callback;
    }

    public void UnregisterEvent(string eventName, Action callback)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable[eventName] -= callback;
    }

    public void RaiseEvent(string eventName)
    {
        if (eventTable.TryGetValue(eventName, out Action action))
        {
            Debug.Log($"[GameStateManager] Event: {eventName}");
            action?.Invoke();
        }
        else
        {
            Debug.LogWarning($"[GameStateManager] No listeners for event '{eventName}'");
        }
    }

    public void PauseGame() => SetState(GameState.Paused);
    public void ResumeGame() => SetState(GameState.Playing);
    public void FinishLevel() => SetState(GameState.LevelComplete);
    public void GameOver() => SetState(GameState.GameOver);

    public void LoadScene(string sceneName)
    {
        SetState(GameState.Loading);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(sceneName).completed += _ => SetState(GameState.Playing);
    }
}
