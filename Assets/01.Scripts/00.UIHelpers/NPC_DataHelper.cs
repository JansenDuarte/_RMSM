using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExtensionMethods;

public class NPC_DataHelper : MonoBehaviour
{
    private void Awake() { buttonBehaviour.enabled = IsSelectable; }

    private void Feed_CardInfo(NpcLayout _npc)
    {
        npc_name.text = _npc.name.ToUpper();
        npc_sex.text = _npc.sex.ToUpper();
        npc_country.text = _npc.country.ToUpper();

        npc_countryFlag.sprite = ExM.Fetch_CountryFlag(_npc.country);

        npc_age.text = _npc.age.ToString();

        for (int i = 0; i < skillBarStruct_Array.Length; i++)
        {
            skillBarStruct_Array[i].skill_name.text = _npc.skills[i].NAME;
            skillBarStruct_Array[i].BarFill = _npc.skills[i].VALUE;
        }
    }



    public void ShowNpcCard(ref NpcRaceEngineer _engineer)
    {
        NpcLayout l = _engineer;
        Feed_CardInfo(l);
    }

    public void ShowNpcCard(ref NpcDriver _driver)
    {
        NpcLayout l = _driver;
        Feed_CardInfo(l);
    }

    public void ShowNpcCard(ref NpcPitCrewLeader _crewLeader)
    {
        NpcLayout l = _crewLeader;
        Feed_CardInfo(l);
    }

    public void ShowNpcCard(ref NpcPitCrewMember _crewMember)
    {
        NpcLayout l = _crewMember;
        Feed_CardInfo(l);
    }


    #region VARIABLES

    [Header("Panel as button")]
    [SerializeField] private bool m_isSelectable = false;
    public bool IsSelectable
    {
        get { return m_isSelectable; }
        set
        {
            buttonBehaviour.enabled = value;
            m_isSelectable = value;
        }
    }
    [SerializeField] private Button buttonBehaviour;

    [Header("Images & Sprites")]
    [SerializeField] private Image npc_photo;
    [SerializeField] private Image npc_countryFlag;

    [Header("Text Information")]
    [SerializeField] private TextMeshProUGUI npc_name;
    [SerializeField] private TextMeshProUGUI npc_country;
    [SerializeField] private TextMeshProUGUI npc_sex;
    [SerializeField] private TextMeshProUGUI npc_age;

    [Header("Skills Panel")]
    [SerializeField] private GameObject skillsPanel_GO;

    [Header("Skill Bar Behaviour")]
    [SerializeField] private UI_SkillBar[] skillBarStruct_Array;

    #endregion // VARIABLES
}
