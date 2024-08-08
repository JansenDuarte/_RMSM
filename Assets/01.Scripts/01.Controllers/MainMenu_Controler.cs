using UnityEngine;

public class MainMenu_Controler : MonoBehaviour
{
    [SerializeField] SaveSlot[] mainMenuSaveSlots;

    [SerializeField] Canvas deleteWarning_Panel;

    private int indexSelectedForDeletion = -1;

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
                mainMenuSaveSlots[i].Set_SlotAsFilled(_saved_Games[i].Team_Name, _saved_Games[i].Car_Number.ToString(), _saved_Games[i].Car_Color, _saved_Games[i].Money);
            else
                mainMenuSaveSlots[i].Change_SlotName("empty");
        }
    }

    public void PrepareSaveSlotsInfo()
    {
        for (int i = 0; i < mainMenuSaveSlots.Length; i++)
        {
            mainMenuSaveSlots[i].Change_SlotName("empty");
        }
    }


    public void UI_LoadGame_At_SlotIndex(int _slotIndex)
    {
        GameManager.Instance.SelectedSaveSlot = _slotIndex;

        GameManager.Instance.LoadGame(_slotIndex);
    }

    public void UI_StartNewGame_At_SlotIndex(int _slotIndex)
    {
        GameManager.Instance.SelectedSaveSlot = _slotIndex;

        GameManager.Instance.LoadScene_Async((int)SceneCodex.TUTORIAL);
    }

    public void UI_WarnDeletion(int _slotIndex)
    {
        deleteWarning_Panel.enabled = true;
        indexSelectedForDeletion = _slotIndex;
    }


    public void UI_Delete_At_Slotindex()
    {
        mainMenuSaveSlots[indexSelectedForDeletion - 1].Change_SlotName("empty");

        GameManager.Instance.DeleteSavedGame(indexSelectedForDeletion);
    }
}
