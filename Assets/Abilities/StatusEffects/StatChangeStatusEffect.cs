using UnityEngine;

[CreateAssetMenu(fileName = "NewStatChangeStatusEffect.asset", menuName ="DataMon/Status Effect/Stat Change")]
public class StatChangeStatusEffect : StatusEffect
{
    public Stat statToChange = Stat.Health;
    public float changeRatio;
    public float changeAbsolute;
    public bool repeatEveryRound = true;

    void ChangeStat(Monster target) {
        float change = changeAbsolute;
        change += target.currentStats[statToChange] * changeRatio;

        target.currentStats[statToChange] += change;
    }

    public override void ApplyTo(Monster target) {
        base.ApplyTo(target);

        if (!repeatEveryRound)
            ChangeStat(target);
    }

    public override bool TryPersistAtEndOfRound(BattleSide side, System.Random dice) {
        if (!base.TryPersistAtEndOfRound(side, dice))
            return false;

        if (repeatEveryRound) {
            ChangeStat(side.currentMonster);
        }

        return true;
    }
}
