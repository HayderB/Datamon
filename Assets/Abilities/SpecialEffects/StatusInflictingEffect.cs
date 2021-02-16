using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewStatusInflictingEffect.asset", menuName ="DataMon/Special Effect/Inflict Status")]
public class StatusInflictingEffect : SpecialEffect
{
    public StatusEffect effectToInflict;
    public bool applyToSelf;

    public override void ModifyRolledAbility(BattleSide side, bool success, System.Random dice) {
        if (applyToSelf) {
            SwitchToSelfEffect(side);
        }
    }

    public override void ApplyPreResolveEffects(BattleSide side, Monster target, System.Random dice) {
        effectToInflict.ApplyTo(target);

        if (effectToInflict is ElementShiftStatusEffect) {
            target.currentElement = side.selectedAbility.element;
        }
    }
}
