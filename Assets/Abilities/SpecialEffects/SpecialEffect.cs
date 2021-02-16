using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialEffect : ScriptableObject
{
    protected void SwitchToSelfEffect(BattleSide side) {        
        side.effectOnSelf.successfulSpecial = side.effectOnOpponent.successfulSpecial;
        side.effectOnOpponent.successfulSpecial = null;
    }

    // Simple effects like miss/critical or stat effects apply here.
    // Effects that affect the user take this opportunity to swap to the "effectOnSelf" channel.
    public virtual void ModifyRolledAbility(BattleSide side, bool success, System.Random dice) {}    

    // Effects that do something special (like inflict a status effect), 
    // only after the opponent has had a chance to block or reflect them, apply here.
    public virtual void ApplyPreResolveEffects(BattleSide side, Monster target, System.Random dice) { }

    // Effects that need to have the last say (like clearing a status effect) apply here.
    public virtual void ApplyPostResolveEffects(BattleSide side, Monster target, System.Random dice) { }
}

public abstract class MutualModifierEffect : SpecialEffect 
{
    public float priorityOrder;

    // Effects that need to look at the opponent's ability (to block/reflect/copy it) apply here.    
    public abstract void OnPostAbilitiesRolled(BattleSide side, Monster target, System.Random dice);
}
