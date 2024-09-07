using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExtensionMethods;

public class GameManager : MonoBehaviour
{
    #region SIMPLE_SINGLETON
    private static GameManager s_instance;
    public static GameManager Instance { get { return s_instance; } }
    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
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




    #region INITIAL_CONFIGURATIONS

    public GameDate_Struct GameDate = new GameDate_Struct(1, Months.JAN, 1973); //Game initial date

    public DBConnector DbInstance = null;

    private Units m_preferedUnit;

    public Units PreferedUnit
    {
        get
        {
            return m_preferedUnit;
        }
    }

    private bool m_firstTimeTutorials;

    public bool FirstTimeTutorials  //TODO Use this information to prepare first race tutorial
    {
        get
        {
            return m_firstTimeTutorials;
        }
    }


    Saved_Game_Struct[] m_savedGames;

    MainMenu_Controler m_mainMenuControler;
    Tutorial_Controler m_tutorialControler;
    Manager_Controler m_managerControler;


    //TODO: this can be changed by the player in the races
    public int SimulationSpeed = 1;


    private void ScreenConfigurations()
    {
        Application.targetFrameRate = 60;
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
                m_mainMenuControler = FindObjectOfType<MainMenu_Controler>();
                m_savedGames = DbInstance.Get_SavedGames();
                m_mainMenuControler.FillUsedSaveSlots(m_savedGames);
                break;

            //  01.Tutorial
            case 1:
                m_tutorialControler = FindObjectOfType<Tutorial_Controler>();
                break;

            //  02.Manager
            case 2:
                m_managerControler = FindObjectOfType<Manager_Controler>();
                break;
        }
    }

    private void VerifyAndSetPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerPrefKeys.FIRST_TIME_TUTORIALS))
        {
            //FIXME: Sometimes, AudioManager is slower and doesnt set its 'instance'
            AudioManager.Instance.BGM_Volume = PlayerPrefs.GetInt(PlayerPrefKeys.BGM_VOLUME);
            AudioManager.Instance.SFX_Volume = PlayerPrefs.GetInt(PlayerPrefKeys.SFX_VOLUME);
            m_preferedUnit = (Units)PlayerPrefs.GetInt(PlayerPrefKeys.PREFERED_UNITS);
            m_firstTimeTutorials = PlayerPrefs.GetInt(PlayerPrefKeys.FIRST_TIME_TUTORIALS) == 0;

            Debug.Log("<b>Game Manager</b> - Player Prefs Loaded!");
        }
        else
        {
            AudioManager.Instance.BGM_Volume = 100;
            AudioManager.Instance.SFX_Volume = 100;
            m_preferedUnit = Units.METRIC;    //Default value = 0 = metric system;

            Debug.Log("<b>Game Manager</b> - No player prefs found! Setting default values");

            SavePlayerPrefs();
        }
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.BGM_VOLUME, AudioManager.Instance.BGM_Volume);
        PlayerPrefs.SetInt(PlayerPrefKeys.SFX_VOLUME, AudioManager.Instance.SFX_Volume);
        PlayerPrefs.SetInt(PlayerPrefKeys.PREFERED_UNITS, (int)m_preferedUnit);
        PlayerPrefs.SetInt(PlayerPrefKeys.FIRST_TIME_TUTORIALS, 0);   //treated as a bool; 0 = false, 1 = true;

        Debug.Log("<b>Game Manager</b> - Player Prefs Saved!");
    }

    #endregion //INITIAL_CONFIGURATIONS


    #region SAVE_SLOT_CONTROL

    private int m_selectedSaveSlot = -1;

    public int SelectedSaveSlot
    {
        get
        {
            return m_selectedSaveSlot;
        }
        set
        {
            if (value < 1 || value > 3)
                Debug.LogWarning("<b>Game Manager</b> - Tried to select a game slot out of the acceptable range");
            else
                m_selectedSaveSlot = value;
        }
    }


    public void SaveGame()
    {
        PlayerManager.Instance.PrepareSaveData(out Saved_Game_Struct _saveData, out Team_Struct _teamData);

        DbInstance.SaveGame(_saveData, m_selectedSaveSlot);
        DbInstance.SaveTeam(_teamData);
    }

    /// <summary>
    /// Make sure that the info in _newGame from "game time" and forward are empty!
    /// </summary>
    /// <param name="_newGame"></param>
    /// <param name="_team"></param>
    public void SaveNewGame(ref Saved_Game_Struct _newGame, ref Team_Struct _team)
    {
        DbInstance.SaveNewTeam(ref _team);

        _newGame.Team_Members = _team.FormatTeamAsIds();

        DbInstance.SaveGame(_newGame, m_selectedSaveSlot);
    }

    public void LoadGame(int _selectedSlot)
    {
        PlayerManager.Instance.SetTeamData(
            m_savedGames[_selectedSlot - 1].Team_Name,
            m_savedGames[_selectedSlot - 1].Car_Number,
            m_savedGames[_selectedSlot - 1].Car_Color.StringToColor(),
            m_savedGames[_selectedSlot - 1].Money);

        string[] date = m_savedGames[_selectedSlot - 1].Game_Date.Split(':');

        GameDate.Week = int.Parse(date[0]);
        GameDate.Month = (Months)int.Parse(date[1]);
        GameDate.Year = int.Parse(date[2]);

        PlayerManager.Instance.LoadTeam(DbInstance.LoadTeamByString(m_savedGames[_selectedSlot - 1].Team_Members));

        LoadScene_Async((int)SceneCodex.MANAGER);
    }

    public void DeleteSavedGame(int _selectedSlot)
    {
        DbInstance.DeleteSavedGameByID(_selectedSlot, m_savedGames[_selectedSlot - 1].Team_Members);
    }

    #endregion //SAVE_SLOT_CONTROL



    #region SCENE_MANAGEMENT

    [SerializeField] Animator sceneSwitcher;

    Coroutine Load_AsyncCO = null;

    public void LoadScene(int _sceneIndex)
    {
        SceneManager.LoadScene(_sceneIndex);
    }

    public void LoadScene(SceneCodex _sceneName)
    {
        SceneManager.LoadScene((int)_sceneName);
    }

    public bool LoadScene_Async(int _sceneIndex, float _haltBeforeSwitching = 1f)
    {
        if (Load_AsyncCO != null)
            return false;

        Load_AsyncCO = StartCoroutine(WaitAndLoad(_sceneIndex, _haltBeforeSwitching));

        return true;
    }

    IEnumerator WaitAndLoad(int _sceneIndex, float _haltBeforeSwitching)
    {
        sceneSwitcher.SetTrigger("Out");

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

        sceneSwitcher.SetTrigger("In");

        Load_AsyncCO = null;
        yield break;
    }

    #endregion //SCENE_MANAGEMENT





    public void ChangePreferedUnits()
    {
        m_preferedUnit = (m_preferedUnit == Units.METRIC) ? Units.IMPERIAL : Units.METRIC;
    }

    public void NpcGenerate(ref NpcStruct _layout)
    {
        _layout = DbInstance.GenerateNpc_WithDbData();
    }

    public NpcStruct NpcGenerate()
    {
        return DbInstance.GenerateNpc_WithDbData();
    }

    public bool NpcGenerateInBulk(ref NpcStruct[] _structArray)
    {
        return DbInstance.GenerateNpc_WithDbData(ref _structArray);
    }
}
