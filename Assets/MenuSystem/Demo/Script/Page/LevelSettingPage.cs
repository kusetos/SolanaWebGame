using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettingPage : BasePage<LevelSettingPage>
{
    public void ClosePage()
    {
        PageManager.Instance.CloseAllPage();
        PageManager.Instance.GetPage<MainMenuPage>(true).Show();
    }
}
