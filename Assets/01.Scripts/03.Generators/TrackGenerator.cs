using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class TrackGenerator : MonoBehaviour
{
    private const int MIN_POINT_AMMOUNT = 4;
    private const int QUADRANT_AMMOUNT = 4;
    public GameObject trackGO;
    public GameObject pointPrefabGO;


    public float pointPosLimit;

    private BezierCurve p_trackLayout;
    public BezierCurve TrackLayout
    {
        get
        {
            if (p_trackLayout != null)
                return p_trackLayout;
            else
                return GenerateTrackLayout();
        }

        private set
        {
            p_trackLayout = GenerateTrackLayout();
        }
    }

    private BezierPoint[] points;


    private BezierCurve GenerateTrackLayout(int _pointLimit = 6)    //FIXME: NEEDS REFACTORING
    {
        int pointAmmount = Random.Range(MIN_POINT_AMMOUNT, _pointLimit + 1);

        points = Generate_Points(pointAmmount);
        Position_Points();

        float turnSharpness;
        float handleMax = 1f;

        p_trackLayout = trackGO.GetComponent<BezierCurve>();
        p_trackLayout.close = true;

        //TODO: the line connecting the handels needs to be perpendicular to the next point

        for (int i = 0; i < pointAmmount; i++)
        {
            points[i].curve = p_trackLayout;

            //I don't know the exact behaviour of this
            if (i == 0)
                turnSharpness = Vector3.Distance(points[pointAmmount - 1].localPosition, points[i].localPosition) / pointPosLimit;
            else
                turnSharpness = Vector3.Distance(points[i - 1].localPosition, points[i].localPosition) / pointPosLimit;


            //This is doing nothing! handleMin & max is never set!
            //points[i].handle2 = new Vector3(Random.Range(turnSharpness * handleMin_X, turnSharpness * handlMax_X), Random.Range(turnSharpness * handleMin_Y, turnSharpness * handleMax_Y), 0f);
            if (i - 1 < 0)
                points[i].handle2 = Find_Handle_Pos(turnSharpness, handleMax, points[pointAmmount - 1].localPosition, points[i].localPosition, points[(i + 1) % pointAmmount].localPosition);
            else
                points[i].handle2 = Find_Handle_Pos(turnSharpness, handleMax, points[i - 1].localPosition, points[i].localPosition, points[(i + 1) % pointAmmount].localPosition);

        }

        return p_trackLayout;
    }

    private Vector3 Find_Handle_Pos(float _turnSharpness, float _maxValue, Vector3 _pastPoint, Vector3 _presentPoint, Vector3 _futurePoint)
    {
        Vector3 p = Vector3.zero;

        //FIXME this factor needs to be rethinked
        if (_turnSharpness > pointPosLimit / 10f)
        {
            //TEST Point towards the next point
            //Direction from current point TO next point
            Vector3 d = _presentPoint.Direction(_futurePoint);
            //Vector perpendicular between direction and current position
            p = Vector3.Cross(d, _presentPoint);
            //Vector perpendicular between current point and next point
            p = Vector3.Cross(d, p).normalized * -1f;
            Debug.Log("Calculating based on future point");
        }
        else
        {
            //TEST Point towards the previous point
            //Direction from current point TO next point
            Vector3 d = _presentPoint.Direction(_pastPoint);
            //Vector perpendicular between direction and current position
            p = Vector3.Cross(d, _presentPoint);
            //Vector perpendicular between current point and next point
            p = Vector3.Cross(d, p).normalized;
            Debug.Log("Calculating based on past point");
        }
        //the position might be inverted by some other factor, don't know it yet
        return p * _turnSharpness;
    }

    private BezierPoint[] Generate_Points(int _pointAmmount)
    {
        BezierPoint[] retValue = new BezierPoint[_pointAmmount];

        for (int i = 0; i < _pointAmmount; i++)
        {
            BezierPoint point = Instantiate(pointPrefabGO, trackGO.transform).GetComponent<BezierPoint>();
            retValue[i] = point;
        }

        return retValue;
    }

    //FIXME: this shit is ugly as fuck! Super hacky and I repeat myself a bunch of times
    private void Position_Points()
    {
        Vector2 pos;

        //pointPerQuadrant => how many points should be place in each quadrant
        //rem => how many quadrants need to be visited again
        int pointPerQuadrant = System.Math.DivRem(points.Length, QUADRANT_AMMOUNT, out int rem);
        int index = 0;


        //TODO Tracks are circles!! Make a circle first, then deform the circle to make the track
        //FIXME i THINK making the position random is problematic
        for (int i = 0; i < QUADRANT_AMMOUNT; i++)
        {
            switch (i % QUADRANT_AMMOUNT)
            {
                case 0:
                    //Quadrante 1
                    for (int j = 0; j < pointPerQuadrant; j++)
                    {
                        pos = ExM.RandomVector2(pointPosLimit, false);
                        points[index].localPosition = new Vector3(pos.x, pos.y, 0f);
                        index++;
                    }
                    if (rem > 0)
                    {
                        pos = ExM.RandomVector2(pointPosLimit, false);
                        points[index].localPosition = new Vector3(pos.x, pos.y, 0f);
                        index++;
                        rem--;
                    }
                    break;
                case 1:
                    //Quadrante 2
                    for (int j = 0; j < pointPerQuadrant; j++)
                    {
                        pos = ExM.RandomVector2(pointPosLimit, false);
                        points[index].localPosition = new Vector3(pos.x, -pos.y, 0f);
                        index++;
                    }
                    if (rem > 0)
                    {
                        pos = ExM.RandomVector2(pointPosLimit, false);
                        points[index].localPosition = new Vector3(pos.x, -pos.y, 0f);
                        index++;
                        rem--;
                    }
                    break;
                case 2:
                    //Quadrante 3
                    for (int j = 0; j < pointPerQuadrant; j++)
                    {
                        pos = ExM.RandomVector2(pointPosLimit, false);
                        points[index].localPosition = new Vector3(-pos.x, -pos.y, 0f);
                        index++;
                    }
                    if (rem > 0)
                    {
                        pos = ExM.RandomVector2(pointPosLimit, false);
                        points[index].localPosition = new Vector3(-pos.x, -pos.y, 0f);
                        index++;
                        rem--;
                    }
                    break;
                case 3:
                    //Quadrante 4
                    for (int j = 0; j < pointPerQuadrant; j++)
                    {
                        pos = ExM.RandomVector2(pointPosLimit, false);
                        points[index].localPosition = new Vector3(-pos.x, pos.y, 0f);
                        index++;
                    }
                    break;
            }
        }
    }


    //TODO: this needs to be set previously
    public static string GenerateWeather()
    {
        string rngWeather = string.Empty;

        switch (Random.Range(0, 3))
        {
            case 0:
                rngWeather = "Clear ";
                break;
            case 1:
                rngWeather = "Cloudy ";
                break;
            case 2:
                rngWeather = "Rainy ";
                break;
        }

        rngWeather += (Random.Range(0, 2) == 0) ? "Day" : "Night";

        return rngWeather;
    }
}
