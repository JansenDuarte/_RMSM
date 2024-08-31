using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NpcDriver : NpcLayout
{
    public int carNumber;

    public const int VARIATION_FACTOR = 2;


    #region CONSTRUCTORS

    public NpcDriver() { Fill_Skill_Names(); }

    public NpcDriver(NpcStruct _struct, int _contractValue)
    {
        Fill_Skill_Names();

        name = _struct.name;
        age = _struct.age;
        country = _struct.country;
        sex = _struct.sex;

        int[] values = Generate_SkillsByContractValue(_contractValue);

        List<int> list = new();
        list.AddRange(values);
        int listIndex;

        for (int i = 0; i < values.Length; i++)
        {
            listIndex = Random.Range(0, list.Count);
            skills[i].VALUE = list[listIndex];
            list.RemoveAt(listIndex);
        }
    }

    override public void Fill_Skill_Names()
    {
        skills[(int)DRIVER_SKILLS.STAMINA].NAME = "STAMINA";
        skills[(int)DRIVER_SKILLS.SPEED].NAME = "SPEED";
        skills[(int)DRIVER_SKILLS.TECHNIQUE].NAME = "TECHNIQUE";
        skills[(int)DRIVER_SKILLS.CONSISTENCY].NAME = "CONSISTENCY";
    }


    #endregion

    /// <summary>
    ///     - General Performance determines if the driver is prone to making mistakes
    /// </summary>
    /// <returns></returns>
    public int GetGeneralPerformance()
    {
        int performance;

        int consistencyVariation;

        //INFO:     This is the calculation that relates to a good graph for the variation of performance
        int variationGradient = Mathf.CeilToInt((-(skills[(int)DRIVER_SKILLS.CONSISTENCY].VALUE * skills[(int)DRIVER_SKILLS.CONSISTENCY].VALUE) / 100) + 10);

        //Consistency; The closer to 100, smaller the variations;

        //  Driver was not consistent in this lap;
        if (Random.Range(LOWER_SKILL_LIMIT, UPPER_SKILL_LIMIT + 1) > skills[(int)DRIVER_SKILLS.CONSISTENCY].VALUE)
            consistencyVariation = Random.Range(-variationGradient * VARIATION_FACTOR, variationGradient * VARIATION_FACTOR);

        //  Driver was consistent in this lap;
        else
            consistencyVariation = Random.Range(-variationGradient, variationGradient);

        // Determine performance:
        performance = skills[(int)DRIVER_SKILLS.SPEED].VALUE + ((100 - skills[(int)DRIVER_SKILLS.STAMINA].VALUE) / 10) + consistencyVariation;

        Debug.Log(string.Format("Performance Calculations:\n" +
            "speed : {0}\n stamina : {1}\n consistency : {2}\n consistency variation : {3}\n consistency gradient : {4}\n performance : {5}",
            skills[(int)DRIVER_SKILLS.SPEED].VALUE, skills[(int)DRIVER_SKILLS.STAMINA].VALUE, skills[(int)DRIVER_SKILLS.CONSISTENCY].VALUE, consistencyVariation, variationGradient, performance));

        return performance;
    }

    public float GetReactionTime()
    {
        float retValue = 0f;

        //TODO:     Make the calculation for the random reaction time

        return retValue;
    }
}
