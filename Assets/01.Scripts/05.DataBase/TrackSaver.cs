using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;
using ExtensionMethods;





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

        _command.CommandText = string.Format("INSERT INTO Tracks (Name, Country, Length, Laps, Type, Curve)" +
        " VALUES (\"{0}\", \"{1}\", \"{2}\", {3}, \"{4}\", \"{5}\")",
        prefix + " " + sufix, country, _length, laps, (int)_type, _track);

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






    #region UTILITIES

    private const int MIN_POINT_AMMOUNT = 4;
    private const int QUADRANT_AMMOUNT = 4;

    private const float POINT_POS_LIMIT = 5f;

    public static void Randomize_Track(this BezierCurve _curve)
    {
        GenerateTrackLayout(ref _curve);
    }

    private static void GenerateTrackLayout(ref BezierCurve _curve, int _pointLimit = 12)
    {
        int pointAmmount = Random.Range(MIN_POINT_AMMOUNT, _pointLimit + 1);

        BezierPoint[] points = Generate_Points(ref _curve, pointAmmount);
        Position_Points(ref points);

        float turnSharpness;
        float handleMax = 1f;

        _curve.close = true;

        for (int i = 0; i < pointAmmount; i++)
        {
            points[i].curve = _curve;

            //I don't know the exact behaviour of this
            if (i == 0)
                turnSharpness = Vector3.Distance(points[pointAmmount - 1].localPosition, points[i].localPosition) / POINT_POS_LIMIT;
            else
                turnSharpness = Vector3.Distance(points[i - 1].localPosition, points[i].localPosition) / POINT_POS_LIMIT;


            //This is doing nothing! handleMin & max is never set!
            //points[i].handle2 = new Vector3(Random.Range(turnSharpness * handleMin_X, turnSharpness * handlMax_X), Random.Range(turnSharpness * handleMin_Y, turnSharpness * handleMax_Y), 0f);
            if (i - 1 < 0)
                points[i].handle2 = Find_Handle_Pos(turnSharpness, handleMax, points[pointAmmount - 1].localPosition, points[i].localPosition, points[(i + 1) % pointAmmount].localPosition);
            else
                points[i].handle2 = Find_Handle_Pos(turnSharpness, handleMax, points[i - 1].localPosition, points[i].localPosition, points[(i + 1) % pointAmmount].localPosition);

        }

    }

    private static Vector3 Find_Handle_Pos(float _turnSharpness, float _maxValue, Vector3 _pastPoint, Vector3 _presentPoint, Vector3 _futurePoint)
    {
        Vector3 p = Vector3.zero;

        if (_turnSharpness > POINT_POS_LIMIT / 10f)
        {
            //Direction from current point TO next point
            Vector3 d = _presentPoint.Direction(_futurePoint);
            //Vector perpendicular between direction and current position
            p = Vector3.Cross(d, _presentPoint);
            //Vector perpendicular between current point and next point
            p = Vector3.Cross(d, p).normalized * -1f;
            // Debug.Log("Calculating based on future point");
        }
        else
        {
            //Direction from current point TO next point
            Vector3 d = _presentPoint.Direction(_pastPoint);
            //Vector perpendicular between direction and current position
            p = Vector3.Cross(d, _presentPoint);
            //Vector perpendicular between current point and next point
            p = Vector3.Cross(d, p).normalized;
            // Debug.Log("Calculating based on past point");
        }
        //the position might be inverted by some other factor, don't know it yet
        return p * _turnSharpness;
    }

    private static BezierPoint[] Generate_Points(ref BezierCurve _curve, int _pointAmmount)
    {
        BezierPoint[] retValue = new BezierPoint[_pointAmmount];

        for (int i = 0; i < _pointAmmount; i++)
        {
            BezierPoint point = _curve.AddPointAt(Vector3.zero);
            retValue[i] = point;
        }

        return retValue;
    }

    private static void Position_Points(ref BezierPoint[] _points)
    {
        Vector2 pos;

        //pointPerQuadrant => how many points should be place in each quadrant
        //rem => how many quadrants need to be visited again
        int pointPerQuadrant = System.Math.DivRem(_points.Length, QUADRANT_AMMOUNT, out int rem);
        int index = 0;


        for (int i = 0; i < QUADRANT_AMMOUNT; i++)
        {
            switch (i % QUADRANT_AMMOUNT)
            {
                case 0:
                    //Quadrante 1
                    for (int j = 0; j < pointPerQuadrant; j++)
                    {
                        pos = ExM.RandomVector2(POINT_POS_LIMIT, false);
                        _points[index].localPosition = new Vector3(pos.x, pos.y, 0f);
                        index++;
                    }
                    if (rem > 0)
                    {
                        pos = ExM.RandomVector2(POINT_POS_LIMIT, false);
                        _points[index].localPosition = new Vector3(pos.x, pos.y, 0f);
                        index++;
                        rem--;
                    }
                    break;
                case 1:
                    //Quadrante 2
                    for (int j = 0; j < pointPerQuadrant; j++)
                    {
                        pos = ExM.RandomVector2(POINT_POS_LIMIT, false);
                        _points[index].localPosition = new Vector3(pos.x, -pos.y, 0f);
                        index++;
                    }
                    if (rem > 0)
                    {
                        pos = ExM.RandomVector2(POINT_POS_LIMIT, false);
                        _points[index].localPosition = new Vector3(pos.x, -pos.y, 0f);
                        index++;
                        rem--;
                    }
                    break;
                case 2:
                    //Quadrante 3
                    for (int j = 0; j < pointPerQuadrant; j++)
                    {
                        pos = ExM.RandomVector2(POINT_POS_LIMIT, false);
                        _points[index].localPosition = new Vector3(-pos.x, -pos.y, 0f);
                        index++;
                    }
                    if (rem > 0)
                    {
                        pos = ExM.RandomVector2(POINT_POS_LIMIT, false);
                        _points[index].localPosition = new Vector3(-pos.x, -pos.y, 0f);
                        index++;
                        rem--;
                    }
                    break;
                case 3:
                    //Quadrante 4
                    for (int j = 0; j < pointPerQuadrant; j++)
                    {
                        pos = ExM.RandomVector2(POINT_POS_LIMIT, false);
                        _points[index].localPosition = new Vector3(-pos.x, pos.y, 0f);
                        index++;
                    }
                    break;
            }
        }
    }

    #endregion // UTILITIES


}