using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Copycat.asset", menuName = "DataMon/Special Effect/Copycat")]
public class CopycatEffect : MutualModifierEffect
{
    public override void ModifyRolledAbility(BattleSide side, bool success, System.Random dice) {
        SwitchToSelfEffect(side);
    }

    public override void OnPostAbilitiesRolled(BattleSide side, Monster target, System.Random dice) {
        // No-op if both monsters use Copycat on each other. Nuke the matching effect.
        if (side.opponent.effectOnSelf.successfulSpecial is CopycatEffect) {
            side.opponent.effectOnSelf.successfulSpecial = null;
            return;
        }

        // Roll the opponent's ability as our own, but keep our stamina cost.
        side.opponent.selectedAbility.Roll(side, dice);
    }
}
