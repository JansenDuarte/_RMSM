
public struct Team_Struct
{
    public NpcRaceEngineer Engineer;
    public NpcDriver Driver;
    public NpcPitCrewLeader CrewLeader;
    public NpcPitCrewMember[] CrewMembers;

    public Team_Struct(NpcRaceEngineer _engineer, NpcDriver _driver, NpcPitCrewLeader _leader, NpcPitCrewMember[] _crewMembers)
    {
        Engineer = _engineer;
        Driver = _driver;
        CrewLeader = _leader;
        CrewMembers = _crewMembers;
    }
}
