using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RaceCar : MonoBehaviour
{
    private const float PERFORMANCE_FACTOR = 0.005f;
    private const float PERCENT = 100f;

    public SpriteRenderer sprite;
    public NpcDriver driver;

    public float trackPositionPerCent;

    public int startingGridPosition;
    public int gridPosition;
    public int currentLap = 0;

    public bool checkeredFlag = false;  //has the event ended?
    public bool raceCompleted = false;


    //Just for my memory's sake:
    //
    //  The car is calling the driver, because, the car knows where it is in the track,
    //  the driver tells the car what it wants it to do, and the car fucking tries its best
    public float Drive()
    {
        trackPositionPerCent += PERFORMANCE_FACTOR * driver.GetGeneralPerformance();
        if (trackPositionPerCent > PERCENT)
        {
            trackPositionPerCent -= PERCENT;
            if (!checkeredFlag)
                currentLap++;
            else
                raceCompleted = true;
        }
        return trackPositionPerCent;
    }

    //TEST  see if it fits
    public void OnPit() { }

    //TODO  See how to implement this
    public void BreakDown() { }
}
