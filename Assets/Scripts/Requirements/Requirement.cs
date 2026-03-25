using System;

[Serializable]
public class Requirement
{
    public RequirementType type;

    public BuildingType buildingType;
    public ResearchType researchType;

    public int level;
}