using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; } = null;

    void Awake()
    {
        // check if an instance already exists
        if (Instance != null)
        {
            Debug.LogError("Found another BootstrappedData on " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        Debug.Log("Bootstrap initialised!");
        Instance = this;

        DontDestroyOnLoad(gameObject);
        
    }


    public void LoadNextScene()
    {
        // Get the build index of the current active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the index of the next scene
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is within the valid range of scenes in the build settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene by its build index
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene available in Build Settings.");
            // Optionally, load a specific scene (e.g., main menu) if no next scene exists
            // SceneManager.LoadScene("MainMenuSceneName"); 
        }
    }
}

public static class PerformBootstrap
{
    const string SceneName = "Boot";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; ++sceneIndex)
        {
            var candidate = SceneManager.GetSceneAt(sceneIndex);

            if (candidate.name == SceneName)
                return;
        }

        Debug.Log("Loading bootstrap scene: " + SceneName);

        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
    }
}