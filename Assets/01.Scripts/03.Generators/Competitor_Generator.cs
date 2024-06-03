using System.Collections.Generic;
using UnityEngine;

public static class Competitor_Generator
{
    public static List<NpcDriver> GenerateCompetitors(int _ammount, int _difficulty)
    {
        List<NpcDriver> competitorList = new();

        NpcDriver npcDriver;
        NpcStruct[] layout = new NpcStruct[_ammount];
        GameManager.Instance.NpcGenerateInBulk(ref layout);
        int[] skills;

        for (int i = 0; i < _ammount; i++)
        {
            npcDriver = new()
            {
                name = layout[i].name,
                country = layout[i].country,
                sex = layout[i].sex,
                age = layout[i].age,

                //Debuging
                carNumber = Random.Range(0, 100)
            };
            skills = npcDriver.Generate_SkillsByContractValue(Random.Range(10, 16) * _difficulty);
            for (int j = 0; j < npcDriver.skills.Length; j++)
            {
                npcDriver.skills[j].VALUE = skills[j];
            }
            //Debuging//

            competitorList.Add(npcDriver);
        }

        return competitorList;
    }
}