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
        npc_age.text = _npc.age.ToString();

        npc_countryFlag.sprite = ExM.Fetch_CountryFlag(_npc.country);
        GenerateRandom_NPCPhoto();

        for (int i = 0; i < skillBarStructs.Length; i++)
        {
            skillBarStructs[i].skill_name.text = _npc.skills[i].NAME;
            skillBarStructs[i].BarFill = _npc.skills[i].VALUE;
        }
    }

    private void GenerateRandom_NPCPhoto()
    {
        //HACK
        //FIXME: Some parts might be deppendent on sex, other might have additional changes
        photoComponents[0].color = PlayerManager.Instance.TeamColor;
        int skinToneIndex = Random.Range(0, skinTones.Length);
        photoComponents[1].color = skinTones[skinToneIndex];

        int facialDetailsIndex = Random.Range(0, facialDetails.Length + 1);
        if (facialDetailsIndex > facialDetails.Length)
            photoComponents[2].enabled = false;
        else
        {
            photoComponents[2].sprite = facialDetails[facialDetailsIndex];
            photoComponents[2].color = skinTones[skinToneIndex];
        }
        photoComponents[3].sprite = nosesAndMouths[Random.Range(0, nosesAndMouths.Length)];
        photoComponents[3].color = skinTones[skinToneIndex];
        photoComponents[4].sprite = eyesAndBrows[Random.Range(0, eyesAndBrows.Length)];
        photoComponents[5].sprite = hairs[Random.Range(0, hairs.Length)];
        photoComponents[6].sprite = facialHairs[Random.Range(0, facialHairs.Length)];

        int accessoriesIndex = Random.Range(0, accessories.Length + 1);
        if (accessoriesIndex > accessories.Length)
            photoComponents[7].enabled = false;
        else
            photoComponents[7].sprite = accessories[accessoriesIndex];
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
    [SerializeField] private Image[] photoComponents;
    [SerializeField] private Image npc_countryFlag;
    [SerializeField] private Color[] skinTones;
    [SerializeField] private Sprite[] facialDetails;
    [SerializeField] private Sprite[] nosesAndMouths;
    [SerializeField] private Sprite[] eyesAndBrows;
    [SerializeField] private Sprite[] hairs;
    [SerializeField] private Sprite[] facialHairs;
    [SerializeField] private Sprite[] accessories;

    [Header("Text Information")]
    [SerializeField] private TextMeshProUGUI npc_name;
    [SerializeField] private TextMeshProUGUI npc_country;
    [SerializeField] private TextMeshProUGUI npc_sex;
    [SerializeField] private TextMeshProUGUI npc_age;

    [Header("Skills Panel")]
    [SerializeField] private GameObject skillsPanel_GO;

    [Header("Skill Bar Behaviour")]
    [SerializeField] private UI_SkillBar[] skillBarStructs;

    #endregion // VARIABLES
}
