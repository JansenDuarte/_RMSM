using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsPanelHelper : MonoBehaviour
{
    [SerializeField] private Slider BGM_Slider;
    [SerializeField] private TextMeshProUGUI BGM_Value;

    [SerializeField] private Slider SFX_Slider;
    [SerializeField] private TextMeshProUGUI SFX_Value;

    [SerializeField] private TextMeshProUGUI Units_BtnText;


    private void Awake()
    {
        InitialySetOptions();
    }


    private void InitialySetOptions()
    {
        BGM_Slider.SetValueWithoutNotify(AudioManager.Instance.BGM_Volume);
        BGM_Value.text = AudioManager.Instance.BGM_Volume.ToString();

        SFX_Slider.SetValueWithoutNotify(AudioManager.Instance.SFX_Volume);
        SFX_Value.text = AudioManager.Instance.SFX_Volume.ToString();

        Units_BtnText.text = GetUnitString(GameManager.Instance.Prefered_Unit);
        Debug.Log("<b>Options Panel Helper</b> - Finished initial options set up");
    }

    private string GetUnitString(Units _units)
    {
        string _returnValue = _units switch
        {
            Units.METRIC => "km/h",
            Units.IMPERIAL => "mph",
            _ => "",
        };
        return _returnValue;
    }


    #region UI_CALLED_METHODS

    public void UI_ChangePreferedUnits()
    {
        GameManager.Instance.ChangePreferedUnits();

        Units_BtnText.text = GetUnitString(GameManager.Instance.Prefered_Unit);
    }


    public void UI_SoundSliderChangedValue()
    {
        AudioManager.Instance.BGM_Volume = (int)BGM_Slider.value;
        BGM_Value.text = AudioManager.Instance.BGM_Volume.ToString();

        AudioManager.Instance.SFX_Volume = (int)SFX_Slider.value;
        SFX_Value.text = AudioManager.Instance.SFX_Volume.ToString();
    }


    public void UI_CloseOptionsPanel()
    {
        GameManager.Instance.SavePlayerPrefs();

        gameObject.SetActive(false);
    }

    #endregion // UI_CALLED_METHODS
}
