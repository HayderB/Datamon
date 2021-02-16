using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInterferenceStatusEffect.asset", menuName = "DataMon/Status Effect/Interference")]
public class InterferenceStatusEffect : StatusEffect
{
    public float chanceOfNoAction;
    public float chanceToAttackSelf;

    public override bool TryModifyRolledAbilities(BattleSide side, System.Random dice) {
        float roll = (float)dice.NextDouble();

        if (roll < chanceOfNoAction) {
            side.effectOnOpponent = default;
            side.effectOnSelf = default;
            return true;
        }

        roll -= chanceOfNoAction;

        if (roll < chanceToAttackSelf) {
            side.effectOnSelf = side.effectOnOpponent;
            side.effectOnOpponent = default;
            return true;
        }

        return false;
    }
}
