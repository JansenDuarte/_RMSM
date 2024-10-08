using ExtensionMethods;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region SIMPLE_SINGLETON
    private static PlayerManager s_instance;
    public static PlayerManager Instance { get { return s_instance; } }
    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Team initial data")]
    [SerializeField] string m_teamName;
    [SerializeField] int m_teamNumber;
    [SerializeField] Color m_teamColor;
    [SerializeField] int m_money;
    [HideInInspector] public string TeamName { get { return m_teamName; } }
    [HideInInspector] public int TeamNumber { get { return m_teamNumber; } }
    [HideInInspector] public Color TeamColor { get { return m_teamColor; } }
    [HideInInspector] public int Money { get { return m_money; } set { m_money = value; } }

    [Header("Team members")]
    [SerializeField] NpcRaceEngineer m_raceEngineer;
    [SerializeField] NpcDriver m_driver;
    [SerializeField] NpcPitCrewLeader m_pitCrewLeader;
    [SerializeField] NpcPitCrewMember[] m_pitCrewMembers;

    [HideInInspector] public NpcRaceEngineer Engineer { get { return m_raceEngineer; } }
    [HideInInspector] public NpcDriver Driver { get { return m_driver; } }
    [HideInInspector] public NpcPitCrewLeader PitLeader { get { return m_pitCrewLeader; } }
    [HideInInspector] public NpcPitCrewMember[] PitMembers { get { return m_pitCrewMembers; } }

    public void SetTeamData(string _name, int _number, Color _color, int _money)
    {
        m_teamName = _name;
        m_teamNumber = _number;
        m_teamColor = _color;
        m_money = _money;
    }

    public void ChangeRaceEngineer(NpcRaceEngineer _newEngineer) { m_raceEngineer = _newEngineer; }

    public void ChangeDriver(NpcDriver _newDriver) { m_driver = _newDriver; }

    public void ChangePitCrewLeader(NpcPitCrewLeader _newLeader, bool _hasOwnPitTeam = true)
    {
        m_pitCrewLeader = _newLeader;

        if (_hasOwnPitTeam)
        {
            m_pitCrewMembers = m_pitCrewLeader.pitCrew;
        }
        else
        {
            if (m_pitCrewMembers != null)
                m_pitCrewLeader.pitCrew = m_pitCrewMembers;
            else
                Debug.LogWarning("<b>Player Manager</> - Player has no pit crew members, and pit crew leader has no team!");
        }
    }

    public void PrepareSaveData(out Saved_Game_Struct _saveData, out Team_Struct _teamData)
    {
        Debug.Log("<b>Player Manager</b> - Preparing data to save");

        string dateFormated = string.Format("{0}:{1}:{2}", GameManager.Instance.GameDate.Week, (int)GameManager.Instance.GameDate.Month, GameManager.Instance.GameDate.Year);

        _saveData = new Saved_Game_Struct(m_teamName, m_teamColor.ToString(), m_teamNumber, m_money, dateFormated, "", "");
        _teamData = new Team_Struct(m_raceEngineer, m_driver, m_pitCrewLeader, m_pitCrewMembers);

        if (m_pitCrewMembers[0].dbId < 0)   //This means that the NPC has been generated now, and its not present in the db
            return;

        _saveData.Team_Members = _teamData.FormatTeamAsIds();
    }

    public void LoadTeam(Team_Struct _team)
    {
        if (VerifyLoadedTeam(ref _team) == false)
        {
            Debug.LogWarning("<b>Player Manager</> - Team could not be loaded correctly! Save file is probably corrupted...");
            return;
        }

        m_raceEngineer = _team.Engineer;
        m_driver = _team.Driver;
        m_pitCrewMembers = _team.CrewMembers;
        m_pitCrewLeader = _team.CrewLeader;
        if (m_pitCrewLeader.pitCrew != null)
            m_pitCrewLeader.pitCrew = m_pitCrewMembers;
    }


    //TODO need to figure out how to give xp and check for level ups later
    //TODO  these two need to be fixed in some place. They were moved here to allow the animation of the UI
    public int xpGained;
    public int moneyGained;
    public void GiveTeamXp()
    {
        m_driver.GainXP(xpGained);
        m_raceEngineer.GainXP(xpGained);
        m_pitCrewLeader.GainXP(xpGained);
        foreach (NpcPitCrewMember pcm in m_pitCrewMembers)
        {
            pcm.GainXP(xpGained);
        }

        xpGained = 0;
    }

    public void GainMoney()
    {
        Money += moneyGained;
        moneyGained = 0;
    }


    private bool VerifyLoadedTeam(ref Team_Struct _team)
    {
        if (_team.Engineer == null || _team.Driver == null || _team.CrewLeader == null || _team.CrewMembers == null)
            return false;

        return true;
    }
}
