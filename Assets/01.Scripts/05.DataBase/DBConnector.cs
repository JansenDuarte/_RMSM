using ExtensionMethods;
using Mono.Data.Sqlite;
using System.Data;
using System.Globalization;
using UnityEngine;


public class DBConnector : MonoBehaviour
{
    #region SIMPLE_SINGLETON
    private static DBConnector m_instance;
    public static DBConnector Instance { get { return m_instance; } }
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
    #endregion // SIMPLE_SINGLETON


    #region CONSTANTS

    private const int NAMES_SIZE = 1001;
    private const int COUNTRIES_SIZE = 196;

    private const int MIN_GEN_NPC_AGE = 18;
    private const int MAX_GEN_NPC_AGE = 30;

    #endregion // CONSTANTS


    #region DATABASE_METHODS

    private IDbConnection _connection = null;
    private IDbCommand _command = null;
    private IDataReader _reader = null;


    #region General DataBase Connection

    private bool Connect()
    {
        try
        {
            _connection = new SqliteConnection(CommandCodex.GAME_URL);
            _connection.Open();
            _command = _connection.CreateCommand();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public Saved_Game_Struct[] Get_SavedGames()
    {
        if (Connect() == false)
        {
            Debug.Log("<b>DB Connector</b> - ERROR! Database connection could not be reached!");
            return null;
        }

        Initiate_OpTimer();

        Saved_Game_Struct[] savedGames = new Saved_Game_Struct[3];

        _command.CommandText = CommandCodex.SELECT_ALL_SAVED_GAMES;

        _reader = _command.ExecuteReader();

        while (_reader.Read())
        {
            savedGames[_reader.GetInt32(0) - 1] = new Saved_Game_Struct(
                _reader.GetString(1),    //Team Name
                _reader.GetString(2),    //Car Color
                _reader.GetInt32(3),     //Car Number
                _reader.GetInt32(4),     //Money
                _reader.GetString(5),    //Game Time
                _reader.GetString(6),    //Team Members
                _reader.GetString(7)     //Current Country
                );
        }

        _reader.Close();
        Print_OpTimer("Get_SavedGames()");
        CloseConnection();

        return savedGames;
    }

    public bool SaveNewTeam(ref Team_Struct _team)
    {
        if (Connect() == false)
        {
            Debug.Log("<b>DB Connector</b> - ERROR! Database connection could not be reached!");
            return false;
        }

        Initiate_OpTimer();




        //Enginieer save

        _command.CommandText = string.Format(CommandCodex.INSERT_TEAM_MEMBER_ENGINEER,
            _team.Engineer.name.ToUpper(),
            _team.Engineer.sex,
            _team.Engineer.age,
            _team.Engineer.country,
            _team.Engineer.level,
            _team.Engineer.xp,
            _team.Engineer.moralValue,
            _team.Engineer.potencial,
            _team.Engineer.skills[0].VALUE,
            _team.Engineer.skills[1].VALUE,
            _team.Engineer.skills[2].VALUE,
            _team.Engineer.skills[3].VALUE);

        _command.ExecuteNonQuery();

        //!!Enginieer save!!

        //Driver save

        _command.CommandText = string.Format(CommandCodex.INSERT_TEAM_MEMBER_DRIVER,
            _team.Driver.name.ToUpper(),
            _team.Driver.sex,
            _team.Driver.age,
            _team.Driver.country,
            _team.Driver.level,
            _team.Driver.xp,
            _team.Driver.moralValue,
            _team.Driver.potencial,
            _team.Driver.skills[0].VALUE,
            _team.Driver.skills[1].VALUE,
            _team.Driver.skills[2].VALUE,
            _team.Driver.skills[3].VALUE);

        _command.ExecuteNonQuery();

        //!!Driver save!!

        //Pit Crew Leader save

        _command.CommandText = string.Format(CommandCodex.INSERT_TEAM_MEMBER_LEADER,
            _team.CrewLeader.name.ToUpper(),
            _team.CrewLeader.sex,
            _team.CrewLeader.age,
            _team.CrewLeader.country,
            _team.CrewLeader.level,
            _team.CrewLeader.xp,
            _team.CrewLeader.moralValue,
            _team.CrewLeader.potencial,
            _team.CrewLeader.skills[0].VALUE,
            _team.CrewLeader.skills[1].VALUE,
            _team.CrewLeader.skills[2].VALUE,
            _team.CrewLeader.skills[3].VALUE);

        _command.ExecuteNonQuery();

        //!!Pit Crew Leader save!!

        //Pit Crew save
        for (int i = 0; i < _team.CrewMembers.Length; i++)
        {
            _command.CommandText = string.Format(CommandCodex.INSERT_TEAM_MEMBER_CREW,
                _team.CrewMembers[i].name.ToUpper(),
                _team.CrewMembers[i].sex,
                _team.CrewMembers[i].age,
                _team.CrewMembers[i].country,
                _team.CrewMembers[i].level,
                _team.CrewMembers[i].xp,
                _team.CrewMembers[i].moralValue,
                _team.CrewMembers[i].potencial,
                _team.CrewMembers[i].skills[0].VALUE,
                _team.CrewMembers[i].skills[1].VALUE,
                _team.CrewMembers[i].skills[2].VALUE,
                _team.CrewMembers[i].skills[3].VALUE);

            _command.ExecuteNonQuery();

        }
        //!!Pit Crew save!!

        int lastInsertedID;
        _command.CommandText = CommandCodex.SELECT_LAST_ROWID;
        lastInsertedID = int.Parse(_command.ExecuteScalar().ToString());

        _team.Engineer.dbId = lastInsertedID - 6;
        _team.Driver.dbId = lastInsertedID - 5;
        _team.CrewLeader.dbId = lastInsertedID - 4;
        _team.CrewMembers[0].dbId = lastInsertedID - 3;
        _team.CrewMembers[1].dbId = lastInsertedID - 2;
        _team.CrewMembers[2].dbId = lastInsertedID - 1;
        _team.CrewMembers[3].dbId = lastInsertedID;

        Print_OpTimer("SaveNewTeam()");

        Debug.Log("<b>DB Connector</b> - New Team Saved Successfuly!");

        CloseConnection();

        return true;
    }


    public bool SaveGame(Saved_Game_Struct _currentGame, int _saveSlot)
    {
        if (Connect() == false)
        {
            Debug.Log("<b>DB Connector</b> - ERROR! Database connection could not be reached!");
            return false;
        }

        Initiate_OpTimer();

        _command.CommandText = string.Format(CommandCodex.UPDATE_SAVED_GAME_WHERE_ID,
            _currentGame.Team_Name,
            _currentGame.Car_Color,
            _currentGame.Car_Number,
            _currentGame.Money,
            _currentGame.Game_Date,
            _currentGame.Team_Members,
            _currentGame.Current_Country,
            _saveSlot);

        _command.ExecuteNonQuery();

        Print_OpTimer("SaveGame()");

        CloseConnection();

        return true;
    }

    public bool SaveTeam(Team_Struct _team)
    {
        if (Connect() == false)
        {
            Debug.Log("<b>DB Connector</b> - ERROR! Database connection could not be reached!");
            return false;
        }

        Initiate_OpTimer();

        NpcLayout data;

        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0: //Driver
                    data = PlayerManager.Instance.Driver;
                    break;
                case 1: //Engineer
                    data = PlayerManager.Instance.Engineer;
                    break;
                case 2: //Pit crew leader
                    data = PlayerManager.Instance.PitLeader;
                    break;

                //Pit crew members
                case 3:
                    data = PlayerManager.Instance.PitMembers[0];
                    break;
                case 4:
                    data = PlayerManager.Instance.PitMembers[1];
                    break;
                case 5:
                    data = PlayerManager.Instance.PitMembers[2];
                    break;
                case 6:
                    data = PlayerManager.Instance.PitMembers[3];
                    break;
                default:
                    return false;
            }

            _command.CommandText = string.Format(CommandCodex.UPDATE_TEAM_MEMBER_WHERE_ID,
            data.level,
            data.xp,
            data.moralValue,
            data.potencial,
            data.skills[0].VALUE,
            data.skills[1].VALUE,
            data.skills[2].VALUE,
            data.skills[3].VALUE,
            data.dbId);

            _command.ExecuteNonQuery();
        }

        Print_OpTimer("SaveTeam()");

        CloseConnection();

        return true;
    }

