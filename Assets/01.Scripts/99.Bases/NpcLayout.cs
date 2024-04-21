using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NpcStruct
{
    public string name;
    public string sex;
    public string country;
    public int age;
}

public enum Moral
{
    AWFUL = 0,
    BAD = 1,
    OK = 2,
    GOOD = 3,
    GREAT = 4
}

public abstract class NpcLayout
{
    #region CONSTANTS
    private const int TIMES_TO_SCRAMBLE = 4;

    private const int AGE_THRESHOLD = 30;

    private const int LEVELUP_SCALING = 10;
    #endregion // CONSTANTS

    #region VARIBLES

    /// <summary>
    /// ID of the Npc on the database, <b>IF</b> it is present
    /// </summary>
    public int dbId = -1;

    public string name;
    public string sex;
    public string country;
    public int age;

    public int level = 1;
    public int xp = 0;
    public int moralValue;           //Affects all other behaviours;

    private Moral moral;
    public Moral Moral { get { return moral; } }

    public int potencial;
    public int contractValue;

    public Skill_Struct[] skills = new Skill_Struct[4];

    #endregion // VARIBLES




    public abstract void Fill_Skill_Names();




    /// <summary>
    /// Retuns 4 values for the skills of the NPC using the contract value
    /// as the defining parameter
    /// </summary>
    /// <param name="_contractValue">contract value of the NPC</param>
    /// <returns></returns>
    public int[] Generate_SkillsByContractValue(int _contractValue)
    {
        int[] skills = { _contractValue, _contractValue, _contractValue, _contractValue };   //shittiest way; all values are equal;

        if (_contractValue == 100)  //max contract value; no reason to do the rest;
            return skills;

        int scrambleRNG = Random.Range(0, TIMES_TO_SCRAMBLE);   //scramble the values this ammount of times

        //scramble
        for (int i = 0; i < scrambleRNG; i++)
        {
            int scrambleFactor = Random.Range(0, skills[i] / 2);

            skills[i] -= scrambleFactor;
            skills[Random.Range(i + 1, skills.Length)] += scrambleFactor;
        }

        //values cannot be higher than 100
        for (int i = 0; i < skills.Length; i++)
        {
            //cut the value below 100
            if (skills[i] > 100)
                skills[i] = 100;
        }

        return skills;
    }




    /// <summary>
    /// Ages the NPC and returns if it can lose some skills due to aging
    /// </summary>
    /// <returns> If <b>TRUE</b> lose skill point due to aging</returns>
    public bool GetOlder()
    {
        age++;

        if (age > AGE_THRESHOLD)
        {
            float chance = Mathf.Pow(100f, age) / 25f;       //Function found by plotting the data; This gives a good curve

            if (Random.Range(0f, 1f) <= chance)
                return true;
        }

        return false;
    }



    /// <summary>
    /// Adds XP and checks if a level up is available.
    /// <b>WILL CONVERT NEGATIVE XP INTO POSITIVE</b>
    /// </summary>
    /// <param name="_addedXP"> XP to be added</param>
    /// <returns><b>TRUE</b> if NPC leveled up</returns>
    public bool GainXP(int _addedXP)
    {
        xp += Mathf.Abs(_addedXP);

        if (xp >= level * LEVELUP_SCALING)
        {
            xp = 0;
            level++;
            return true;
        }

        return false;
    }


    //FIXME: this needs to be refactored
    public void EffectMoral(int _effect)
    {
        int _prevMoral = moralValue;
        moralValue += _effect;

        int[] _lowerLimits = new int[] { 19, 39, 59, 79 };
        int[] _upperLimits = new int[] { 80, 60, 40, 20 };

        //Verify if the moral passes through a threshold
        if (_effect > 0)
        {
            //HACK this might become a problem
            if (CheckThreshold(_prevMoral, moralValue, _lowerLimits))
                moral += 1;
        }
        else
        {
            //HACK this might become a problem
            if (CheckThreshold(_prevMoral, moralValue, _upperLimits))
                moral -= 1;
        }

    }


    //FIXME: this needs to be refactored
    /// <summary>
    /// Verifies if any of the thresholds have been surpassed
    /// </summary>
    /// <param name="_previousValue"> Previous value, before the change </param>
    /// <param name="_currentValue"> Current value, already changed </param>
    /// <param name="_limit"> Array of the limits to be tested </param>
    /// <returns></returns>
    private bool CheckThreshold(int _previousValue, int _currentValue, int[] _limit)
    {
        if (_previousValue - _currentValue > 0)
        {
            for (int i = 0; i < _limit.Length; i++)
            {
                if (_previousValue <= _limit[i] && _currentValue > _limit[i])
                {
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < _limit.Length; i++)
            {
                if (_previousValue >= _limit[i] && _currentValue < _limit[i])
                {
                    return true;
                }
            }
        }

        return false;
    }
}