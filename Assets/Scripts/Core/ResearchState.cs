using System;
using System.Collections.Generic;

[Serializable]
public class ResearchState
{
    public Dictionary<ResearchType, int> levels = new Dictionary<ResearchType, int>();

    public int GetLevel(ResearchType type)
    {
        return levels.TryGetValue(type, out int lvl) ? lvl : 0;
    }

    public void IncreaseLevel(ResearchType type)
    {
        if (!levels.ContainsKey(type))
            levels[type] = 0;

        levels[type]++;
    }
}