    public Team_Struct LoadTeamByString(string _teamMembers)
    {
        Team_Struct ts = new();

        if (Connect() == false)
            return ts;

        Initiate_OpTimer();

        _command.CommandText = string.Format(CommandCodex.SELECT_ALL_TEAM_MEMBERS, _teamMembers);

        _reader = _command.ExecuteReader();
        int readIndex = 0;
        int crewIndex = 0;

        ts.CrewMembers = new NpcPitCrewMember[4];

        while (_reader.Read())
        {
            switch (readIndex)
            {
                //Race Enineer
                case 0:
                    ts.Engineer = new();
                    ts.Engineer.dbId = _reader.GetInt32(0);
                    ts.Engineer.name = _reader.GetString(1);
                    ts.Engineer.sex = _reader.GetString(2);
                    ts.Engineer.age = _reader.GetInt32(3);
                    ts.Engineer.country = _reader.GetString(4);
                    ts.Engineer.level = _reader.GetInt32(5);
                    ts.Engineer.xp = _reader.GetInt32(6);
                    ts.Engineer.moralValue = _reader.GetInt32(7);
                    ts.Engineer.potencial = _reader.GetInt32(8);
                    ts.Engineer.skills[0].VALUE = _reader.GetInt32(9);
                    ts.Engineer.skills[1].VALUE = _reader.GetInt32(10);
                    ts.Engineer.skills[2].VALUE = _reader.GetInt32(11);
                    ts.Engineer.skills[3].VALUE = _reader.GetInt32(12);
                    break;
                //Driver
                case 1:
                    ts.Driver = new();
                    ts.Driver.dbId = _reader.GetInt32(0);
                    ts.Driver.name = _reader.GetString(1);
                    ts.Driver.sex = _reader.GetString(2);
                    ts.Driver.age = _reader.GetInt32(3);
                    ts.Driver.country = _reader.GetString(4);
                    ts.Driver.level = _reader.GetInt32(5);
                    ts.Driver.xp = _reader.GetInt32(6);
                    ts.Driver.moralValue = _reader.GetInt32(7);
                    ts.Driver.potencial = _reader.GetInt32(8);
                    ts.Driver.skills[0].VALUE = _reader.GetInt32(9);
                    ts.Driver.skills[1].VALUE = _reader.GetInt32(10);
                    ts.Driver.skills[2].VALUE = _reader.GetInt32(11);
                    ts.Driver.skills[3].VALUE = _reader.GetInt32(12);
                    break;
                //Pit Crew Leader
                case 2:
                    ts.CrewLeader = new();
                    ts.CrewLeader.dbId = _reader.GetInt32(0);
                    ts.CrewLeader.name = _reader.GetString(1);
                    ts.CrewLeader.sex = _reader.GetString(2);
                    ts.CrewLeader.age = _reader.GetInt32(3);
                    ts.CrewLeader.country = _reader.GetString(4);
                    ts.CrewLeader.level = _reader.GetInt32(5);
                    ts.CrewLeader.xp = _reader.GetInt32(6);
                    ts.CrewLeader.moralValue = _reader.GetInt32(7);
                    ts.CrewLeader.potencial = _reader.GetInt32(8);
                    ts.CrewLeader.skills[0].VALUE = _reader.GetInt32(9);
                    ts.CrewLeader.skills[1].VALUE = _reader.GetInt32(10);
                    ts.CrewLeader.skills[2].VALUE = _reader.GetInt32(11);
                    ts.CrewLeader.skills[3].VALUE = _reader.GetInt32(12);
                    break;
                //Pit Crew Members
                default:
                    ts.CrewMembers[crewIndex] = new();
                    ts.CrewMembers[crewIndex].dbId = _reader.GetInt32(0);
                    ts.CrewMembers[crewIndex].name = _reader.GetString(1);
                    ts.CrewMembers[crewIndex].sex = _reader.GetString(2);
                    ts.CrewMembers[crewIndex].age = _reader.GetInt32(3);
                    ts.CrewMembers[crewIndex].country = _reader.GetString(4);
                    ts.CrewMembers[crewIndex].level = _reader.GetInt32(5);
                    ts.CrewMembers[crewIndex].xp = _reader.GetInt32(6);
                    ts.CrewMembers[crewIndex].moralValue = _reader.GetInt32(7);
                    ts.CrewMembers[crewIndex].potencial = _reader.GetInt32(8);
                    ts.CrewMembers[crewIndex].skills[0].VALUE = _reader.GetInt32(9);
                    ts.CrewMembers[crewIndex].skills[1].VALUE = _reader.GetInt32(10);
                    ts.CrewMembers[crewIndex].skills[2].VALUE = _reader.GetInt32(11);
                    ts.CrewMembers[crewIndex].skills[3].VALUE = _reader.GetInt32(12);
                    crewIndex++;
                    break;
            }

            readIndex++;
        }

        Print_OpTimer("LoadTeamByString()");
        _reader.Close();
        CloseConnection();

        return ts;
    }


