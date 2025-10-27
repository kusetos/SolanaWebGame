using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenuPage : BasePage<OptionMenuPage>
{
    public Slider Slider;

    public void OnMagicButtonPressed()
    {
        PageManager.Instance.GetPage<AwesomePage>(true).Show(Slider.value);
    }
}
