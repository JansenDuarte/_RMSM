using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Image colorExposer;

    public Slider r_Picker;
    public Slider g_Picker;
    public Slider b_Picker;

    public void UI_ChangeColor()
    {
        colorExposer.color = new Color(r_Picker.value, g_Picker.value, b_Picker.value);
    }
}
