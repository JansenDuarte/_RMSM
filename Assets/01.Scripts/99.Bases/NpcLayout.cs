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

    public const int LEVELUP_SCALING = 10;
    public const int LOWER_SKILL_LIMIT = 1;
    public const int UPPER_SKILL_LIMIT = 100;
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

        //values cannot be higher than the upper limit
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] > UPPER_SKILL_LIMIT)
            {
                //cut the value to the upper limit
                skills[i] = UPPER_SKILL_LIMIT;
            }
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
        //FIXME there is a bug here. xp is not going over the first level up. Try doing it recursevely
        xp += Mathf.Abs(_addedXP);

        if (xp >= level * LEVELUP_SCALING)
        {
            xp = 0;
            level++;
            return true;
        }

        return false;
    }


    public void EffectMoral(int _effect)
    {
        int prevMoral = moralValue;
        moralValue += _effect;

        if (Check_ThresholdAndChangeMoral(prevMoral, _effect))
        {
            //TODO: trigger moral change!
        }

        //Block it from going over 100 and under 0
        if (_effect > 0)
            moralValue = (moralValue > 100) ? 100 : moralValue;
        else
            moralValue = (moralValue < 0) ? 0 : moralValue;
    }


    /// <summary>
    /// Verifies if any of the thresholds have been surpassed and changes the 'moral' enum
    /// </summary>
    /// <param name="_previousValue"> Value before the change </param>
    /// <param name="_variation"></param>
    /// <returns></returns>
    private bool Check_ThresholdAndChangeMoral(int _previousValue, int _variation)
    {
        int[] limits = new int[] { 20, 40, 60, 80 };    //Thresholds based on 5 separations of the Moral values

        for (int i = 0; i < limits.Length; i++)
        {
            if (_previousValue < limits[i] && _previousValue + _variation > limits[i])
            {
                moral += 1;
                return true;
            }
            else if (_previousValue > limits[i] && _previousValue + _variation < limits[i])
            {
                moral -= 1;
                return true;
            }
        }

        return false;
    }
}