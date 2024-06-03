using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial_Controler : MonoBehaviour
{

    [Header("Scene references")]
    [SerializeField] TMP_InputField teamName_Input;
    [SerializeField] TMP_InputField teamNumber_Input;
    [SerializeField] Image teamColor_Img;
    [SerializeField] Canvas colorPicker_Canvas;
    [SerializeField] GameObject nameYourTeam_Pivot;
    [SerializeField] GameObject pickRaceEngineer_Pivot;
    [SerializeField] GameObject raceEngineerDescription_Pivot;
    [SerializeField] GameObject pickDriver_Pivot;
    [SerializeField] GameObject driverDescription_Pivot;
    [SerializeField] GameObject pickPitCrewLeader_Pivot;
    [SerializeField] GameObject pitCrewLeaderDescription_Pivot;
    [SerializeField] ColorPicker colorPicker;


    [Header("Warning Canvas")]
    [SerializeField] Canvas warningCanvas;
    [SerializeField] TextMeshProUGUI warningText;
    [SerializeField][Range(1f, 5f)] float warningTimer;

    [Header("Pick Race Manager references")]
    [SerializeField] NPC_DataHelper[] raceManager_DataHelper;

    [Header("Pick Driver references")]
    [SerializeField] NPC_DataHelper[] driver_DataHelper;

    [Header("Pick Pit Crew Leader references")]
    [SerializeField] NPC_DataHelper[] pitCrewLeader_DataHelper;

    private bool m_warnedColorIsBlack = false;
    private Coroutine m_warningCO = null;
    private NpcStruct[] npcsGenerated = new NpcStruct[13];
    private NpcRaceEngineer[] raceEngineers = new NpcRaceEngineer[3];
    private NpcPitCrewLeader[] pitCrewLeaders = new NpcPitCrewLeader[3];
    private NpcPitCrewMember[] pitCrewMembers = new NpcPitCrewMember[4];
    private NpcDriver[] drivers = new NpcDriver[3];

    private int selected_raceEngineer = -1;
    private int selected_pitCrewLeader = -1;
    private int selected_driver = -1;

    private const int MIN_INITIAL_CONTRACT_VALUE = 10;
    private const int MAX_INITIAL_CONTRACT_VALUE = 16;

    private const int INITIAL_CASH = 10;


    private void Start()
    {
        GameManager.Instance.NpcGenerateInBulk(ref npcsGenerated);
    }


    public void UI_NameYourTeam_NextButtonClicked()
    {
        if (teamName_Input.text == "")
        {
            m_warningCO = StartCoroutine(ShowWarningPanel("You forgot your team's name!", Color.red));
            return;
        }
        else if (teamNumber_Input.text == "")
        {
            m_warningCO = StartCoroutine(ShowWarningPanel("You forgot your team's number!", Color.red));
            return;
        }
        else if (teamColor_Img.color == Color.black && !m_warnedColorIsBlack)
        {
            m_warningCO = StartCoroutine(ShowWarningPanel("You might have forgoten to change the color! If you chose black just click 'next' again!", Color.white, 2f));
            m_warnedColorIsBlack = true;
            return;
        }

        //Give info to the PlayerManager
        PlayerManager.Instance.SetTeamData(teamName_Input.text.ToUpper(), int.Parse(teamNumber_Input.text), teamColor_Img.color, INITIAL_CASH);

        nameYourTeam_Pivot.SetActive(false);
        pickRaceEngineer_Pivot.SetActive(true);

        for (int i = 0; i < raceEngineers.Length; i++)
        {
            raceEngineers[i] = new NpcRaceEngineer(npcsGenerated[i], Random.Range(MIN_INITIAL_CONTRACT_VALUE, MAX_INITIAL_CONTRACT_VALUE));
            raceManager_DataHelper[i].ShowNpcCard(ref raceEngineers[i]);
        }
    }

    public void UI_RaceEngineerSelected()
    {
        if (selected_raceEngineer < 0)
        {
            m_warningCO = StartCoroutine(ShowWarningPanel("Click one of the engineer's cards to select one!", Color.red));
            return;
        }

        pickRaceEngineer_Pivot.SetActive(false);
        raceEngineerDescription_Pivot.SetActive(true);

        PlayerManager.Instance.ChangeRaceEngineer(raceEngineers[selected_raceEngineer]);
    }

    public void UI_RaceEngineer_Description_NextButtonClicked()
    {
        for (int i = 0; i < drivers.Length; i++)
        {
            drivers[i] = new NpcDriver(npcsGenerated[i + 3], Random.Range(MIN_INITIAL_CONTRACT_VALUE, MAX_INITIAL_CONTRACT_VALUE));
            driver_DataHelper[i].ShowNpcCard(ref drivers[i]);
        }
    }

    public void UI_DriverSelected()
    {
        if (selected_driver < 0)
        {
            m_warningCO = StartCoroutine(ShowWarningPanel("Click one of the driver's cards to select one!", Color.red));
            return;
        }

        pickDriver_Pivot.SetActive(false);
        driverDescription_Pivot.SetActive(true);

        PlayerManager.Instance.ChangeDriver(drivers[selected_driver]);
    }

    public void UI_Driver_Description_NextButtonClicked()
    {
        for (int i = 0; i < pitCrewLeaders.Length; i++)
        {
            pitCrewLeaders[i] = new NpcPitCrewLeader(npcsGenerated[i + 6], Random.Range(MIN_INITIAL_CONTRACT_VALUE, MAX_INITIAL_CONTRACT_VALUE));
            pitCrewLeader_DataHelper[i].ShowNpcCard(ref pitCrewLeaders[i]);
        }
    }

    public void UI_PitCrewLeaderSelected()
    {
        if (selected_pitCrewLeader < 0)
        {
            m_warningCO = StartCoroutine(ShowWarningPanel("Click one of the leader's cards to select one!", Color.red));
            return;
        }

        pickPitCrewLeader_Pivot.SetActive(false);
        pitCrewLeaderDescription_Pivot.SetActive(true);

        for (int i = 0; i < pitCrewMembers.Length; i++)
        {
            pitCrewMembers[i] = new NpcPitCrewMember(npcsGenerated[i + 9], Random.Range(MIN_INITIAL_CONTRACT_VALUE, MAX_INITIAL_CONTRACT_VALUE));
        }
        pitCrewLeaders[selected_pitCrewLeader].pitCrew = pitCrewMembers;

        PlayerManager.Instance.ChangePitCrewLeader(pitCrewLeaders[selected_pitCrewLeader]);
    }

    public void UI_FinishTutorial()
    {
        PlayerManager.Instance.PrepareDataAndSave();

        GameManager.Instance.LoadScene_Async((int)SceneCodex.MANAGER);
    }








    public void UI_Select_RaceEngineer_Index(int _index)
    {
        selected_raceEngineer = _index;
    }

    public void UI_Select_PitCrewLeader_Index(int _index)
    {
        selected_pitCrewLeader = _index;
    }

    public void UI_Select_Driver_Index(int _index)
    {
        selected_driver = _index;
    }


    public void UI_OpenColorPicker() { colorPicker_Canvas.enabled = true; }

    public void UI_ConfirmColorChange()
    {
        teamColor_Img.color = colorPicker.colorExposer.color;
        colorPicker_Canvas.enabled = false;
    }


    public void UI_WarningPanelForceClose()
    {
        if (m_warningCO != null)
            StopCoroutine(m_warningCO);
    }

    IEnumerator ShowWarningPanel(string _msg, Color _msgColor, float _aditionalWait = 0f)
    {
        warningText.color = _msgColor;
        warningText.text = _msg;
        warningCanvas.enabled = true;

        yield return new WaitForSeconds(warningTimer + _aditionalWait);

        warningCanvas.enabled = false;

        yield break;
    }
}
