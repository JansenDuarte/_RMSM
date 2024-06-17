using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExtensionMethods;

public class NPC_DataHelper : MonoBehaviour
{
    #region VARIABLES

    [Header("Panel as button"), Space]
    [SerializeField] private bool m_isSelectable = false;
    public bool Is_Selectable
    {
        get { return m_isSelectable; }
        set
        {
            m_buttonBehaviour.enabled = value;
            m_isSelectable = value;
        }
    }
    [SerializeField] private Button m_buttonBehaviour;

    [Header("Images & Sprites"), Space]
    [SerializeField] private Image[] m_photoComponents;
    [SerializeField] private Image m_countryFlag;
    [SerializeField] private Color[] m_skinTones;
    [SerializeField] private Sprite[] m_facialDetails;
    [SerializeField] private Sprite[] m_nosesAndMouths;
    [SerializeField] private Sprite[] m_eyesAndBrows;
    [SerializeField] private Sprite[] m_hairs;
    [SerializeField] private Sprite[] m_facialHairs;
    [SerializeField] private Sprite[] m_accessories;

    [Header("Text Information"), Space]
    [SerializeField] private TextMeshProUGUI m_name;
    [SerializeField] private TextMeshProUGUI m_country;
    [SerializeField] private TextMeshProUGUI m_sex;
    [SerializeField] private TextMeshProUGUI m_age;

    [Header("Skill Bar Behaviour"), Space]
    [SerializeField] private UI_SkillBar[] m_skillBars;

    #endregion // VARIABLES


    private void Awake() { m_buttonBehaviour.enabled = Is_Selectable; }

    private void FeedCardInfo(NpcLayout _npc)
    {
        m_name.text = _npc.name.ToUpper();
        m_sex.text = _npc.sex.ToUpper();
        m_country.text = _npc.country.ToUpper();
        m_age.text = _npc.age.ToString();

        m_countryFlag.sprite = ExM.Fetch_CountryFlag(_npc.country);
        GenerateRandomNpcPhoto();

        for (int i = 0; i < m_skillBars.Length; i++)
        {
            m_skillBars[i].skill_name.text = _npc.skills[i].NAME;
            m_skillBars[i].BarFill = _npc.skills[i].VALUE;
        }
    }

    private void GenerateRandomNpcPhoto()
    {
        //HACK
        //FIXME: Some parts might be deppendent on sex, other might have additional changes
        m_photoComponents[0].color = PlayerManager.Instance.TeamColor;
        int skinToneIndex = Random.Range(0, m_skinTones.Length);
        m_photoComponents[1].color = m_skinTones[skinToneIndex];

        int facialDetailsIndex = Random.Range(0, m_facialDetails.Length + 1);
        if (facialDetailsIndex == m_facialDetails.Length)
            m_photoComponents[2].enabled = false;
        else
        {
            m_photoComponents[2].sprite = m_facialDetails[facialDetailsIndex];
            m_photoComponents[2].color = m_skinTones[skinToneIndex];
        }
        m_photoComponents[3].sprite = m_nosesAndMouths[Random.Range(0, m_nosesAndMouths.Length)];
        m_photoComponents[3].color = m_skinTones[skinToneIndex];
        m_photoComponents[4].sprite = m_eyesAndBrows[Random.Range(0, m_eyesAndBrows.Length)];
        m_photoComponents[5].sprite = m_hairs[Random.Range(0, m_hairs.Length)];
        m_photoComponents[6].sprite = m_facialHairs[Random.Range(0, m_facialHairs.Length)];

        int accessoriesIndex = Random.Range(0, m_accessories.Length + 1);
        if (accessoriesIndex == m_accessories.Length)
            m_photoComponents[7].enabled = false;
        else
            m_photoComponents[7].sprite = m_accessories[accessoriesIndex];
    }



    public void Show_Npc_Card(ref NpcRaceEngineer _engineer)
    {
        NpcLayout l = _engineer;
        FeedCardInfo(l);
    }

    public void Show_Npc_Card(ref NpcDriver _driver)
    {
        NpcLayout l = _driver;
        FeedCardInfo(l);
    }

    public void Show_Npc_Card(ref NpcPitCrewLeader _crewLeader)
    {
        NpcLayout l = _crewLeader;
        FeedCardInfo(l);
    }

    public void Show_Npc_Card(ref NpcPitCrewMember _crewMember)
    {
        NpcLayout l = _crewMember;
        FeedCardInfo(l);
    }
}
