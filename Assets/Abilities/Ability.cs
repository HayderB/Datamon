using UnityEngine;

[CreateAssetMenu(fileName ="NewAbility.asset", menuName ="DataMon/Ability")]
public class Ability : ScriptableObject
{
    public Element element;
    public int staminaCost;
    public int minDamage = 0;
    public int maxDamage = 0;
    public SpecialEffect special;
    public float specialChance = 1f;

    public float GetDamage(Monster monster, System.Random dice) {
        float bellCurve = (float)(dice.NextDouble() + dice.NextDouble() + dice.NextDouble()) / 3.0f;
        float baseDamage = minDamage + bellCurve * (maxDamage - minDamage) / 3.0f;
        return baseDamage * monster.currentStats.attack / 100f;
    }

    public bool GetSpecialSuccess(Monster monster, System.Random dice) {
        return dice.NextDouble() < specialChance * monster.currentStats.luck / 100f;
    }

    public void Roll(BattleSide side, System.Random dice) {
        side.effectOnOpponent = default;
        side.effectOnSelf = default;

        side.selectedAbility = this;
        side.effectOnOpponent.statChanges.health = -GetDamage(side.currentMonster, dice);              

        if (special != null) {
            bool success = GetSpecialSuccess(side.currentMonster, dice);

            if (success)
                side.effectOnOpponent.successfulSpecial = special;

            special.ModifyRolledAbility(side, success, dice);
        }
    }
}
