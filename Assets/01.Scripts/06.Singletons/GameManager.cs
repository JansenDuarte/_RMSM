using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExtensionMethods;

public class GameManager : MonoBehaviour
{
    #region SIMPLE_SINGLETON
    private static GameManager m_instance;
    public static GameManager Instance { get { return m_instance; } }
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            ScreenConfigurations();
            VerifyAndSetPlayerPrefs();
            SceneManager.sceneLoaded += ActiveSceneChanged;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion




    #region Initial_Configurations

    public GameDate_Struct gameDate = new GameDate_Struct(1, Months.JAN, 1973); //Game initial date

    public DBConnector DbInstance = null;

    private Units p_prefered_Unit;

    public Units Prefered_Unit
    {
        get
        {
            return p_prefered_Unit;
        }
    }

    private bool p_firstTimeTutorials;

    public bool FirstTimeTutorials  //TODO: Usar essa informação para o tutorial da primeira corrida
    {
        get
        {
            return p_firstTimeTutorials;
        }
    }


    Saved_Game_Struct[] m_savedGames;

    MainMenu_Controler mainMenu_Controler;
    Tutorial_Controler tutorial_Controler;
    Manager_Controler manager_Controler;


    private void ScreenConfigurations()
    {
        Application.targetFrameRate = 30;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.sleepTimeout = 0;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }


    private void ActiveSceneChanged(Scene _scene, LoadSceneMode _mode)
    {
        switch (_scene.buildIndex)
        {
            //  00.MainMenu
            case 0:
                mainMenu_Controler = FindObjectOfType<MainMenu_Controler>();
                m_savedGames = DbInstance.GetSavedGames();
                mainMenu_Controler.FillUsedSaveSlots(m_savedGames);
                break;

            //  01.Tutorial
            case 1:
                tutorial_Controler = FindObjectOfType<Tutorial_Controler>();
                break;

            //  02.Manager
            case 2:
                manager_Controler = FindObjectOfType<Manager_Controler>();
                break;
        }
    }

