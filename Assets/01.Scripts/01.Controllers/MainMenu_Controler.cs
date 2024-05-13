using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Controler : MonoBehaviour
{
    [SerializeField] SaveSlot_Struct[] mainMenuSaveSlots;

    [SerializeField] Canvas deleteWarning_Panel;
    [SerializeField] Animator sceneSwitcher;

    public void FillUsedSaveSlots(Saved_Game_Struct[] _saved_Games)
    {
        if (_saved_Games == null)
        {
            Debug.Log("<b>MainMenu Controler</b> - No saved data found");
            PrepareSaveSlotsInfo();
            return;
        }

        Debug.Log("<b>MainMenu Controler</b> - Loaded saved game data!");
        for (int i = 0; i < mainMenuSaveSlots.Length; i++)
        {
            if (_saved_Games[i].Team_Name != null && _saved_Games[i].Team_Name != "")
                mainMenuSaveSlots[i].SetSlotAsFilled(_saved_Games[i].Team_Name, _saved_Games[i].Car_Number.ToString(), _saved_Games[i].Car_Color, _saved_Games[i].Money);
            else
                mainMenuSaveSlots[i].ChangeSlotName("empty");
        }
    }

    public void PrepareSaveSlotsInfo()
    {
        for (int i = 0; i < mainMenuSaveSlots.Length; i++)
        {
            mainMenuSaveSlots[i].ChangeSlotName("empty");
        }
    }


    public void UI_LoadGame_At_SlotIndex(int _slotIndex)
    {
        GameManager.Instance.SelectedSaveSlot = _slotIndex;

        //TODO play animation and sound of load game

        GameManager.Instance.LoadGame(_slotIndex);
    }

    public void UI_StartNewGame_At_SlotIndex(int _slotIndex)
    {
        GameManager.Instance.SelectedSaveSlot = _slotIndex;

        //TODO: play animation and sounds of new game

        GameManager.Instance.LoadScene_Async((int)SceneCodex.TUTORIAL);
    }

    public void UI_WarnDeletion(int _slotIndex)
    {
        deleteWarning_Panel.enabled = true;
        indexSelectedForDeletion = _slotIndex;
    }

    int indexSelectedForDeletion = -1;

    public void UI_Delete_At_Slotindex()
    {
        mainMenuSaveSlots[indexSelectedForDeletion - 1].ChangeSlotName("empty");

        GameManager.Instance.DeleteSavedGame(indexSelectedForDeletion);
    }
}
