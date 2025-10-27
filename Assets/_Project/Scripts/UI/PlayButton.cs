using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void StartFirstLevel()
    {
        FadePanel.Instance.FadeIn(() => GameStateManager.Instance.LoadScene("level_1"));
    }

}
