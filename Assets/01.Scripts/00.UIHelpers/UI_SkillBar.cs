using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExtensionMethods;

public class UI_SkillBar : MonoBehaviour
{
    public TextMeshProUGUI skill_name;
    [SerializeField] private Image skillBarFiller;

    private float p_barFill;


    /// <summary>
    /// Ammount to fill the bar image. Any value is accepted, it'll get corrected internaly
    /// </summary>
    public float BarFill
    {
        set
        {
            if (value < 0f)
                value = 0f;

            if (value > 1f)
                value.Change_Range(0f, value, 0f, 1f);

            p_barFill = value;
            StartCoroutine(ChangeBarFill());
        }
    }


    private IEnumerator ChangeBarFill()
    {
        //Lerp speed
        float _s = 0.01f;
        float _percent = 0f;

        //Animate
        while (skillBarFiller.fillAmount != p_barFill)
        {
            skillBarFiller.fillAmount = Mathf.Lerp(skillBarFiller.fillAmount, p_barFill, _percent);
            _percent += _s;

            yield return new WaitForEndOfFrame();
        }

        if (p_barFill == 1f)
            Debug.Log("Animate level up / Max skill");

        yield break;
    }
}
