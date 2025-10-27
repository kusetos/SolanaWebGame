using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLevelPage : BasePage<SelectLevelPage>
{
    public void OnLevelSettingPressed()
    {
        PageManager.Instance.GetPage<LevelSettingPage>(true).Show();
    }
}