    private void VerifyAndSetPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerPrefKeys.FIRST_TIME_TUTORIALS))
        {
            AudioManager.Instance.BGM_Volume = PlayerPrefs.GetInt(PlayerPrefKeys.BGM_VOLUME);
            AudioManager.Instance.SFX_Volume = PlayerPrefs.GetInt(PlayerPrefKeys.SFX_VOLUME);
            p_prefered_Unit = (Units)PlayerPrefs.GetInt(PlayerPrefKeys.PREFERED_UNITS);
            p_firstTimeTutorials = PlayerPrefs.GetInt(PlayerPrefKeys.FIRST_TIME_TUTORIALS) == 0;

            Debug.Log("<b>Game Manager</b> - Player Prefs Loaded!");
        }
        else
        {
            AudioManager.Instance.BGM_Volume = 100;
            AudioManager.Instance.SFX_Volume = 100;
            p_prefered_Unit = Units.METRIC;    //Default value = 0 = metric system;

            Debug.Log("<b>Game Manager</b> - No player prefs found! Setting default values");

            SavePlayerPrefs();
        }
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.BGM_VOLUME, AudioManager.Instance.BGM_Volume);
        PlayerPrefs.SetInt(PlayerPrefKeys.SFX_VOLUME, AudioManager.Instance.SFX_Volume);
        PlayerPrefs.SetInt(PlayerPrefKeys.PREFERED_UNITS, (int)p_prefered_Unit);
        PlayerPrefs.SetInt(PlayerPrefKeys.FIRST_TIME_TUTORIALS, 0);   //treated as a bool; 0 = false, 1 = true;

        Debug.Log("<b>Game Manager</b> - Player Prefs Saved!");
    }

    #endregion


    #region Save_Slot_Controls

    private int p_selectedSaveSlot = -1;

    public int SelectedSaveSlot
    {
        get
        {
            return p_selectedSaveSlot;
        }
        set
        {
            if (value < 1 || value > 3)
                Debug.LogWarning("<b>Game Manager</b> - Tried to select a game slot out of the acceptable range");
            else
                p_selectedSaveSlot = value;
        }
    }


    public void Save()
    {

    }

    /// <summary>
    /// Make sure that the info in _newGame from "game time" and forward are empty!
    /// </summary>
    /// <param name="_newGame"></param>
    /// <param name="_team"></param>
    public void SaveNewGame(ref Saved_Game_Struct _newGame, ref Team_Struct _team)
    {
        double timebefore = Time.realtimeSinceStartupAsDouble;
        DbInstance.SaveNewTeam(ref _team);

        //ORDER: Engineer, Driver, PitCrewLeader, PitCrewMember x4
        string formated_teamIDs = string.Format("{0},{1},{2},{3},{4},{5},{6}",
            _team.engineer.dbId,
            _team.driver.dbId,
            _team.leader.dbId,
            _team.crewMembers[0].dbId,
            _team.crewMembers[1].dbId,
            _team.crewMembers[2].dbId,
            _team.crewMembers[3].dbId);

        _newGame.Team_Members = formated_teamIDs;

        DbInstance.SaveGame(_newGame, p_selectedSaveSlot);

        Debug.Log((Time.realtimeSinceStartupAsDouble - timebefore).ToString());
    }

    public void Load(int _selectedSlot)
    {
        PlayerManager.Instance.SetTeamData(
            m_savedGames[_selectedSlot - 1].Team_Name,
            m_savedGames[_selectedSlot - 1].Car_Number,
            m_savedGames[_selectedSlot - 1].Car_Color.StringToColor(),
            m_savedGames[_selectedSlot - 1].Money);

        string[] date = m_savedGames[_selectedSlot - 1].Game_Date.Split(':');

        gameDate.Week = int.Parse(date[0]);
        gameDate.Month = (Months)int.Parse(date[1]);
        gameDate.Year = int.Parse(date[2]);

        PlayerManager.Instance.LoadTeam(DbInstance.LoadTeamByString(m_savedGames[_selectedSlot - 1].Team_Members));

        LoadScene(2);
    }

    public void DeleteSavedGame(int _selectedSlot)
    {
        DbInstance.DeleteSavedGameByID(_selectedSlot, m_savedGames[_selectedSlot - 1].Team_Members);
    }

    #endregion



    #region Scene Management

    Coroutine Load_AsyncCO = null;

    public void LoadScene(int _sceneIndex)
    {
        SceneManager.LoadScene(_sceneIndex);
    }

    public bool LoadScene_Async(int _sceneIndex, float _haltBeforeSwitching = 0f)
    {
        if (Load_AsyncCO != null)
            return false;

        Load_AsyncCO = StartCoroutine(WaitAndLoad(_sceneIndex, _haltBeforeSwitching));

        return true;
    }

    IEnumerator WaitAndLoad(int _sceneIndex, float _haltBeforeSwitching)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Single);
        load.allowSceneActivation = false;

        float timer = Time.realtimeSinceStartup;

        while (load.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        timer = Time.realtimeSinceStartup - timer;

        if (timer < _haltBeforeSwitching)
            yield return new WaitForSeconds(_haltBeforeSwitching - timer);

        load.allowSceneActivation = true;

        Load_AsyncCO = null;
        yield break;
    }

    #endregion





    public void ChangePreferedUnits()
    {
        p_prefered_Unit = (p_prefered_Unit == Units.METRIC) ? Units.IMPERIAL : Units.METRIC;
    }

    public void Generate_NPC(ref NpcStruct _layout)
    {
        _layout = DbInstance.GenerateNpc_WithDbData();
    }

    public NpcStruct Generate_NPC()
    {
        return DbInstance.GenerateNpc_WithDbData();
    }

    public bool Generate_NPC_InBulk(ref NpcStruct[] _structArray)
    {
        return DbInstance.GenerateNpc_WithDbData(ref _structArray);
    }
}
