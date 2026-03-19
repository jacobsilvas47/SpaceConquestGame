using System;

[Serializable]
public class MissionReport
{
    public string reportId;
    public MissionType missionType;

    public string title;
    public string summary;
    public string details;

    public string originPlanetId;
    public string targetId;
    public string targetDisplayName;

    public double createdAtGameTime;

    public bool attackerWon;
    public bool wasRead;

    public MissionReport(
        MissionType missionType,
        string title,
        string summary,
        string details,
        string originPlanetId,
        string targetId,
        string targetDisplayName,
        double createdAtGameTime,
        bool attackerWon = false)
    {
        reportId = Guid.NewGuid().ToString();
        this.missionType = missionType;
        this.title = title;
        this.summary = summary;
        this.details = details;
        this.originPlanetId = originPlanetId;
        this.targetId = targetId;
        this.targetDisplayName = targetDisplayName;
        this.createdAtGameTime = createdAtGameTime;
        this.attackerWon = attackerWon;
        this.wasRead = false;
    }
}