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
    [HideInInspector] public NpcDriver Driver { get { return m_driver; } }

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

    public void PrepareDataAndSave()
    {
        Debug.Log("<b>Player Manager</b> - Preparing data to save");

        string dateFormated = string.Format("{0}:{1}:{2}", GameManager.Instance.GameDate.Week, (int)GameManager.Instance.GameDate.Month, GameManager.Instance.GameDate.Year);

        Saved_Game_Struct saveData = new Saved_Game_Struct(m_teamName, m_teamColor.ToString(), m_teamNumber, m_money, dateFormated, "", "");
        Team_Struct teamData = new Team_Struct(m_raceEngineer, m_driver, m_pitCrewLeader, m_pitCrewMembers);

        GameManager.Instance.SaveNewGame(ref saveData, ref teamData);
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

    private bool VerifyLoadedTeam(ref Team_Struct _team)
    {
        if (_team.Engineer == null || _team.Driver == null || _team.CrewLeader == null || _team.CrewMembers == null)
            return false;

        return true;
    }
}
