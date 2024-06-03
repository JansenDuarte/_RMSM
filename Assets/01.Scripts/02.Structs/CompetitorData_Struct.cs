using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompetitorData_Struct : MonoBehaviour
{
    public TextMeshProUGUI competitor_Name;
    public TextMeshProUGUI competitor_CarNumber_AndColor;

    public Image changedPositionIndicator;
    public Sprite[] indicators;

    public IEnumerator GainedPosition()
    {
        changedPositionIndicator.sprite = indicators[0];
        yield return new WaitForSeconds(1f);
        changedPositionIndicator.sprite = indicators[2];
        yield break;
    }

    public IEnumerator LostPosition()
    {
        changedPositionIndicator.sprite = indicators[1];
        yield return new WaitForSeconds(1f);
        changedPositionIndicator.sprite = indicators[2];
        yield break;
    }
}
