using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

public class RaceDayHelper : MonoBehaviour
{
    //Text information
    [Header("Text Information")]
    public TextMeshProUGUI trackEventName;
    public TextMeshProUGUI trackName;
    public TextMeshProUGUI trackCountry;
    public TextMeshProUGUI trackLaps;
    public TextMeshProUGUI trackWeather;
    public CompetitorData_Struct[] competitorsArray;

    //Image information
    [Header("Image Information")]
    public Sprite lightOn;
    public Sprite lightOff;
    public Image[] raceLights;

    //Track Components information
    [Header("Track Components Information")]
    public TrackGenerator trackGenerator;
    public SpriteShapeController shapeController;
    public float trackHeight;

    //UI Panels
    [Header("Layout Panels")]
    public GameObject raceInfo_Panel_GO;
    public GameObject race_Panel_GO;
    public GameObject raceEnd_Panel_GO;
    [Space]
    public GameObject competitorList_GO;




    //FIXME: This is all fucked! Needs to be fixed ASAP
    public void PrepareRaceDayInfo(ref RaceCar_Struct[] _competitorCarsArray, ref Track _trackInfo)
    {
        ShowTrackLayout(_trackInfo.curve);

        trackEventName.text = _trackInfo.trackName;
        trackName.text = _trackInfo.trackName;
        trackCountry.text = _trackInfo.country;
        trackLaps.text = _trackInfo.laps.ToString();
        trackWeather.text = TrackGenerator.GenerateWeather();

        competitorsArray[0].competitor_Name.text = _competitorCarsArray[0].driver.name;
        competitorsArray[0].competitor_CarNumber_AndColor.text = (_competitorCarsArray[0].driver.carNumber < 10) ? string.Format("0{0}", _competitorCarsArray[0].driver.carNumber) : _competitorCarsArray[0].driver.carNumber.ToString();
        competitorsArray[0].competitor_CarNumber_AndColor.color = PlayerManager.Instance.TeamColor;
        _competitorCarsArray[0].sprite.color = PlayerManager.Instance.TeamColor;

        //Set up the competitors (after the player car)
        for (int i = 1; i < _competitorCarsArray.Length; i++)
        {

            competitorsArray[i].competitor_Name.text = _competitorCarsArray[i].driver.name;
            competitorsArray[i].competitor_CarNumber_AndColor.text = (_competitorCarsArray[i].driver.carNumber < 10) ? string.Format("0{0}", _competitorCarsArray[i].driver.carNumber) : _competitorCarsArray[i].driver.carNumber.ToString();
            Color rngColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0f, 1f, 1f, 1f); ;
            competitorsArray[i].competitor_CarNumber_AndColor.color = rngColor;
            _competitorCarsArray[i].sprite.color = rngColor;
        }
    }



    private void ShowTrackLayout(BezierCurve _curve)
    {
        BezierPoint[] points = _curve.GetAnchorPoints();

        for (int i = 0; i < 2; i++)
        {
            shapeController.spline.SetPosition(i, points[i].localPosition);
            shapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            shapeController.spline.SetLeftTangent(i, points[i].handle1);
            shapeController.spline.SetRightTangent(i, points[i].handle2);
            shapeController.spline.SetHeight(i, trackHeight);
        }

        // shapeController.spline.SetPosition(1, points[1].localPosition);
        // shapeController.spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        // shapeController.spline.SetLeftTangent(1, points[1].handle1);
        // shapeController.spline.SetRightTangent(1, points[1].handle2);
        // shapeController.spline.SetHeight(1, trackHeight);

        for (int i = 2; i < points.Length; i++)
        {
            shapeController.spline.InsertPointAt(i, points[i].localPosition);
            shapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            shapeController.spline.SetLeftTangent(i, points[i].handle1);
            shapeController.spline.SetRightTangent(i, points[i].handle2);
            shapeController.spline.SetHeight(i, trackHeight);
        }

        shapeController.spline.InsertPointAt(points.Length, points[0].localPosition);
        shapeController.spline.SetTangentMode(points.Length, ShapeTangentMode.Continuous);
        shapeController.spline.SetLeftTangent(points.Length, points[0].handle1);
        shapeController.spline.SetRightTangent(points.Length, points[0].handle2);
        shapeController.spline.SetHeight(points.Length, trackHeight);
    }



    #region UI_CALLED_METHODS

    public void UI_StartRace()
    {
        raceInfo_Panel_GO.SetActive(false);

        competitorList_GO.transform.SetParent(race_Panel_GO.transform);

        race_Panel_GO.SetActive(true);
    }

    #endregion



    #region COROUTINES

    public bool raceLightsEnded = false;

    public IEnumerator RaceLights()
    {
        raceLightsEnded = false;

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSecondsRealtime(1f);

            raceLights[i].sprite = lightOn;
        }

        yield return new WaitForSecondsRealtime(Random.Range(1f, 2f));
        for (int i = 0; i < 3; i++) { raceLights[i].sprite = lightOff; }

        raceLightsEnded = true;

        yield return 0;
    }

    #endregion
}
