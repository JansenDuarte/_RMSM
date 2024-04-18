using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;





//HACK: This is the tool to save the tracks into the database!
/*
        This contains a few issues:
    
    The database URLs are "magic numbers". This could prove to be intensive during debuging.

    Parts of the track generation are completly random. This can remove some countries from the roster.

    The number of laps can become too small, or even too big.

    The calculation of the track length is purely based on the curve length. This could become unrealistic in size.

*/

[ExecuteInEditMode]
public static class TrackSaver
{

    private const int MAX_NUM_LAPS = 9;

    private static readonly string TracksDb_url = "URI=file:" + Application.dataPath + "/99.InternalDataBase/" + "Tracks.db";
    private static readonly string NpcDataDb_url = "URI=file:" + Application.dataPath + "/99.InternalDataBase/" + "NpcData.db";

    private static IDbConnection _connection = null;

    private static IDbCommand _command = null;


    private static bool Connect_NpcDataDb()
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


    private static bool Connect_TrackDb()
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

    public static bool Save_Track(string _track, TrackType _type, float _length)
    {
        string prefix = Generate_Prefix();
        string sufix = Generate_Sufix(ref _type);
        string country = Generate_Country();
        int laps = Calculate_Laps(ref _type);


        if (Connect_TrackDb() == false)
        {
            Debug.Log("<b>DB Connector</b> - ERROR! Track database connection could not be reached!");
            return false;
        }

        _command = _connection.CreateCommand();

        Debug.Log(string.Format("{0} {1} ; {2} ; {3}km ; {4} ; {5}", prefix, sufix, country, _length.ToString(), laps.ToString(), _type.ToString()));

        //TODO create save command
        _command.CommandText = string.Format("INSERT INTO Tracks (Name, Country, Length, Laps, Type, Curve)" +
        " VALUES (\"{0}\", \"{1}\", \"{2}\", {3}, \"{4}\", \"{5}\")",
        prefix + " " + sufix, country, _length, laps, _type.ToString(), _track);

        _command.ExecuteNonQuery();

        CloseConnection();

        return true;
    }

    private static int Calculate_Laps(ref TrackType _type)
    {
        int ret = -1;

        if (_type == TrackType.RALLY || _type == TrackType.HILL_CLIMB || _type == TrackType.DRAG)
            ret = 0;
        else
            ret = Random.Range(1, MAX_NUM_LAPS);

        return ret;
    }


    private static string Generate_Country()
    {
        string ret = "";

        if (Connect_NpcDataDb() == false)
            return ret;

        int rng = Random.Range(1, 196);

        _command = _connection.CreateCommand();

        _command.CommandText = "SELECT * FROM CountriesOfTheWorld WHERE ID=" + rng;

        IDataReader reader = _command.ExecuteReader();

        while (reader.Read())
        {
            ret = reader.GetString(1);  //Pos 1 is the country name column
        }

        CloseConnection();

        return ret;
    }



    private static string Generate_Prefix()
    {
        string ret = "";

        if (Connect_NpcDataDb() == false)
            return ret;

        int rng = Random.Range(1, 1001);

        _command = _connection.CreateCommand();

        _command.CommandText = "SELECT * FROM Names WHERE ID=" + rng;

        IDataReader reader = _command.ExecuteReader();

        while (reader.Read())
        {
            ret = reader.GetString(3);  //Pos 3 is the surename column
        }

        CloseConnection();

        return ret;
    }






    private static string Generate_Sufix(ref TrackType _type)
    {
        string ret = "";
        if (_type == TrackType.RALLY)
            return ret;

        Dictionary<TrackType, string[]> dict = new()
        {
            {TrackType.CIRCUIT, new string[] {"Arena", "Circuit", "Raceway", "Speedway", "Race track", "Air field", "Speed park", "Motorsport park", "Motor park", "Race resort"}},

            {TrackType.HILL_CLIMB, new string[] { "Hills", "Hill climb"}},

            {TrackType.OVAL, new string[] { "Arena", "Circuit", "Raceway", "Speedway", "Race track", "Speed park", "Motorsport park", "Motor park", "Race resort"}},

            {TrackType.DRAG, new string[] {"Dragway", "Strip", "Speedway",}}
        };

        switch (_type)
        {
            case TrackType.CIRCUIT:
                ret = dict[TrackType.CIRCUIT][Random.Range(0, dict[TrackType.CIRCUIT].Length)];
                break;
            case TrackType.HILL_CLIMB:
                ret = dict[TrackType.HILL_CLIMB][Random.Range(0, dict[TrackType.HILL_CLIMB].Length)];
                break;
            case TrackType.OVAL:
                ret = dict[TrackType.OVAL][Random.Range(0, dict[TrackType.OVAL].Length)];
                break;
            case TrackType.DRAG:
                ret = dict[TrackType.DRAG][Random.Range(0, dict[TrackType.DRAG].Length)];
                break;
        }

        return ret;
    }

    private static void CloseConnection()
    {
        if (_connection != null)
            _connection.Close();
        _connection = null;
    }

}