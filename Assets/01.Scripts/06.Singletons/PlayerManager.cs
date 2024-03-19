using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region SIMPLE_SINGLETON
    private static PlayerManager m_instance;
    public static PlayerManager Instance { get { return m_instance; } }
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
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
    [HideInInspector] public string TeamName { get { return m_teamName; } }
    [SerializeField] int m_teamNumber;
    [HideInInspector] public int TeamNumber { get { return m_teamNumber; } }
    [SerializeField] Color m_teamColor;
    [HideInInspector] public Color TeamColor { get { return m_teamColor; } }
    [SerializeField] int m_money;
    [HideInInspector] public int Money { get { return m_money; } set { m_money = value; } }   //FIXME: this could be chaged to be more safe

    [Header("Team members")]
    [SerializeField] NpcRaceEngineer raceEngineer;
    [SerializeField] NpcDriver driver;
    [HideInInspector] public NpcDriver Driver { get { return driver; } }
    [SerializeField] NpcPitCrewLeader pitCrewLeader;
    [SerializeField] NpcPitCrewMember[] pitCrewMembers;

    public void SetTeamData(string _name, int _number, Color _color, int _money)
    {
        m_teamName = _name;
        m_teamNumber = _number;
        m_teamColor = _color;
        m_money = _money;
    }

    public void ChangeRaceEngineer(NpcRaceEngineer _newEngineer) { raceEngineer = _newEngineer; }

    public void ChangeDriver(NpcDriver _newDriver) { driver = _newDriver; }

    public void ChangePitCrewLeader(NpcPitCrewLeader _newLeader, bool _hasOwnPitTeam = true)
    {
        pitCrewLeader = _newLeader;

        if (_hasOwnPitTeam)
        {
            pitCrewMembers = pitCrewLeader.pitCrew;
        }
        else
        {
            if (pitCrewMembers != null)
                pitCrewLeader.pitCrew = pitCrewMembers;
            else
                Debug.LogWarning("<b>Player Manager</> - Player has no pit crew members, and pit crew leader has no team!");
        }
    }

    public void PrepareDataAndSave()
    {
        Debug.Log("<b>Player Manager</b> - Preparing data to save");

        string dateFormated = string.Format("{0}:{1}:{2}", GameManager.Instance.gameDate.Week, (int)GameManager.Instance.gameDate.Month, GameManager.Instance.gameDate.Year);

        Saved_Game_Struct saveData = new Saved_Game_Struct(m_teamName, m_teamColor.ToString(), m_teamNumber, m_money, dateFormated, "", "");
        Team_Struct teamData = new Team_Struct(raceEngineer, driver, pitCrewLeader, pitCrewMembers);

        GameManager.Instance.SaveNewGame(ref saveData, ref teamData);
    }

    public void LoadTeam(Team_Struct _team)
    {
        raceEngineer = _team.engineer;
        driver = _team.driver;
        pitCrewMembers = _team.crewMembers;
        pitCrewLeader = _team.leader;
        if (pitCrewLeader.pitCrew != null)
            pitCrewLeader.pitCrew = pitCrewMembers;
        else
        {
            //TODO bark an error! save was corrupted!
        }
    }
}
