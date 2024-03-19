using System.Collections.Generic;
using UnityEngine;

public static class Competitor_Generator
{
    public static List<NpcDriver> GenerateCompetitors(int _ammount, int _difficulty)
    {
        List<NpcDriver> competitorList = new();

        NpcDriver npcDriver;
        NpcStruct[] layout = new NpcStruct[_ammount];
        GameManager.Instance.Generate_NPC_InBulk(ref layout);

        for (int i = 0; i < _ammount; i++)
        {
            npcDriver = new();
            npcDriver.name = layout[i].name;
            npcDriver.country = layout[i].country;
            npcDriver.sex = layout[i].sex;
            npcDriver.age = layout[i].age;

            //Debuging
            npcDriver.carNumber = Random.Range(0, 100);
            npcDriver.skills[0].VALUE = Random.Range(1, 11) * _difficulty;
            npcDriver.skills[0].VALUE = Random.Range(1, 11) * _difficulty;
            npcDriver.skills[0].VALUE = Random.Range(1, 11) * _difficulty;
            npcDriver.skills[0].VALUE = Random.Range(1, 11) * _difficulty;
            //Debuging//

            competitorList.Add(npcDriver);
            //Debug.Log(i);
        }

        return competitorList;
    }
}