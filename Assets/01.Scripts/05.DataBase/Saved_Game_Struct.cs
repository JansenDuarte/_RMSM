using System.Collections;

public struct Saved_Game_Struct
{
    public string Team_Name;
    public string Car_Color;
    public int Car_Number;
    public int Money;
    /// <summary>
    /// <b>ORDER:</b> w:m:yyyy
    /// </summary>
    public string Game_Date;
    /// <summary>
    /// <b>ORDER:</b> Engineer, Driver, PitCrewLeader, PitCrewMember x4
    /// </summary>
    public string Team_Members;
    public string Current_Country;

    public Saved_Game_Struct(string _teamName, string _carColor, int _carNumber, int _money, string _gameTime, string _teamMembers, string _currentCountry)
    {
        Team_Name = _teamName;
        Car_Color = _carColor;
        Car_Number = _carNumber;
        Money = _money;
        Game_Date = _gameTime;
        Team_Members = _teamMembers;
        Current_Country = _currentCountry;
    }
}
