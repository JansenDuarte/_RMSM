using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class RaceDay_Controller : MonoBehaviour
{
    #region CONSTANTS

    private const int COMPETITOR_AMMOUNT = 7;
    public const float GRID_DIFF_FACTOR = 0.015f;
    public const float DISTANCE_CONVERSION_FACTOR = 100f;

    #endregion  //CONSTANTS

    public RaceDayHelper raceDayHelper;
    public RaceCar[] cars;
    private RaceCar playerCar;
    private Track track;
    private int laps = 1;
    private float newTrackPosition;
    private Vector3 targetPosition;


    void Start()
    {
        //DEBUG This needs to call the track that is beeing raced on
        DBConnector.Instance.Load_Track(out track, 1);

        PrepareRacersStatsAndSprites();

        RandomizeStartingGrid();

        SortCars();

        //Show Layout
        raceDayHelper.PrepareRaceDayInfo(ref cars, ref track);
    }

    private void PrepareRacersStatsAndSprites()
    {
        NpcDriver[] competitors = Competitor_Generator.GenerateCompetitors(COMPETITOR_AMMOUNT, 1).ToArray();

        cars[0].driver = PlayerManager.Instance.Driver;
        cars[0].sprite.color = PlayerManager.Instance.TeamColor;

        playerCar = cars[0];

        //Set up all the competitors
        for (int i = 0; i < competitors.Length; i++)
        {
            cars[i + 1].driver = competitors[i];
            cars[i + 1].sprite.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f);
        }
    }

    private IEnumerator RaceStart_MiniGame()
    {
        bool didFalseStart = false;

        //Call UI Helper to show the race start mini game
        raceDayHelper.Show_MinigameLayout(MiniGameCodex.RACE_START);

        //wait for click to begin
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) == true);
        raceDayHelper.Hide_ClutchText();

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

                //TODO Apply penalties
            }

        }

        //get the time when lights finish
        float lightsOffTime = Time.time;

        //wait for clutch click release
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Mouse0) == true || didFalseStart);
        float clutchOffTime = Time.time;

        //compare times
        float reactionTime = clutchOffTime - lightsOffTime;
        Debug.Log(string.Format("Reaction time: {0} ms", reactionTime));

        //show results
        playerCar.BonusPerformance = EvaluateReactionTime(reactionTime);

        raceDayHelper.HideActiveMinigame();

        StartCoroutine(SimulateRace());

        yield break;
    }

    private float EvaluateReactionTime(float _reactionTime)
    {
        float bonus = -1f;

        //Do stuff
        if (_reactionTime < 0f)
        {
            //False start
            return bonus;
        }

        //min reaction time = 0.2s, or 200ms
        //max reaction time = positiveInfinity

        //bonus should be 1 if the reaction time is 0.2
        //bonus should get closer to 0 the further you are from the common avg. of 250ms

        return bonus;
    }


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

    private void SortCars()
    {
        RaceCar[] sorted = new RaceCar[8];
        foreach (RaceCar raceCar in cars)
        {
            sorted[raceCar.startingGridPosition - 1] = raceCar;
        }

        for (int i = 0; i < cars.Length; i++)
        { cars[i] = sorted[i]; }
    }


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
        int xpGained = 10;
        int moneyGained = 2;

        PlayerManager.Instance.xpGained = xpGained;
        PlayerManager.Instance.moneyGained = moneyGained;

        //Show end race stats
        raceDayHelper.Show_EndRace(ref cars, moneyGained, xpGained);


        //TODO Effect moral based on the result of the race
    }


    private void RandomizeStartingGrid()
    {
        List<int> availablePositions = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
        int rng = -1;

        foreach (RaceCar raceCar in cars)
        {
            rng = Random.Range(0, availablePositions.Count);

            raceCar.startingGridPosition = availablePositions[rng];
            raceCar.gridPosition = raceCar.startingGridPosition;
            availablePositions.RemoveAt(rng);

            //Position the cars
            //FIXME: some of the cars are starting on top of eachother. this needs to be verifyied with different tracks
            float gridPosition = 1f - (GRID_DIFF_FACTOR * raceCar.startingGridPosition);
            raceCar.trackPositionPerCent = gridPosition * DISTANCE_CONVERSION_FACTOR;
            raceCar.transform.position = track.curve.GetPointAt(gridPosition);
        }
    }


    #region UI_CALLED_METHODS

    public void UI_StartRace()
    {
        raceDayHelper.UI_StartRace();
        StartCoroutine(RaceStart_MiniGame());
    }

    public void UI_LeaveRaceDay()
    {
        //Calculate xp for the player
        PlayerManager.Instance.GiveTeamXp();  //need to figure out how to calculate xp to be gained
        //Calculate money won
        PlayerManager.Instance.GainMoney();

        GameManager.Instance.SaveGame();

        GameManager.Instance.LoadScene_Async((int)SceneCodex.MANAGER);
    }

    #endregion
}
