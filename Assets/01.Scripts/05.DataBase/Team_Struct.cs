using System.Collections;

public struct Team_Struct
{
    public NpcRaceEngineer engineer;
    public NpcDriver driver;
    public NpcPitCrewLeader leader;
    public NpcPitCrewMember[] crewMembers;

    public Team_Struct(NpcRaceEngineer _engineer, NpcDriver _driver, NpcPitCrewLeader _leader, NpcPitCrewMember[] _crewMembers)
    {
        engineer = _engineer;
        driver = _driver;
        leader = _leader;
        crewMembers = _crewMembers;
    }
}
