using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ExtensionMethods;

public class SaveSlot_Struct : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI teamName_text;
    [SerializeField] TextMeshProUGUI teamNumber_text;
    [SerializeField] TextMeshProUGUI money_text;

    public GameObject selectSlot_pivot;
    public GameObject loadSlot_pivot;


    public void ChangeSlotName(string _teamName, bool _showOnlyName = true)
    {
        if (_showOnlyName)
        {
            teamNumber_text.text = "";
            money_text.text = "";
            ChangeVisibleButtons(false);
        }

        teamName_text.text = _teamName;
    }

    public void SetSlotAsFilled(string _teamName, string _teamNumber, string _teamColor, int _money)
    {
        teamName_text.text = _teamName;
        teamNumber_text.text = _teamNumber;
        teamNumber_text.color = _teamColor.StringToColor();
        money_text.text = _money.ToString();


        ChangeVisibleButtons(true);
    }

    public void ChangeVisibleButtons(bool _isSlotUsed)
    {
        if (_isSlotUsed)
        {
            selectSlot_pivot.SetActive(false);
            loadSlot_pivot.SetActive(true);
        }
        else
        {
            selectSlot_pivot.SetActive(true);
            loadSlot_pivot.SetActive(false);
        }
    }
}
