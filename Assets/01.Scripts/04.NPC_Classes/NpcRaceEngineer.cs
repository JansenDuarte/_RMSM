using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NpcRaceEngineer : NpcLayout
{




    #region CONSTRUCTORS

    public NpcRaceEngineer() { Fill_Skill_Names(); }

    public NpcRaceEngineer(NpcStruct _struct, int _contractValue)
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

    public override void Fill_Skill_Names()
    {
        skills[(int)ENGINEER_SKILLS.RACE_KNOWLEDGE].NAME = "RACE KNOWLEDGE";
        skills[(int)ENGINEER_SKILLS.RACE_STRATEGY].NAME = "RACE STRATEGY";
        skills[(int)ENGINEER_SKILLS.CAR_KNOWLEDGE].NAME = "CAR KNOWLEDGE";
        skills[(int)ENGINEER_SKILLS.DRIVER_COACHING].NAME = "DRIVER COACHING";
    }


    #endregion

    //TODO      See if there's need to make a performance calculator for the race engineer

}
