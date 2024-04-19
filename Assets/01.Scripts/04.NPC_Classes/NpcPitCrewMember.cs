using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NpcPitCrewMember : NpcLayout
{


    #region Constructors

    public NpcPitCrewMember() { Fill_Skill_Names(); }

    public NpcPitCrewMember(NpcStruct _struct, int _contractValue)
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

    public override void Fill_Skill_Names()
    {
        skills[(int)PIT_MEMBER_SKILLS.SPEED].NAME = "SPEED";
        skills[(int)PIT_MEMBER_SKILLS.CONSISTENCY].NAME = "CONSISTENCY";
        skills[(int)PIT_MEMBER_SKILLS.TEAM_WORK].NAME = "TEAM WORK";
        skills[(int)PIT_MEMBER_SKILLS.STAMINA].NAME = "STAMINA";
    }

    //TODO      Make the performance calculator for the crew members

    #endregion
}
