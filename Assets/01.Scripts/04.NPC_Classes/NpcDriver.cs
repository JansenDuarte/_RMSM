using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NpcDriver : NpcLayout
{
    public int carNumber;


    #region CONSTRUCTORS

    public NpcDriver() { Fill_Skill_Names(); }

    public NpcDriver(NpcStruct _struct, int _contractValue)
    {
        Fill_Skill_Names();

        name = _struct.name;
        age = _struct.age;
        country = _struct.country;
        sex = _struct.sex;

        int[] values = Return_Skills_BasedOn_ContractValue(_contractValue);

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
    ///     - General Performance determines how well the driver is going to
    ///     drive during 1 lap.
    /// </summary>
    /// <returns></returns>
    public int GetGeneralPerformance()
    {
        int _performance;

        int _variationGradient = Mathf.CeilToInt((-(skills[(int)DRIVER_SKILLS.CONSISTENCY].VALUE * skills[(int)DRIVER_SKILLS.CONSISTENCY].VALUE) / 100) + 100);
        _variationGradient /= 10;

        float _consistencyVariation;

        //Consistency; The closer to 100, smaller the variations;
        if (Random.Range(0, 100) > skills[(int)DRIVER_SKILLS.CONSISTENCY].VALUE)
        {
            //  Driver was not consistent in this lap;
            //  Lap variation is big

            _consistencyVariation = Random.Range(-_variationGradient * 2, _variationGradient * 2);
        }
        else
        {
            //  Driver was consistent in this lap;
            //  Lap variation is small

            _consistencyVariation = Random.Range(-_variationGradient, _variationGradient);
        }

        // Determine performance:
        _performance = (int)(skills[(int)DRIVER_SKILLS.SPEED].VALUE + (((100 - skills[(int)DRIVER_SKILLS.STAMINA].VALUE) / 10) + _consistencyVariation));

        Debug.Log(string.Format("Performance Calculations:\n" +
            "speed : {0}\n stamina : {1}\n consistency : {2}\n consistency variation : {3}\n consistency gradient : {4}\n performance : {5}",
            skills[(int)DRIVER_SKILLS.SPEED].VALUE, skills[(int)DRIVER_SKILLS.STAMINA].VALUE, skills[(int)DRIVER_SKILLS.CONSISTENCY].VALUE, _consistencyVariation, _variationGradient, _performance));

        return _performance;
    }
}
