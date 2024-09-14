using System.Collections;
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
    public CompetitorData_Struct[] endRaceInfo;
    public TextMeshProUGUI xpGained;
    public TextMeshProUGUI moneyGained;

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

    [Header("Xp Structs")]
    public UI_SkillBar[] xp_bars;

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

        //Set up all the
        for (int i = 0; i < _cars.Length; i++)
        {
            racersInfo[i].competitor_Name.text = _cars[i].driver.name;
            racersInfo[i].competitor_CarNumber_AndColor.text = _cars[i].driver.carNumber.Format_AddLeadingZero();
            racersInfo[i].competitor_CarNumber_AndColor.color = _cars[i].sprite.color;
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


    public void Show_EndRace(ref RaceCar[] _cars, int _moneyGained, int _xpGained)
    {
        for (int i = 0; i < _cars.Length; i++)
        {
            endRaceInfo[i].competitor_Name.text = _cars[i].driver.name;
            endRaceInfo[i].competitor_CarNumber_AndColor.text = _cars[i].driver.carNumber.Format_AddLeadingZero();
            endRaceInfo[i].competitor_CarNumber_AndColor.color = racersInfo[i].competitor_CarNumber_AndColor.color;
        }

        moneyGained.text += _moneyGained.ToString();
        xpGained.text += _xpGained.ToString();

        raceEnd_Panel_GO.SetActive(true);
        raceInfo_Panel_GO.SetActive(false);
        race_Panel_GO.SetActive(false);

        StartCoroutine(Show_XpChanges(_xpGained));
    }

    private IEnumerator Show_XpChanges(int _xpGained)
    {
        //current xp xp to next level
        int xp2nextLevel = PlayerManager.Instance.Driver.level * NpcLayout.LEVELUP_SCALING;
        float value = PlayerManager.Instance.Driver.xp + _xpGained;
        value.Change_Range(0f, (float)xp2nextLevel, 0f, 1f);
        xp_bars[0].BarFill = value;

        yield return new WaitForSeconds(0.5f);

        xp2nextLevel = PlayerManager.Instance.Engineer.level * NpcLayout.LEVELUP_SCALING;
        value = PlayerManager.Instance.Engineer.xp + _xpGained;
        value.Change_Range(0f, (float)xp2nextLevel, 0f, 1f);
        xp_bars[1].BarFill = value;

        yield return new WaitForSeconds(0.5f);

        xp2nextLevel = PlayerManager.Instance.PitLeader.level * NpcLayout.LEVELUP_SCALING;
        value = PlayerManager.Instance.PitLeader.xp + _xpGained;
        value.Change_Range(0f, (float)xp2nextLevel, 0f, 1f);
        xp_bars[2].BarFill = value;

        yield break;
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
