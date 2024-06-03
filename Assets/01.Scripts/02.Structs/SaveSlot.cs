using UnityEngine;
using TMPro;
using ExtensionMethods;

public class SaveSlot : MonoBehaviour
{
    public void Change_SlotName(string _teamName, bool _showOnlyName = true)
    {
        if (_showOnlyName)
        {
            teamNumber_text.text = "";
            money_text.text = "";
            ChangeVisibleButtons(false);
        }

        teamName_text.text = _teamName;
    }

    public void Set_SlotAsFilled(string _teamName, string _teamNumber, string _teamColor, int _money)
    {
        teamName_text.text = _teamName;
        teamNumber_text.text = _teamNumber;
        teamNumber_text.color = _teamColor.StringToColor();
        money_text.text = _money.ToString();

        ChangeVisibleButtons(true);
    }

    private void ChangeVisibleButtons(bool _isSlotUsed)
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


    #region VARIABLES

    [SerializeField] private TextMeshProUGUI teamName_text;
    [SerializeField] private TextMeshProUGUI teamNumber_text;
    [SerializeField] private TextMeshProUGUI money_text;

    [SerializeField] private GameObject selectSlot_pivot;
    [SerializeField] private GameObject loadSlot_pivot;

    #endregion // VARIABLES
}