    public bool DeleteSavedGameByID(int _id, string _teamLinkedWithSave)
    {
        if (Connect() == false)
        {
            Debug.Log("<b>DB Connector</b> - ERROR! Database connection could not be reached!");
            return false;
        }

        Initiate_OpTimer();

        //Clear Saved_Game row && delete team associated with this Saved_Game
        _command.CommandText = string.Format(CommandCodex.DELETE_SAVED_GAME, _id, _teamLinkedWithSave);

        _command.ExecuteNonQuery();

        Print_OpTimer("DeleteSavedGameByID()");

        CloseConnection();

        return true;
    }

    #endregion

    #region NPC Data Methods

    private bool Connect_NpcDataDb()
    {
        try
        {
            _connection = new SqliteConnection(CommandCodex.NPC_URL);
            _connection.Open();
            _command = _connection.CreateCommand();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }


    /// <summary>
    /// Generates 1 NpcStruct with database contents
    /// </summary>
    /// <returns></returns>
    public NpcStruct GenerateNpc_WithDbData()
    {
        NpcStruct layout = new NpcStruct();

        if (Connect_NpcDataDb() == false)
        {
            Debug.LogWarning("NPC data couldn't be reached...");
            return layout;
        }
        else
        {
            int rng = Random.Range(1, NAMES_SIZE);

            _command.CommandText = CommandCodex.SELECT_NAMES_WHERE_ID + rng;

            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                layout.name = _reader.GetString(1);

                if (_reader.GetString(2) == "mf")
                {
                    int sex_rng = Random.Range(0, 2);

                    if (sex_rng == 0)
                        layout.sex = "m";
                    else
                        layout.sex = "f";
                }
                else
                    layout.sex = _reader.GetString(2);
            }

            _reader.Close();

            rng = Random.Range(1, NAMES_SIZE);

            _command.CommandText = CommandCodex.SELECT_NAMES_WHERE_ID + rng;

            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                layout.name = layout.name + " " + _reader.GetString(3);
            }

            _reader.Close();

            rng = Random.Range(1, COUNTRIES_SIZE);

            _command.CommandText = CommandCodex.SELECT_COUNTRY_WHERE_ID + rng;

            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                layout.country = _reader.GetString(1);
            }

