using UnityEngine;

public class TrackGenerator : MonoBehaviour
{

    public GameObject trackGO;
    public GameObject pointPrefabGO;


    public float pointPosLimit;






    //TODO: this needs to be set previously
    public static string GenerateWeather()
    {
        string rngWeather = string.Empty;

        switch (Random.Range(0, 3))
        {
            case 0:
                rngWeather = "Clear ";
                break;
            case 1:
                rngWeather = "Cloudy ";
                break;
            case 2:
                rngWeather = "Rainy ";
                break;
        }

        rngWeather += (Random.Range(0, 2) == 0) ? "Day" : "Night";

        return rngWeather;
    }
}
