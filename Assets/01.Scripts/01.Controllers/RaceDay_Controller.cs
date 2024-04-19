using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceDay_Controller : MonoBehaviour
{
    public RaceDayHelper raceDayHelper;

    public RaceCar_Struct[] carSpritesArray;

    private Track track;


    void Start()
    {
        //DEBUG This needs to call the track that is beeing raced on
        DBConnector.Instance.Load_Track(out track, 1);

        Debug_PrepareRacersStats();


        //Show Layout
        raceDayHelper.PrepareRaceDayInfo(ref carSpritesArray, ref track);


        //Position the cars
        BezierPoint[] trackAnchorPoints_Array = track.curve.GetAnchorPoints();
        BezierPoint p1 = trackAnchorPoints_Array[trackAnchorPoints_Array.Length - 1];
        BezierPoint p2 = trackAnchorPoints_Array[0];
        for (int i = 0; i < carSpritesArray.Length; i++)
        {
            float gridPosition = 1f - (0.08f * i);
            carSpritesArray[i].trackPosition = gridPosition;
            carSpritesArray[i].transform.position = BezierCurve.GetPoint(p1, p2, gridPosition);
        }

        //Simulate Race

        //Race mini-games based on the happenings of the simulated race

        //Show end race data

        //End Raceday
    }

    private void Debug_PrepareRacersStats()
    {
        //NpcDriver[] _auxArray = Competitor_Generator.GenerateCompetitors(7, 1).ToArray();

        carSpritesArray[0].driver = PlayerManager.Instance.Driver;

        //for (int i = 1; i < _auxArray.Length; i++)
        //{
        //    carSpritesArray[i].driver = _auxArray[i];
        //}
    }

    private IEnumerator RaceStart_MiniGame()
    {
        bool didFalseStart = false;

        //Show race start UI - Press clutch to start

        //wait for click to begin
        Debug.Log("Press the clutch to start");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) == true);

        //send request to start the lights coroutine
        StartCoroutine(raceDayHelper.RaceLights());
        while (raceDayHelper.raceLightsEnded != true)
        {
            yield return new WaitForEndOfFrame();

            if (Input.GetKey(KeyCode.Mouse0) == false)
            {
                //False start; Fucker gets a drive through penalty

                //Show false start message and the penalty
                didFalseStart = true;
                Debug.Log("False start... you get a drive through penalty");
            }

        }

        //get the time when lights finish
        float lightsOff_Time = Time.time;

        //wait for clutch click release
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Mouse0) == true || didFalseStart);
        float clutchOff_Time = Time.time;

        //compare times
        float reaction_Time = clutchOff_Time - lightsOff_Time;
        Debug.Log(string.Format("Reaction time: {0} ms", reaction_Time));

        //show results

        StartCoroutine(SimulateRace());

        yield break;
    }

    private IEnumerator SimulateRace()
    {
        int laps = 0;
        float newTrackPosition;

        //DEBUG
        while (laps < 10)
        {
            for (int i = 0; i < carSpritesArray.Length; i++)
            {
                newTrackPosition = carSpritesArray[i].trackPosition += 0.0001f * carSpritesArray[i].driver.GetGeneralPerformance();      //Change due to driver speed, stamina, and car attributes
                carSpritesArray[i].transform.position = track.curve.GetPointAt(newTrackPosition % 1f);
                carSpritesArray[i].trackPosition = newTrackPosition;
            }
            //FIXME: this needs to be a set timer for the updates; Can be changed to speed up the simulation
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            //Check overtake opportunities for all cars (?)

            //Lap count based on a collider && car in first position
        }

        //Race ended

        //Show race end info
        Show_EndRace_Info();

        yield break;
    }

    private void Overtake_MiniGame()
    {

    }

    private void Show_EndRace_Info()
    {

    }




    #region UI_CALLED_METHODS

    public void UI_StartRace()
    {
        raceDayHelper.UI_StartRace();
        StartCoroutine(RaceStart_MiniGame());
    }

    #endregion
}
