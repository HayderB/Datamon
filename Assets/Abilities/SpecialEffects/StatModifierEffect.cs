using UnityEngine;

[CreateAssetMenu(fileName ="NewStatModifierEffect.asset", menuName = "DataMon/Special Effect/Stat Modifier")]
public class StatModifierEffect : SpecialEffect
{
    public Stat statToModify;
    public float changeRatio;
    public int changeAbsolute;
    public bool applyToSelf;

    public override void ModifyRolledAbility(BattleSide side, bool success, System.Random dice) {
        if (!success) return;

        if (applyToSelf) {
            side.effectOnSelf.statChanges[statToModify] = GetChangeAmount(side.currentMonster);
        } else {
            side.effectOnOpponent.statChanges[statToModify] = GetChangeAmount(side.opponent.currentMonster);
        }
    }

    float GetChangeAmount(Monster monster) {
        float change = changeAbsolute;
        change += monster.currentStats[statToModify] * changeRatio;
        return change;
    }
}
