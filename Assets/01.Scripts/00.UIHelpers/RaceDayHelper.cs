using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;
using ExtensionMethods;

public class RaceDayHelper : MonoBehaviour
{
    #region VARIABLES

    //Text information
    [Header("Text Information")]
    public TextMeshProUGUI trackEventName;
    public TextMeshProUGUI trackName;
    public TextMeshProUGUI trackCountry;
    public TextMeshProUGUI trackLaps;
    public TextMeshProUGUI trackWeather;
    public CompetitorData_Struct[] racersInfo;

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

    #endregion // VARIABLES



    public void PrepareRaceDayInfo(ref RaceCar[] _cars, ref Track _trackInfo)
    {
        ShowTrackLayout(_trackInfo.curve.GetAnchorPoints());

        //TODO: The event name might need to be changed
        trackEventName.text = _trackInfo.trackName;
        trackName.text = _trackInfo.trackName;
        trackCountry.text = _trackInfo.country;
        trackLaps.text = _trackInfo.laps.ToString();
        trackWeather.text = TrackGenerator.GenerateWeather();

        racersInfo[0].competitor_Name.text = PlayerManager.Instance.Driver.name;
        racersInfo[0].competitor_CarNumber_AndColor.text = PlayerManager.Instance.TeamNumber.Format_AddLeadingZero();
        racersInfo[0].competitor_CarNumber_AndColor.color = PlayerManager.Instance.TeamColor;
        _cars[0].sprite.color = PlayerManager.Instance.TeamColor;

        //Set up the competitors (after the player car)
        for (int i = 1; i < _cars.Length; i++)
        {
            racersInfo[i].competitor_Name.text = _cars[i].driver.name;
            racersInfo[i].competitor_CarNumber_AndColor.text = _cars[i].driver.carNumber.Format_AddLeadingZero();
            Color rngColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f);
            racersInfo[i].competitor_CarNumber_AndColor.color = rngColor;
            _cars[i].sprite.color = rngColor;
        }
    }

    public void Show_GridChanges(int _gainedPosIndex, int _lostPosIndex)
    {
        StartCoroutine(racersInfo[_gainedPosIndex].GainedPosition());
        StartCoroutine(racersInfo[_lostPosIndex].LostPosition());
        racersInfo[_gainedPosIndex].transform.SetSiblingIndex(_lostPosIndex);
        racersInfo[_lostPosIndex].transform.SetSiblingIndex(_gainedPosIndex);
        (racersInfo[_gainedPosIndex], racersInfo[_lostPosIndex]) = (racersInfo[_lostPosIndex], racersInfo[_gainedPosIndex]);
    }



    private void ShowTrackLayout(BezierPoint[] _points)
    {
        //Adding all the points in the sprite spline;
        for (int i = 2; i < _points.Length; i++) { shapeController.spline.InsertPointAt(i, _points[i].localPosition); }

        for (int i = 0; i < _points.Length; i++)
        {
            shapeController.spline.SetPosition(i, _points[i].localPosition);
            shapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            shapeController.spline.SetLeftTangent(i, _points[i].handle1);
            shapeController.spline.SetRightTangent(i, _points[i].handle2);
            shapeController.spline.SetHeight(i, trackHeight);
        }

        //Add the point to close the loop
        shapeController.spline.InsertPointAt(_points.Length, _points[0].localPosition);
        shapeController.spline.SetTangentMode(_points.Length, ShapeTangentMode.Continuous);
        shapeController.spline.SetLeftTangent(_points.Length, _points[0].handle1);
        shapeController.spline.SetRightTangent(_points.Length, _points[0].handle2);
        shapeController.spline.SetHeight(_points.Length, trackHeight);
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

        //Wait 1 second and remove the lights from the screen
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < raceLights.Length; i++) { raceLights[i].gameObject.SetActive(false); }

        yield break;
    }

    #endregion
}
