using UnityEngine;
using System;

[Serializable]
public class Mission
{
    public string missionId;
    public MissionType missionType;
    public MissionStatus status;

    public string originPlanetId;
    public string targetPlanetId; // optional, for expedition can be "deep_space" or null

    public string fleetId; // reference to the fleet being used

    // time tracking (seconds since game start, or Time.time, or your own clock later)
    public double departTime;
    public double arriveTime;
    public double returnTime;

    public Mission(MissionType type, string originPlanetId, string targetPlanetId, string fleetId)
    {
        missionId = IdUtil.NewId("msn");
        missionType = type;
        status = MissionStatus.EnRoute;

        this.originPlanetId = originPlanetId;
        this.targetPlanetId = targetPlanetId;
        this.fleetId = fleetId;
    }
}