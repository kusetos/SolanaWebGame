using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwesomePage : Page<AwesomePage>
{
    public Image Background;
    public Text Title;

    public void Show(float awesomeness)
    {
        Open();
        
        Background.color = new Color32((byte)(129 * awesomeness), (byte)(197 * awesomeness), (byte)(34 * awesomeness), 255);
		Title.text = string.Format("This menu is {0:P} awesome", awesomeness);
    }

    public void Hide()
    {
        Close();
    }
}
