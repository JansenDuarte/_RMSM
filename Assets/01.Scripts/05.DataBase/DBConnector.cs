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
            PrepareDataBaseInfo();
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

    [SerializeField]
    [Tooltip("Name of the general database")]
    private string GameDb_url;

    [SerializeField]
    [Tooltip("Name of the NPC database with names, surenames and nationalities")]
    private string NpcDataDb_url;

    [SerializeField]
    [Tooltip("Database for the tracks")]
    private string TracksDb_url;

    private IDbConnection _connection = null;

    private IDbCommand _command = null;







    private void PrepareDataBaseInfo()
    {
        GameDb_url = "URI=file:" + Application.dataPath + "/99.InternalDataBase/" + GameDb_url;
        NpcDataDb_url = "URI=file:" + Application.dataPath + "/99.InternalDataBase/" + NpcDataDb_url;
        TracksDb_url = "URI=file:" + Application.dataPath + "/99.InternalDataBase/" + TracksDb_url;
    }







    #region General DataBase Connection

    private bool Connect()
    {
        _connection = new SqliteConnection(GameDb_url);

        if (_connection != null)
        {
            _connection.Open();
            return true;
        }
        else
            return false;
    }

    public Saved_Game_Struct[] Get_SavedGames()
    {
        if (Connect() == false)
        {
            Debug.Log("<b>DB Connector</b> - ERROR! Database connection could not be reached!");
            return null;
        }

        Initiate_OpTimer();

        Saved_Game_Struct[] _returnValue = new Saved_Game_Struct[3];

        _command = _connection.CreateCommand();

        _command.CommandText = "SELECT * FROM Saved_Game";

        IDataReader reader = _command.ExecuteReader();

        while (reader.Read())
        {
            _returnValue[reader.GetInt32(0) - 1] = new Saved_Game_Struct(
                reader.GetString(1),    //Team Name
                reader.GetString(2),    //Car Color
                reader.GetInt32(3),     //Car Number
                reader.GetInt32(4),     //Money
                reader.GetString(5),    //Game Time
                reader.GetString(6),    //Team Members
                reader.GetString(7)     //Current Country
                );
        }

        reader.Close();
        Print_OpTimer("Get_SavedGames()");
        CloseConnection();

        return _returnValue;
    }

    public bool SaveNewTeam(ref Team_Struct _team)
    {
        if (Connect() == false)
        {
            Debug.Log("<b>DB Connector</b> - ERROR! Database connection could not be reached!");
            return false;
        }

        Initiate_OpTimer();

        _command = _connection.CreateCommand();



        //Enginieer save

        _command.CommandText = string.Format("INSERT INTO Team_Members (Name, Sex, Age, Country, Level, Experience, Moral, Potential, Skill_1, Skill_2, Skill_3, Skill_4) " +
            "VALUES (\"{0}\", \"{1}\", {2}, \"{3}\", {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
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

        _command.CommandText = string.Format("INSERT INTO Team_Members (Name, Sex, Age, Country, Level, Experience, Moral, Potential, Skill_1, Skill_2, Skill_3, Skill_4) " +
            "VALUES (\"{0}\", \"{1}\", {2}, \"{3}\", {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
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

        _command.CommandText = string.Format("INSERT INTO Team_Members (Name, Sex, Age, Country, Level, Experience, Moral, Potential, Skill_1, Skill_2, Skill_3, Skill_4) " +
            "VALUES (\"{0}\", \"{1}\", {2}, \"{3}\", {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
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
            _command.CommandText = string.Format("INSERT INTO Team_Members (Name, Sex, Age, Country, Level, Experience, Moral, Potential, Skill_1, Skill_2, Skill_3, Skill_4) " +
                "VALUES (\"{0}\", \"{1}\", {2}, \"{3}\", {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
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
        _command.CommandText = "SELECT last_insert_rowid();";
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

        _command = _connection.CreateCommand();

        _command.CommandText = string.Format("UPDATE Saved_Game SET " +
            "Team_Name = \"{0}\", " +
            "Car_Color = \"{1}\", " +
            "Car_Number = {2}, " +
            "Money = {3}, " +
            "Game_Time = \"{4}\", " +
            "Team_Members = \"{5}\", " +
            "Current_Country = \"{6}\" " +
            "WHERE ID = {7}",
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

    public Team_Struct LoadTeamByString(string _teamMembers)
    {
        Team_Struct ts = new();

        if (Connect() == false)
            return ts;

        Initiate_OpTimer();

        _command = _connection.CreateCommand();

        _command.CommandText = string.Format("SELECT * FROM Team_Members WHERE ID in ({0})", _teamMembers);

        IDataReader reader = _command.ExecuteReader();
        int readIndex = 0;
        int crewIndex = 0;

        ts.CrewMembers = new NpcPitCrewMember[4];

        while (reader.Read())
        {
            switch (readIndex)
            {
                //Race Enineer
                case 0:
                    ts.Engineer = new();
                    ts.Engineer.dbId = reader.GetInt32(0);
                    ts.Engineer.name = reader.GetString(1);
                    ts.Engineer.sex = reader.GetString(2);
                    ts.Engineer.age = reader.GetInt32(3);
                    ts.Engineer.country = reader.GetString(4);
                    ts.Engineer.level = reader.GetInt32(5);
                    ts.Engineer.xp = reader.GetInt32(6);
                    ts.Engineer.moralValue = reader.GetInt32(7);
                    ts.Engineer.potencial = reader.GetInt32(8);
                    ts.Engineer.skills[0].VALUE = reader.GetInt32(9);
                    ts.Engineer.skills[1].VALUE = reader.GetInt32(10);
                    ts.Engineer.skills[2].VALUE = reader.GetInt32(11);
                    ts.Engineer.skills[3].VALUE = reader.GetInt32(12);
                    break;
                //Driver
                case 1:
                    ts.Driver = new();
                    ts.Driver.dbId = reader.GetInt32(0);
                    ts.Driver.name = reader.GetString(1);
                    ts.Driver.sex = reader.GetString(2);
                    ts.Driver.age = reader.GetInt32(3);
                    ts.Driver.country = reader.GetString(4);
                    ts.Driver.level = reader.GetInt32(5);
                    ts.Driver.xp = reader.GetInt32(6);
                    ts.Driver.moralValue = reader.GetInt32(7);
                    ts.Driver.potencial = reader.GetInt32(8);
                    ts.Driver.skills[0].VALUE = reader.GetInt32(9);
                    ts.Driver.skills[1].VALUE = reader.GetInt32(10);
                    ts.Driver.skills[2].VALUE = reader.GetInt32(11);
                    ts.Driver.skills[3].VALUE = reader.GetInt32(12);
                    break;
                //Pit Crew Leader
                case 2:
                    ts.CrewLeader = new();
                    ts.CrewLeader.dbId = reader.GetInt32(0);
                    ts.CrewLeader.name = reader.GetString(1);
                    ts.CrewLeader.sex = reader.GetString(2);
                    ts.CrewLeader.age = reader.GetInt32(3);
                    ts.CrewLeader.country = reader.GetString(4);
                    ts.CrewLeader.level = reader.GetInt32(5);
                    ts.CrewLeader.xp = reader.GetInt32(6);
                    ts.CrewLeader.moralValue = reader.GetInt32(7);
                    ts.CrewLeader.potencial = reader.GetInt32(8);
                    ts.CrewLeader.skills[0].VALUE = reader.GetInt32(9);
                    ts.CrewLeader.skills[1].VALUE = reader.GetInt32(10);
                    ts.CrewLeader.skills[2].VALUE = reader.GetInt32(11);
                    ts.CrewLeader.skills[3].VALUE = reader.GetInt32(12);
                    break;
                //Pit Crew Members
                default:
                    ts.CrewMembers[crewIndex] = new();
                    ts.CrewMembers[crewIndex].dbId = reader.GetInt32(0);
                    ts.CrewMembers[crewIndex].name = reader.GetString(1);
                    ts.CrewMembers[crewIndex].sex = reader.GetString(2);
                    ts.CrewMembers[crewIndex].age = reader.GetInt32(3);
                    ts.CrewMembers[crewIndex].country = reader.GetString(4);
                    ts.CrewMembers[crewIndex].level = reader.GetInt32(5);
                    ts.CrewMembers[crewIndex].xp = reader.GetInt32(6);
                    ts.CrewMembers[crewIndex].moralValue = reader.GetInt32(7);
                    ts.CrewMembers[crewIndex].potencial = reader.GetInt32(8);
                    ts.CrewMembers[crewIndex].skills[0].VALUE = reader.GetInt32(9);
                    ts.CrewMembers[crewIndex].skills[1].VALUE = reader.GetInt32(10);
                    ts.CrewMembers[crewIndex].skills[2].VALUE = reader.GetInt32(11);
                    ts.CrewMembers[crewIndex].skills[3].VALUE = reader.GetInt32(12);
                    crewIndex++;
                    break;
            }

            readIndex++;
        }

        Print_OpTimer("LoadTeamByString()");
        reader.Close();
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

        _command = _connection.CreateCommand();

        //Clear Saved_Game row
        _command.CommandText = string.Format("UPDATE Saved_Game SET " +
            "Team_Name = \"\", Car_Color = \"\", Car_Number = 0, Money = 0, Game_Time = \"\", Team_Members = \"\", Current_Country = \"\" WHERE ID = {0};\n", _id);

        //Delete all team members linked with this save
        _command.CommandText += string.Format("DELETE FROM Team_Members WHERE ID in ({0});", _teamLinkedWithSave);

        _command.ExecuteNonQuery();

        Print_OpTimer("DeleteSavedGameByID()");

        CloseConnection();

        return true;
    }

    #endregion

    //NPC Data DataBase Methods
    #region NPC Data Methods

    private bool Connect_NpcDataDb()
    {
        _connection = new SqliteConnection(NpcDataDb_url);

        if (_connection != null)
        {
            _connection.Open();
            return true;
        }
        else
            return false;
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

            _command = _connection.CreateCommand();

            _command.CommandText = "SELECT * FROM Names WHERE ID=" + rng;

            IDataReader reader = _command.ExecuteReader();

            while (reader.Read())
            {
                layout.name = reader.GetString(1);

                if (reader.GetString(2) == "mf")
                {
                    int sex_rng = Random.Range(0, 2);

                    if (sex_rng == 0)
                        layout.sex = "m";
                    else
                        layout.sex = "f";
                }
                else
                    layout.sex = reader.GetString(2);
            }

            reader.Close();

            rng = Random.Range(1, NAMES_SIZE);

            _command.CommandText = "SELECT * FROM Names WHERE ID=" + rng;

            reader = _command.ExecuteReader();

            while (reader.Read())
            {
                layout.name = layout.name + " " + reader.GetString(3);
            }

            reader.Close();

            rng = Random.Range(1, COUNTRIES_SIZE);

            _command.CommandText = "SELECT * FROM CountriesOfTheWorld WHERE ID=" + rng;

            reader = _command.ExecuteReader();

            while (reader.Read())
            {
                layout.country = reader.GetString(1);
            }

            reader.Close();

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

                _command = _connection.CreateCommand();

                _command.CommandText = "SELECT * FROM Names WHERE ID=" + rng;

                IDataReader reader = _command.ExecuteReader();

                while (reader.Read())
                {
                    _structArray[i].name = reader.GetString(1);

                    if (reader.GetString(2) != "mf")
                        _structArray[i].sex = reader.GetString(2);
                    else
                    {
                        if (Random.Range(0, 2) == 0)
                            _structArray[i].sex = "m";
                        else
                            _structArray[i].sex = "f";
                    }
                }

                reader.Close();

                rng = Random.Range(1, NAMES_SIZE);

                _command.CommandText = "SELECT * FROM Names WHERE ID=" + rng;

                reader = _command.ExecuteReader();

                while (reader.Read())
                {
                    _structArray[i].name = _structArray[i].name + " " + reader.GetString(3);
                }

                reader.Close();

                rng = Random.Range(1, COUNTRIES_SIZE);

                _command.CommandText = "SELECT * FROM CountriesOfTheWorld WHERE ID=" + rng;

                reader = _command.ExecuteReader();

                while (reader.Read())
                {
                    _structArray[i].country = reader.GetString(1);
                }

                reader.Close();

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
        _connection = new SqliteConnection(TracksDb_url);

        if (_connection != null)
        {
            _connection.Open();
            return true;
        }
        else
            return false;
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

        _command = _connection.CreateCommand();
        _command.CommandText = "SELECT last_insert_rowid();";
        int SIZE = int.Parse(_command.ExecuteScalar().ToString());

        int index = (_index == -1) ? Random.Range(1, SIZE) : _index;

        _command.CommandText = "SELECT * FROM Tracks WHERE ID = " + index;

        IDataReader reader = _command.ExecuteReader();

        while (reader.Read())
        {
            _track.trackName = reader.GetString(1);
            _track.country = reader.GetString(2);
            _track.length = float.Parse(reader.GetString(3), CultureInfo.InvariantCulture);
            _track.laps = reader.GetInt32(4);
            _track.type = (TrackType)reader.GetInt32(5);
            _track.curve = reader.GetString(6).Convert_StringToTrack();
        }

        if (_track.type == TrackType.CIRCUIT || _track.type == TrackType.OVAL)
            _track.curve.close = true;

        Print_OpTimer("Load_Track()");

        reader.Close();
        CloseConnection();

        return true;
    }

    #endregion // TRACK_DB_METHODS



    private void CloseConnection()
    {
        if (_connection != null)
            _connection.Close();

        _connection = null;
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
}
