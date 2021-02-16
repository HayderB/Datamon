using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRedirectEffect.asset", menuName = "DataMon/Special Effect/Redirect")]
public class RedirectEffect : MutualModifierEffect
{
    public enum Behaviour {
        Normal,
        Block,
        Reflect
    }

    public Behaviour impactOnHealth;
    public Behaviour impactOnStamina;
    public Behaviour impactOnAttack;
    public Behaviour impactOnDefense;
    public Behaviour impactOnLuck;
    public Behaviour specialImpacts;

    public Behaviour ImpactOnStat(Stat stat) {
            switch(stat) {
                case Stat.Health:   return impactOnHealth;
                case Stat.Stamina:  return impactOnStamina;
                case Stat.Attack:   return impactOnAttack;
                case Stat.Defense:  return impactOnDefense;
                case Stat.Luck:     return impactOnLuck;
                default: return Behaviour.Normal;
            }
    }

    public override void ModifyRolledAbility(BattleSide side, bool success, System.Random dice) {
        SwitchToSelfEffect(side);
    }

    public override void OnPostAbilitiesRolled(BattleSide side, Monster target, System.Random dice) {
        // Can't redirect effects on yourself.
        if (target == side.currentMonster) return;

        foreach(var stat in Monster.STATS) {
            switch(ImpactOnStat(stat)) {
                case Behaviour.Block:
                    side.opponent.effectOnOpponent.statChanges[stat] = 0;
                    break;
                case Behaviour.Reflect:
                    side.opponent.effectOnSelf.statChanges[stat] += side.opponent.effectOnOpponent.statChanges[stat];
                    side.opponent.effectOnOpponent.statChanges[stat] = 0;
                    break;
            }
        }

        switch (specialImpacts) {
            case Behaviour.Block:
                side.opponent.effectOnOpponent.successfulSpecial = null;
                break;
            case Behaviour.Reflect:
                if (side.opponent.effectOnOpponent.successfulSpecial != null) {
                    side.effectOnOpponent.successfulSpecial = side.opponent.effectOnOpponent.successfulSpecial;
                    side.opponent.effectOnOpponent.successfulSpecial = null;
                }
                break;
        }

        return;
    }
}