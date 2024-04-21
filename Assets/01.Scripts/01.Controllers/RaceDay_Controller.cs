using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceDay_Controller : MonoBehaviour
{
    public RaceDayHelper raceDayHelper;

    public RaceCar[] cars;

    private Track track;

    private const int COMPETITOR_AMMOUNT = 7;

    public const float GRID_DIFF_FACTOR = 0.01f;


    void Start()
    {
        //DEBUG This needs to call the track that is beeing raced on
        DBConnector.Instance.Load_Track(out track, 1);

        PrepareRacersStats();

        //Show Layout
        raceDayHelper.PrepareRaceDayInfo(ref cars, ref track);


        //Position the cars
        for (int i = 0; i < cars.Length; i++)
        {
            float gridPosition = 1f - (GRID_DIFF_FACTOR * i);
            cars[i].startingEventPosition = i + 1;  //setting the grid starting position
            cars[i].trackPositionPerCent = gridPosition;
            cars[i].transform.position = track.curve.GetPointAt(gridPosition);
        }

        //Simulate Race

        //Race mini-games based on the happenings of the simulated race

        //Show end race data

        //End Raceday
    }

    private void PrepareRacersStats()
    {
        NpcDriver[] competitors = Competitor_Generator.GenerateCompetitors(COMPETITOR_AMMOUNT, 1).ToArray();

        cars[0].driver = PlayerManager.Instance.Driver;

        for (int i = 0; i < competitors.Length; i++)
        {
            cars[i + 1].driver = competitors[i];
        }
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
        int laps = 1;
        float newTrackPosition;

        //DEBUG
        while (laps <= track.laps)
        {
            for (int i = 0; i < cars.Length; i++)
            {
                newTrackPosition = cars[i].Drive();
                cars[i].transform.position = track.curve.GetPointAt(newTrackPosition);
            }
            //FIXME: this is not the right way to do it
            for (int i = 0; i < GameManager.Instance.simulationSpeed; i++) { yield return new WaitForEndOfFrame(); }

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
