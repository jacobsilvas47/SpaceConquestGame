using System;

[Serializable]
public class BattleResult
{
    public bool attackerWon;

    public int attackerPower;
    public int defenderPower;

    public float attackerLossPercent;
    public float defenderLossPercent;

    public BattleLosses attackerLosses = new BattleLosses();
    public BattleLosses defenderLosses = new BattleLosses();
}