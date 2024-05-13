using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RaceCar : MonoBehaviour
{
    private const float PERFORMANCE_FACTOR = 0.0001f;

    public SpriteRenderer sprite;
    public NpcDriver driver;

    public float trackPositionPerCent;

    public int startingEventPosition;
    public int eventPosition;


    //Just for my memory's sake:
    //
    //  The car is calling the driver, because, the car knows where it is in the track,
    //  the driver tells the car what it wants it to do, and the car fucking tries its best
    public float Drive()
    {
        trackPositionPerCent += PERFORMANCE_FACTOR * driver.GetGeneralPerformance();
        trackPositionPerCent %= 1f;
        return trackPositionPerCent;
    }

    //TEST  see if it fits
    public void OnPit() { }

    //TODO  See how to implement this
    public void BreakDown() { }
}
