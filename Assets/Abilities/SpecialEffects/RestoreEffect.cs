using UnityEngine;

[CreateAssetMenu(fileName = "NewRestoreEffect.asset", menuName = "DataMon/Special Effect/Restore")]
public class RestoreEffect : SpecialEffect
{
    public Stat[] statsToRestore;
    public bool clearStatusEffect;

    public override void ApplyPostResolveEffects(BattleSide side, Monster target, System.Random dice) {
        if (clearStatusEffect && target.status != null)
            target.status.RemoveFrom(target);

        foreach(var stat in statsToRestore) {
            target.currentStats[stat] = target.species.baseStats[stat];
        }
    }
}
