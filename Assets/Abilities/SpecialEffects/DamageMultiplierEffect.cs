using UnityEngine;

[CreateAssetMenu(fileName ="NewDamageMultiplierEffect.asset", menuName ="DataMon/Special Effect/Damage Multiplier")]
public class DamageMultiplierEffect : SpecialEffect
{
    public float damageMultiplierOnFail;
    public float damageMultiplierOnSuccess;

    public override void ModifyRolledAbility(BattleSide side, bool success, System.Random dice) {
        if (success) {
            side.effectOnOpponent.statChanges.health *= damageMultiplierOnSuccess;
        } else {
            side.effectOnOpponent.statChanges.health *= damageMultiplierOnFail;
        }
    }
}
