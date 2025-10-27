using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject popupUI;

    public void Show()
    {
        popupUI.SetActive(true);
        Time.timeScale = 0f; // pause
    }

    public void Hide()
    {
        popupUI.SetActive(false);
        Time.timeScale = 1f; // resume
    }
}
