using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NpcPitCrewLeader : NpcLayout
{

    public NpcPitCrewMember[] pitCrew;


    #region Constructors

    public NpcPitCrewLeader() { Fill_Skill_Names(); }

    public NpcPitCrewLeader(NpcStruct _struct, int _contractValue)
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
        skills[(int)PIT_LEADER_SKILLS.LEADERSHIP].NAME = "LEADERSHIP";
        skills[(int)PIT_LEADER_SKILLS.CREW_COACHING].NAME = "CREW COACHING";
        skills[(int)PIT_LEADER_SKILLS.TECH_KNOWLEDGE].NAME = "TECH KNOWLEDGE";
        skills[(int)PIT_LEADER_SKILLS.CONSISTENCY].NAME = "CONSISTENCY";
    }

    #endregion
}
