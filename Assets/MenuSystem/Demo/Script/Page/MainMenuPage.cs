using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPage : BasePage<MainMenuPage>
{
    public void OnOptionMenuPressed()
    {
        PageManager.Instance.GetPage<OptionMenuPage>(true).Show();
    }

    public void OnSelectLevelMenuPressed()
    {
        PageManager.Instance.GetPage<SelectLevelPage>(true).Show();
    }

    public override void OnBackPressed()
    {
        base.OnBackPressed();
        if (isAnimationRunning) return;
        Debug.Log("Application Quit");
    }
}
