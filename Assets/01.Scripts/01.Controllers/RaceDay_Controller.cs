using System.Collections;
using UnityEngine;

public class RaceDay_Controller : MonoBehaviour
{
    public RaceDayHelper raceDayHelper;

    public RaceCar[] cars;

    private Track track;

    private const int COMPETITOR_AMMOUNT = 7;

    public const float GRID_DIFF_FACTOR = 0.015f;
    public const float DISTANCE_CONVERSION_FACTOR = 100f;


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
            //FIXME: some of the cars are starting on top of eachother
            float gridPosition = 1f - (GRID_DIFF_FACTOR * i);
            cars[i].startingGridPosition = i + 1;  //setting the grid starting position
            cars[i].gridPosition = i + 1;
            cars[i].trackPositionPerCent = gridPosition * DISTANCE_CONVERSION_FACTOR;
            cars[i].transform.position = track.curve.GetPointAt(gridPosition);
        }
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

    int laps = 1;
    float newTrackPosition;
    Vector3 targetPosition;
    private IEnumerator SimulateRace()
    {
        //DEBUG
        while (laps <= track.laps)
        {
            SimulateDriving();

            //Check overtake opportunities for all cars
            Change_GridPosition();

            //Sorting happens when cars change position
            //Sort_ByGridPosition();

            //Lap count based on car in first position
            if (Check_LapCompleted(laps))
                laps++;

            //FIXME: this is not the right way to do it
            for (int i = 0; i < GameManager.Instance.SimulationSpeed; i++) { yield return new WaitForEndOfFrame(); }
        }

        StartCoroutine(EndEvent());

        yield break;
    }

    private IEnumerator EndEvent()
    {
        for (int i = 0; i < cars.Length; i++)
        { cars[i].checkeredFlag = true; }

        while (!AllCarsFinished())
        {
            SimulateDriving();

            //FIXME: this is not the right way to do it
            for (int i = 0; i < GameManager.Instance.SimulationSpeed; i++) { yield return new WaitForEndOfFrame(); }
        }

        Show_EndRace_Info();

        yield break;
    }

    private void SimulateDriving()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (!cars[i].raceCompleted)
            {
                newTrackPosition = cars[i].Drive() / DISTANCE_CONVERSION_FACTOR;
                targetPosition = track.curve.GetPointAt(newTrackPosition);
                cars[i].transform.position = targetPosition;
            }
        }
    }



    private bool AllCarsFinished()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (!cars[i].raceCompleted)
                return false;
        }

        return true;
    }

    private void Change_GridPosition()
    {
        int carInFrontIndex;

        for (int i = 0; i < cars.Length; i++)
        {
            carInFrontIndex = i - 1;

            //check position gained
            if (carInFrontIndex >= 0)
            {
                if (cars[i].trackPositionPerCent > cars[carInFrontIndex].trackPositionPerCent &&
                cars[i].currentLap == cars[carInFrontIndex].currentLap)
                {
                    cars[i].gridPosition--;
                    cars[carInFrontIndex].gridPosition++;
                    (cars[carInFrontIndex], cars[i]) = (cars[i], cars[carInFrontIndex]);    //Tupple for swapping positions
                    raceDayHelper.Show_GridChanges(i, carInFrontIndex);
                }
            }
        }
    }


    //INFO: Sorting after the grid changes had a bug when cars were too close
    // private void Sort_ByGridPosition()
    // {
    //     RaceCar[] sorted = new RaceCar[COMPETITOR_AMMOUNT + 1];

    //     for (int i = 0; i < cars.Length; i++)
    //     {
    //         sorted[cars[i].gridPosition - 1] = cars[i];
    //     }

    //     for (int i = 0; i < cars.Length; i++)
    //     {
    //         cars[i] = sorted[i];
    //     }
    // }

    private bool Check_LapCompleted(int _eventLap)
    {
        if (cars[0].currentLap > _eventLap)
            return true;

        return false;
    }

    private void Overtake_MiniGame()
    {

    }

    private void Show_EndRace_Info()
    {
        //Calculate xp for the player
        PlayerManager.Instance.GiveTeamXp(10);  //need to figure out how to calculate xp to be gained

        //Calculate money won
        PlayerManager.Instance.Money += 2;

        //TODO Effect moral based on the result of the race

        //Show end race stats
        raceDayHelper.Show_EndRace();
    }




    #region UI_CALLED_METHODS

    public void UI_StartRace()
    {
        raceDayHelper.UI_StartRace();
        StartCoroutine(RaceStart_MiniGame());
    }

    public void UI_LeaveRaceDay()
    {
        GameManager.Instance.SaveGame();

        GameManager.Instance.LoadScene_Async((int)SceneCodex.MANAGER);
    }

    #endregion
}