            _reader.Close();

            layout.age = Random.Range(MIN_GEN_NPC_AGE, MAX_GEN_NPC_AGE);

            CloseConnection();

            return layout;
        }

    }

    /// <summary>
    /// Generates enough NpcStructs to populate the array with data
    /// </summary>
    /// <param name="_structArray"></param>
    /// <returns></returns>
    public bool GenerateNpc_WithDbData(ref NpcStruct[] _structArray)
    {
        if (_structArray.Length == 0)
            return false;

        if (Connect_NpcDataDb() == false)
        {
            Debug.LogWarning("NPC data couldn't be reached...");
            return false;
        }
        else
        {
            for (int i = 0; i < _structArray.Length; i++)
            {
                int rng = Random.Range(1, NAMES_SIZE);

                _command.CommandText = CommandCodex.SELECT_NAMES_WHERE_ID + rng;

                _reader = _command.ExecuteReader();

                while (_reader.Read())
                {
                    _structArray[i].name = _reader.GetString(1);

                    if (_reader.GetString(2) != "mf")
                        _structArray[i].sex = _reader.GetString(2);
                    else
                    {
                        if (Random.Range(0, 2) == 0)
                            _structArray[i].sex = "m";
                        else
                            _structArray[i].sex = "f";
                    }
                }

                _reader.Close();

                rng = Random.Range(1, NAMES_SIZE);

                _command.CommandText = CommandCodex.SELECT_NAMES_WHERE_ID + rng;

                _reader = _command.ExecuteReader();

                while (_reader.Read())
                {
                    _structArray[i].name = _structArray[i].name + " " + _reader.GetString(3);
                }

                _reader.Close();

                rng = Random.Range(1, COUNTRIES_SIZE);

                _command.CommandText = CommandCodex.SELECT_COUNTRY_WHERE_ID + rng;

                _reader = _command.ExecuteReader();

                while (_reader.Read())
                {
                    _structArray[i].country = _reader.GetString(1);
                }

                _reader.Close();

                //FIXME: maybe this needs to be done after the fact
                _structArray[i].age = Random.Range(MIN_GEN_NPC_AGE, MAX_GEN_NPC_AGE);
            }

            CloseConnection();

            return true;
        }

    }

    #endregion // DATABASE_METHODS



    #region TRACK_DB_METHODS

    private bool Connect_TrackDb()
    {
        try
        {
            _connection = new SqliteConnection(CommandCodex.TRACK_URL);
            _connection.Open();
            _command = _connection.CreateCommand();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public bool Load_Track(out Track _track, int _index = -1)
    {
        _track = new Track();

        if (Connect_TrackDb() == false)
        {
            Debug.LogWarning("Track data couldn't be reached...");
            return false;
        }

        Initiate_OpTimer();

        _command.CommandText = CommandCodex.SELECT_LAST_ROWID;
        int size = int.Parse(_command.ExecuteScalar().ToString());

        int idx = (_index == -1) ? Random.Range(1, size) : _index;

        _command.CommandText = CommandCodex.SELECT_TRACKS_WHERE_ID + idx;

        _reader = _command.ExecuteReader();

        while (_reader.Read())
        {
            _track.trackName = _reader.GetString(1);
            _track.country = _reader.GetString(2);
            _track.length = float.Parse(_reader.GetString(3), CultureInfo.InvariantCulture);
            _track.laps = _reader.GetInt32(4);
            _track.type = (TrackType)_reader.GetInt32(5);
            _track.curve = _reader.GetString(6).Convert_StringToTrack();
        }

        if (_track.type == TrackType.CIRCUIT || _track.type == TrackType.OVAL)
            _track.curve.close = true;

        Print_OpTimer("Load_Track()");

        _reader.Close();
        CloseConnection();

        return true;
    }

    #endregion // TRACK_DB_METHODS



    private void CloseConnection()
    {
        _connection?.Close();
        _connection = null;

        _command?.Dispose();
        _command = null;
    }

    #endregion



    #region UTILITIES

    double m_opTimer;

    void Initiate_OpTimer() { m_opTimer = Time.realtimeSinceStartupAsDouble; }

    void Print_OpTimer(string _opExecuted = "")
    {
        m_opTimer = Time.realtimeSinceStartupAsDouble - m_opTimer;

        Debug.Log(string.Format("<b>DataBase Connector</b> - Operation {0} took {1} ms", _opExecuted, m_opTimer * 1000d));
    }

    #endregion // UTILITIES


    private static class CommandCodex
    {
#if UNITY_EDITOR
        public static readonly string GAME_URL = "URI=file:" + Application.streamingAssetsPath + "/99.InternalDataBase/" + "GameDataBase.db";
        public static readonly string NPC_URL = "URI=file:" + Application.streamingAssetsPath + "/99.InternalDataBase/" + "NpcData.db";
        public static readonly string TRACK_URL = "URI=file:" + Application.streamingAssetsPath + "/99.InternalDataBase/" + "Tracks.db";
#else
        public static readonly string GAME_URL = "URI=file:" + Application.persistentDataPath + "/99.InternalDataBase/" + "GameDataBase.db";
        public static readonly string NPC_URL = "URI=file:" + Application.persistentDataPath + "/99.InternalDataBase/" + "NpcData.db";
        public static readonly string TRACK_URL = "URI=file:" + Application.persistentDataPath + "/99.InternalDataBase/" + "Tracks.db";
#endif
        public const string SELECT_LAST_ROWID = "SELECT last_insert_rowid();";

        public const string SELECT_ALL_SAVED_GAMES = "SELECT * FROM Saved_Game";
        public const string INSERT_TEAM_MEMBER_ENGINEER = "INSERT INTO Team_Members " +
        "(Name, Sex, Age, Country, Level, Experience, Moral, Potential, Skill_1, Skill_2, Skill_3, Skill_4) " +
        "VALUES (\"{0}\", \"{1}\", {2}, \"{3}\", {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";
        public const string INSERT_TEAM_MEMBER_DRIVER = "INSERT INTO Team_Members " +
        "(Name, Sex, Age, Country, Level, Experience, Moral, Potential, Skill_1, Skill_2, Skill_3, Skill_4) " +
        "VALUES (\"{0}\", \"{1}\", {2}, \"{3}\", {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";
        public const string INSERT_TEAM_MEMBER_LEADER = "INSERT INTO Team_Members " +
        "(Name, Sex, Age, Country, Level, Experience, Moral, Potential, Skill_1, Skill_2, Skill_3, Skill_4) " +
        "VALUES (\"{0}\", \"{1}\", {2}, \"{3}\", {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";
        public const string INSERT_TEAM_MEMBER_CREW = "INSERT INTO Team_Members " +
        "(Name, Sex, Age, Country, Level, Experience, Moral, Potential, Skill_1, Skill_2, Skill_3, Skill_4) " +
        "VALUES (\"{0}\", \"{1}\", {2}, \"{3}\", {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";
        public const string UPDATE_SAVED_GAME_WHERE_ID = "UPDATE Saved_Game SET Team_Name = " +
        "\"{0}\", Car_Color = \"{1}\", Car_Number = {2}, Money = {3}, Game_Time = \"{4}\", Team_Members = \"{5}\", Current_Country = \"{6}\" " +
        "WHERE ID = {7}";
        public const string UPDATE_TEAM_MEMBER_WHERE_ID = "UPDATE Team_Members SET Level = " +
        "{0}, Experience = {1}, Moral = {2}, Potential = {3}, Skill_1 = {4}, Skill_2 = {5}, Skill_3 = {6} , Skill_4 = {7} " +
        "WHERE ID = {8}";
        public const string SELECT_ALL_TEAM_MEMBERS = "SELECT * FROM Team_Members WHERE ID in ({0})";
        public const string DELETE_SAVED_GAME = "UPDATE Saved_Game SET Team_Name = " +
        "\"\", Car_Color = \"\", Car_Number = 0, Money = 0, Game_Time = \"\", Team_Members = \"\", Current_Country = \"\" " +
        "WHERE ID = {0};" +
        "\nDELETE FROM Team_Members WHERE ID in ({1});";
        public const string SELECT_NAMES_WHERE_ID = "SELECT * FROM Names WHERE ID=";
        public const string SELECT_COUNTRY_WHERE_ID = "SELECT * FROM CountriesOfTheWorld WHERE ID=";
        public const string SELECT_TRACKS_WHERE_ID = "SELECT * FROM Tracks WHERE ID = ";
    }
}