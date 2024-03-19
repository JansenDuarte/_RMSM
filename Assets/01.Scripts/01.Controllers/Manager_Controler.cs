using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager_Controler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gameDate_Text;

    [SerializeField] TextMeshProUGUI teamName_Text;
    [SerializeField] TextMeshProUGUI money_Text;

    [SerializeField] GameObject teamMembers_pivot;

    private void Start()
    {
        //Initial setup
        gameDate_Text.text = string.Format("week {0}/ {1}/ {2}", GameManager.Instance.gameDate.Week, GameManager.Instance.gameDate.Month.ToString(), GameManager.Instance.gameDate.Year);

        teamName_Text.text = PlayerManager.Instance.TeamName;
        teamName_Text.color = PlayerManager.Instance.TeamColor;
        money_Text.text = string.Format("BO$ {0}", PlayerManager.Instance.Money);
    }


    public void UI_Show_TeamMembers()
    {
        //TODO
        //prepare team info

        //show / animate panel change
    }
}
