using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbsorbEffect.asset", menuName = "DataMon/Special Effect/Absorb")]
public class AbsorbEffect : SpecialEffect
{
    public enum Source {
        DamageDealt,
        DamageTaken,
        StaminaSpentByOpponent
    }

    public Stat statToBuff;
    public float amountToBuff;
    public Source source;

    public override void ModifyRolledAbility(BattleSide side, bool success, System.Random dice) {        
        SwitchToSelfEffect(side);
    }

    public override void ApplyPreResolveEffects(BattleSide side, Monster target, System.Random dice) {
        float change = 0f;

        switch (source) {
            case Source.DamageTaken:
                change = -side.opponent.effectOnOpponent.GetHealthChangeWithVulnerability(side.opponent, target);                
                break;
            case Source.DamageDealt:
                change = -side.effectOnOpponent.GetHealthChangeWithVulnerability(side, side.opponent.currentMonster);
                break;
            case Source.StaminaSpentByOpponent:
                change = side.opponent.staminaSpent;
                break;
        }

        change = amountToBuff * Mathf.Max(change, 0f);

        side.effectOnSelf.statChanges[statToBuff] += change;
    }
}
