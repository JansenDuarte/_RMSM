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

    public float Drive()
    {
        trackPositionPerCent += PERFORMANCE_FACTOR * driver.GetGeneralPerformance();
        trackPositionPerCent %= 1f;
        return trackPositionPerCent;
    }
}
